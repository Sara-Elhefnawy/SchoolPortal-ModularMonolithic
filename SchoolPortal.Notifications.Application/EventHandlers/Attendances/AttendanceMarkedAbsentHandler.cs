using MediatR;
using Microsoft.Extensions.Logging;
using SchoolPortal.Notifications.Application.Persistence;
using SchoolPortal.Notifications.Domain.Notifications;
using SchoolPortal.SharedKernel.Domain.Events.Attendances;

namespace SchoolPortal.Notifications.Application.EventHandlers.Attendances;

public sealed class AttendanceMarkedAbsentHandler(
    INotificationDirectory directory,
    ILogger<AttendanceMarkedAbsentHandler> logger)
    : INotificationHandler<AttendanceMarkedAbsent>
{
    public async Task Handle(
        AttendanceMarkedAbsent notification,
        CancellationToken ct)
    {
        logger.LogInformation(
            "Student {StudentId} was marked absent for occurrence {SessionOccurrenceId}",
            notification.StudentId,
            notification.SessionOccurrenceId);

        var record = Notification.Create(
            notification.StudentId,
            "You were marked absent for a class.");

        directory.Add(record);

        await directory.SaveChangesAsync(ct);
    }
}