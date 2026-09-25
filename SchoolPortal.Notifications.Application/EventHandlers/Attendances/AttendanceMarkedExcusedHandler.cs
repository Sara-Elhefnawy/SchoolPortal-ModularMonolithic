using SchoolPortal.SharedKernel.Mediator;
using Microsoft.Extensions.Logging;
using SchoolPortal.Notifications.Application.Directories;
using SchoolPortal.Notifications.Domain.Notifications;
using SchoolPortal.SharedKernel.Domain.Events.Attendances;

namespace SchoolPortal.Notifications.Application.EventHandlers.Attendances;

public sealed class AttendanceMarkedExcusedHandler(
    INotificationDirectory directory,
    ILogger<AttendanceMarkedExcusedHandler> logger)
    : INotificationHandler<AttendanceMarkedExcused>
{
    public async Task Handle(
        AttendanceMarkedExcused notification,
        CancellationToken ct)
    {
        logger.LogInformation(
            "Student {StudentId} was marked excused for occurrence {SessionOccurrenceId}",
            notification.StudentId,
            notification.SessionOccurrenceId);

        var record = Notification.Create(
            notification.StudentId,
            "Your absence was marked as excused.");

        directory.Add(record);

        await directory.SaveChangesAsync(ct);
    }
}