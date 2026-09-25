using SchoolPortal.SharedKernel.Mediator;
using Microsoft.Extensions.Logging;
using SchoolPortal.Notifications.Application.Directories;
using SchoolPortal.Notifications.Domain.Notifications;
using SchoolPortal.SharedKernel.Domain.Events.Grades;

namespace SchoolPortal.Notifications.Application.EventHandlers.Grades;

public sealed class GradeLockedHandler(
    INotificationDirectory directory,
    ILogger<GradeLockedHandler> logger)
    : INotificationHandler<GradeLocked>
{
    public async Task Handle(
        GradeLocked notification,
        CancellationToken ct)
    {
        logger.LogInformation(
            "Grade {GradeId} for student {StudentId} was locked",
            notification.GradeId,
            notification.StudentId);

        var record = Notification.Create(
            notification.StudentId,
            "Your grade has been locked and can no longer be changed.");

        directory.Add(record);

        await directory.SaveChangesAsync(ct);
    }
}