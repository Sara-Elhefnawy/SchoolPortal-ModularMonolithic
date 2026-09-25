using SchoolPortal.SharedKernel.Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SchoolPortal.Attendances.Infrastructure.Persistence;

// Used only by dotnet ef. EF creates this class itself, with no DI and no constructor
// arguments, so it can't receive IConfiguration and must read its own settings.
public sealed class AttendancesDbContextFactory : IDesignTimeDbContextFactory<AttendancesDbContext>
{
    public AttendancesDbContext CreateDbContext(string[] args)
    {
        // Read from an environment variable so no password is committed to GitHub.
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__SchoolPortalDb") ??
                               throw new InvalidOperationException(
                                   "Set ConnectionStrings__SchoolPortalDb before running dotnet ef.");

        var options = new DbContextOptionsBuilder<AttendancesDbContext>()
            // Must match the runtime registration so EF looks for the history table in the same place.
            .UseNpgsql(connectionString,
                npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", AttendancesDbContext.Schema)).Options;

        // Migrations never call SaveChanges, so nothing is ever published.
        // A do-nothing publisher satisfies the constructor.
        return new AttendancesDbContext(options, new NoOpPublisher());
    }

    private sealed class NoOpPublisher : IPublisher
    {
        public Task Publish(INotification notification, CancellationToken ct = default) => Task.CompletedTask;
    }
}