using Microsoft.EntityFrameworkCore;
using SchoolPortal.Notifications.Application.Persistence;
using SchoolPortal.Notifications.Domain.Notifications;

namespace SchoolPortal.Notifications.Infrastructure.Persistence;

internal sealed class NotificationRepository(NotificationsDbContext dbContext) : INotificationDirectory
{
    public void Add(Notification notification) => dbContext.Notifications.Add(notification);

    public async Task<IReadOnlyList<Notification>> GetForRecipientAsync(int recipientId, CancellationToken ct) =>
        await dbContext.Notifications.Where(n => n.RecipientId == recipientId).OrderByDescending(n => n.CreatedAtUtc)
            .ToListAsync(ct);

    public async Task SaveChangesAsync(CancellationToken ct) => await dbContext.SaveChangesAsync(ct);
}