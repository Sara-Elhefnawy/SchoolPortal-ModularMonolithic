using SchoolPortal.SharedKernel.Mediator;
using Microsoft.Extensions.Logging;
using SchoolPortal.Notifications.Application.Directories;
using SchoolPortal.Notifications.Domain.Notifications;
using SchoolPortal.SharedKernel.Domain.Events.Students;

namespace SchoolPortal.Notifications.Application.EventHandlers.Students;

public sealed class StudentGraduatedHandler(
    INotificationDirectory directory,
    ILogger<StudentGraduatedHandler> logger)
    : INotificationHandler<StudentGraduated>
{
    public async Task Handle(
        StudentGraduated notification,
        CancellationToken ct)
    {
        logger.LogInformation(
            "Student {StudentId} graduated on {GraduationDate}",
            notification.StudentId,
            notification.GraduationDate);

        var record = Notification.Create(
            notification.StudentId,
            $"Congratulations! You graduated on {notification.GraduationDate:yyyy-MM-dd}.");

        directory.Add(record);

        await directory.SaveChangesAsync(ct);
    }
}