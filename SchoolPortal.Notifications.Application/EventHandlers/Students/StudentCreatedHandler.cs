using MediatR;
using Microsoft.Extensions.Logging;
using SchoolPortal.Notifications.Application.Directories;
using SchoolPortal.Notifications.Domain.Notifications;
using SchoolPortal.SharedKernel.Domain.Events.Students;

namespace SchoolPortal.Notifications.Application.EventHandlers.Students;

public sealed class StudentCreatedHandler(
    INotificationDirectory directory,
    ILogger<StudentCreatedHandler> logger)
    : INotificationHandler<StudentCreated>
{
    public async Task Handle(
        StudentCreated notification,
        CancellationToken ct)
    {
        logger.LogInformation(
            "Student {StudentId} ({FullName}) was created",
            notification.StudentId,
            notification.FullName);

        var record = Notification.Create(
            notification.StudentId,
            $"Welcome to SchoolPortal, {notification.FullName}.");

        directory.Add(record);

        await directory.SaveChangesAsync(ct);
    }
}