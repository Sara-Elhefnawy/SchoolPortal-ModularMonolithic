using MediatR;
using Microsoft.Extensions.Logging;
using SchoolPortal.SharedKernel.Domain.Events.Attendances;

namespace SchoolPortal.Notifications.Application.EventHandlers.Attendances;

public sealed class AttendanceMarkedPresentHandler(
    ILogger<AttendanceMarkedPresentHandler> logger)
    : INotificationHandler<AttendanceMarkedPresent>
{
    public Task Handle(
        AttendanceMarkedPresent notification,
        CancellationToken ct)
    {
        logger.LogInformation(
            "Student {StudentId} was marked present for occurrence {SessionOccurrenceId}",
            notification.StudentId,
            notification.SessionOccurrenceId);

        return Task.CompletedTask;
    }
}