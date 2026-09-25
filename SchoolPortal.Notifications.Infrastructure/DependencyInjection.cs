using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SchoolPortal.Notifications.Application.Directories;
using SchoolPortal.Notifications.Application.EventHandlers.Students;
using SchoolPortal.Notifications.Infrastructure.Persistence;
using SchoolPortal.SharedKernel.Mediator;

namespace SchoolPortal.Notifications.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddNotificationsModule(this IServiceCollection services,
        IConfiguration config)
    {
        var connectionString = config.GetConnectionString("SchoolPortalDb") ??
                               throw new InvalidOperationException("Connection string 'SchoolPortalDb' was not found.");

        services.AddDbContext<NotificationsDbContext>(opt => opt.UseNpgsql(connectionString,
            // Same reason as every other module: its own history table in its own schema.
            npgsqlOptions =>
                npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", NotificationsDbContext.Schema)));

        services.AddScoped<INotificationDirectory, NotificationDirectory>();
        
        services.AddMediatorPattern(typeof(StudentCreatedHandler).Assembly);
        
        return services;
    }
}