using MediatR;
using Microsoft.Extensions.Logging;
using SchoolPortal.Notifications.Application.Directories;
using SchoolPortal.Notifications.Domain.Notifications;
using SchoolPortal.SharedKernel.Domain.Events.Attendances;

namespace SchoolPortal.Notifications.Application.EventHandlers.Attendances;

public sealed class AttendanceMarkedLateHandler(
    INotificationDirectory directory,
    ILogger<AttendanceMarkedLateHandler> logger)
    : INotificationHandler<AttendanceMarkedLate>
{
    public async Task Handle(
        AttendanceMarkedLate notification,
        CancellationToken ct)
    {
        logger.LogInformation(
            "Student {StudentId} was marked late for occurrence {SessionOccurrenceId}",
            notification.StudentId,
            notification.SessionOccurrenceId);

        var record = Notification.Create(
            notification.StudentId,
            "You were marked late for a class.");

        directory.Add(record);

        await directory.SaveChangesAsync(ct);
    }
}