using SchoolPortal.SharedKernel.Mediator;
using Microsoft.Extensions.Logging;
using SchoolPortal.Notifications.Application.Directories;
using SchoolPortal.Notifications.Domain.Notifications;
using SchoolPortal.SharedKernel.Domain.Events.Attendances;

namespace SchoolPortal.Notifications.Application.EventHandlers.Attendances;

public sealed class AttendanceMarkedPresentHandler(
    INotificationDirectory directory,
    ILogger<AttendanceMarkedPresentHandler> logger)
    : INotificationHandler<AttendanceMarkedPresent>
{
    public async Task Handle(AttendanceMarkedPresent notification, CancellationToken ct)
    {
        logger.LogInformation("Student {StudentId} was marked present for occurrence {SessionOccurrenceId}",
            notification.StudentId, notification.SessionOccurrenceId);

        var record = Notification.Create(notification.StudentId, "You were marked present for your attendance.");

        directory.Add(record);

        await directory.SaveChangesAsync(ct);
    }
}