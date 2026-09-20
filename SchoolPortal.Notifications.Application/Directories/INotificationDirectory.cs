using SchoolPortal.Notifications.Domain.Notifications;

namespace SchoolPortal.Notifications.Application.Directories;

public interface INotificationDirectory
{
    void Add(Notification notification);

    Task<Notification?> GetByIdAsync(int notificationId, CancellationToken ct);

    Task<IReadOnlyList<Notification>> GetByRecipientIdAsync(int recipientId, CancellationToken ct);

    Task<int> GetUnreadCountAsync(int recipientId, CancellationToken ct);

    Task MarkAllAsReadAsync(int recipientId, CancellationToken ct);

    Task SaveChangesAsync(CancellationToken ct);
}