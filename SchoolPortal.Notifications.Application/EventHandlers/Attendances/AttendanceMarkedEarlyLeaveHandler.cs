using MediatR;
using Microsoft.Extensions.Logging;
using SchoolPortal.Notifications.Application.Directories;
using SchoolPortal.Notifications.Domain.Notifications;
using SchoolPortal.SharedKernel.Domain.Events.Attendances;

namespace SchoolPortal.Notifications.Application.EventHandlers.Attendances;

public sealed class AttendanceMarkedEarlyLeaveHandler(
    INotificationDirectory directory,
    ILogger<AttendanceMarkedEarlyLeaveHandler> logger)
    : INotificationHandler<AttendanceMarkedEarlyLeave>
{
    public async Task Handle(
        AttendanceMarkedEarlyLeave notification,
        CancellationToken ct)
    {
        logger.LogInformation(
            "Student {StudentId} was marked early leave for occurrence {SessionOccurrenceId}",
            notification.StudentId,
            notification.SessionOccurrenceId);

        var record = Notification.Create(
            notification.StudentId,
            "You were marked as leaving class early.");

        directory.Add(record);

        await directory.SaveChangesAsync(ct);
    }
}