using SchoolPortal.Notifications.Domain.Notifications;

namespace SchoolPortal.Notifications.Application.Persistence;

public interface INotificationDirectory
{
    // Called by event handlers to record a new notification.
    void Add(Notification notification);

    // Called by whatever endpoint lets a user view their notifications (built later).
    Task<IReadOnlyList<Notification>> GetForRecipientAsync(int recipientId, CancellationToken ct);

    // Same reasoning as IStudentsUnitOfWork earlier: commits the DbContext.
    Task SaveChangesAsync(CancellationToken ct);
}