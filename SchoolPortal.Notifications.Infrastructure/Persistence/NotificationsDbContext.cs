using Microsoft.EntityFrameworkCore;
using SchoolPortal.Notifications.Domain.Notifications;

namespace SchoolPortal.Notifications.Infrastructure.Persistence;

// Its own DbContext, its own schema — same "one DbContext per module" rule you're
// already following for Students/Grades/Attendance. Notifications doesn't share a
// database table with any other module.
public sealed class NotificationsDbContext(DbContextOptions<NotificationsDbContext> options)
    : DbContext(options)
{
    public const string Schema = "notifications";
    
    public DbSet<Notification> Notifications => Set<Notification>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Without this, the Notifications table silently lands in dbo, sharing
        // a namespace with anything else that has no schema.
        modelBuilder.HasDefaultSchema(Schema);

        // Without this, any IEntityTypeConfiguration<T> in this module is never applied.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NotificationsDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}