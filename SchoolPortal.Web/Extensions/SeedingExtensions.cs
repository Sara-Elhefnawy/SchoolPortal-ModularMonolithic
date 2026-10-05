using SchoolPortal.SharedKernel.Persistence.Seeders;

namespace SchoolPortal.Web.Extensions;

public static class SeedingExtensions
{
    public static async Task SeedDatabaseAsync(this WebApplication app)
    {
        // WHY a scope: seeders use scoped services (DbContexts, UserManager).
        using var scope = app.Services.CreateScope();

        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseSeeding");

        // WHY OrderBy: Identity (10) must run before Students (20) before the link seeder (30).
        var seeders = scope.ServiceProvider.GetServices<IDataSeeder>().OrderBy(s => s.Order).ToList();

        logger.LogInformation("Found {Count} database seeders.", seeders.Count);

        foreach (var seeder in seeders)
        {
            logger.LogInformation("Running database seeder: {Seeder} (Order {Order})", seeder.GetType().FullName,
                seeder.Order);

            await seeder.SeedAsync(app.Lifetime.ApplicationStopping);

            logger.LogInformation("Completed database seeder: {Seeder}", seeder.GetType().FullName);
        }
    }
}