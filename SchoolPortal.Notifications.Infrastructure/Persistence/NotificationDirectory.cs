using Microsoft.EntityFrameworkCore;
using SchoolPortal.Notifications.Application.Directories;
using SchoolPortal.Notifications.Domain.Notifications;

namespace SchoolPortal.Notifications.Infrastructure.Persistence;

internal sealed class NotificationDirectory(NotificationsDbContext dbContext) : INotificationDirectory
{
    public void Add(Notification notification) => dbContext.Notifications.Add(notification);

    public async Task<Notification?> GetByIdAsync(int notificationId, CancellationToken ct) =>
        await dbContext.Notifications.FirstOrDefaultAsync(n => n.Id == notificationId, ct);

    public async Task<IReadOnlyList<Notification>> GetByRecipientIdAsync(int recipientId, CancellationToken ct) =>
        await dbContext.Notifications.Where(n => n.RecipientId == recipientId).OrderByDescending(n => n.CreatedAtUtc)
            .ToListAsync(ct);

    public async Task<int> GetUnreadCountAsync(int recipientId, CancellationToken ct) =>
        await dbContext.Notifications.CountAsync(n => n.RecipientId == recipientId && !n.IsRead, ct);

    public async Task MarkAllAsReadAsync(int recipientId, CancellationToken ct)
    {
        var notifications = await dbContext.Notifications.Where(n => n.RecipientId == recipientId && !n.IsRead)
            .ToListAsync(ct);

        foreach (var notification in notifications)
            notification.MarkAsRead();
    }

    public async Task SaveChangesAsync(CancellationToken ct) => await dbContext.SaveChangesAsync(ct);
}