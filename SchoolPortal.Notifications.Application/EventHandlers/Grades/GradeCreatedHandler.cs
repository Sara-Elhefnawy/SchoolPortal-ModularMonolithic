using SchoolPortal.SharedKernel.Mediator;
using Microsoft.Extensions.Logging;
using SchoolPortal.Notifications.Application.Directories;
using SchoolPortal.Notifications.Domain.Notifications;
using SchoolPortal.SharedKernel.Domain.Events.Grades;

namespace SchoolPortal.Notifications.Application.EventHandlers.Grades;

public sealed class GradeCreatedHandler(
    INotificationDirectory directory,
    ILogger<GradeCreatedHandler> logger)
    : INotificationHandler<GradeCreated>
{
    public async Task Handle(
        GradeCreated notification,
        CancellationToken ct)
    {
        logger.LogInformation(
            "Grade created for student {StudentId}, assessment {AssessmentId}, score {Score}",
            notification.StudentId,
            notification.AssessmentId,
            notification.Score);

        var record = Notification.Create(
            notification.StudentId,
            $"A grade of {notification.Score} was created for assessment {notification.AssessmentId}.");

        directory.Add(record);

        await directory.SaveChangesAsync(ct);
    }
}