using SchoolPortal.SharedKernel.Mediator;
using Microsoft.Extensions.Logging;
using SchoolPortal.Notifications.Application.Directories;
using SchoolPortal.Notifications.Domain.Notifications;
using SchoolPortal.SharedKernel.Domain.Events.Grades;

namespace SchoolPortal.Notifications.Application.EventHandlers.Grades;

public sealed class GradeUnlockedHandler(
    INotificationDirectory directory,
    ILogger<GradeUnlockedHandler> logger)
    : INotificationHandler<GradeUnlocked>
{
    public async Task Handle(
        GradeUnlocked notification,
        CancellationToken ct)
    {
        logger.LogInformation(
            "Grade {GradeId} for student {StudentId} was unlocked",
            notification.GradeId,
            notification.StudentId);

        var record = Notification.Create(
            notification.StudentId,
            "Your grade has been locked and can no longer be changed by you. only admins can.");

        directory.Add(record);

        await directory.SaveChangesAsync(ct);
    }
}