using Microsoft.EntityFrameworkCore;
using SchoolPortal.Notifications.Domain.Notifications;

namespace SchoolPortal.Notifications.Infrastructure.Persistence;

// Its own DbContext, its own schema — same "one DbContext per module" rule you're
// already following for Students/Grades/Attendance. Notifications doesn't share a
// database table with any other module.
public sealed class NotificationsDbContext(DbContextOptions<NotificationsDbContext> options)
    : DbContext(options)
{
    public DbSet<Notification> Notifications => Set<Notification>();
}