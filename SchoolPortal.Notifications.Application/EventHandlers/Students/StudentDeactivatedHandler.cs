using MediatR;
using Microsoft.Extensions.Logging;
using SchoolPortal.Notifications.Application.Persistence;
using SchoolPortal.Notifications.Domain.Notifications;
using SchoolPortal.SharedKernel.Domain.Events.Students;

namespace SchoolPortal.Notifications.Application.EventHandlers.Students;

public sealed class StudentDeactivatedHandler(
    INotificationDirectory directory,
    ILogger<StudentDeactivatedHandler> logger)
    : INotificationHandler<StudentDeactivated>
{
    public async Task Handle(
        StudentDeactivated notification,
        CancellationToken ct)
    {
        logger.LogInformation(
            "Student {StudentId} was deactivated",
            notification.StudentId);

        var record = Notification.Create(
            notification.StudentId,
            "Your student account has been deactivated.");

        directory.Add(record);

        await directory.SaveChangesAsync(ct);
    }
}