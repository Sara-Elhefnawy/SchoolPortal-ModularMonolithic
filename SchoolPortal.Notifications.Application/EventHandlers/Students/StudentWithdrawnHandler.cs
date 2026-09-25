using SchoolPortal.SharedKernel.Mediator;
using Microsoft.Extensions.Logging;
using SchoolPortal.Notifications.Application.Directories;
using SchoolPortal.Notifications.Domain.Notifications;
using SchoolPortal.SharedKernel.Domain.Events.Students;

namespace SchoolPortal.Notifications.Application.EventHandlers.Students;

public sealed class StudentWithdrawnHandler(
    INotificationDirectory directory,
    ILogger<StudentWithdrawnHandler> logger)
    : INotificationHandler<StudentWithdrawn>
{
    public async Task Handle(
        StudentWithdrawn notification,
        CancellationToken ct)
    {
        logger.LogInformation(
            "Student {StudentId} withdrew effective {EffectiveDate}",
            notification.StudentId,
            notification.EffectiveDate);

        var record = Notification.Create(
            notification.StudentId,
            $"Your withdrawal is effective {notification.EffectiveDate:yyyy-MM-dd}.");

        directory.Add(record);

        await directory.SaveChangesAsync(ct);
    }
}