using MediatR;
using Microsoft.Extensions.Logging;
using SchoolPortal.Notifications.Application.Directories;
using SchoolPortal.Notifications.Domain.Notifications;
using SchoolPortal.SharedKernel.Domain.Events.Students;

namespace SchoolPortal.Notifications.Application.EventHandlers.Students;

public sealed class StudentSuspendedHandler(
    INotificationDirectory directory,
    ILogger<StudentSuspendedHandler> logger)
    : INotificationHandler<StudentSuspended>
{
    public async Task Handle(
        StudentSuspended notification,
        CancellationToken ct)
    {
        logger.LogInformation(
            "Student {StudentId} was suspended. Reason: {Reason}",
            notification.StudentId,
            notification.Reason);

        var record = Notification.Create(
            notification.StudentId,
            $"Your student account has been suspended. Reason: {notification.Reason}");

        directory.Add(record);

        await directory.SaveChangesAsync(ct);
    }
}