using MediatR;
using Microsoft.Extensions.Logging;
using SchoolPortal.Notifications.Application.Directories;
using SchoolPortal.Notifications.Domain.Notifications;
using SchoolPortal.SharedKernel.Domain.Events.Grades;

namespace SchoolPortal.Notifications.Application.EventHandlers.Grades;

public sealed class GradeChangedHandler(
    INotificationDirectory directory,
    ILogger<GradeChangedHandler> logger)
    : INotificationHandler<GradeChanged>
{
    public async Task Handle(
        GradeChanged notification,
        CancellationToken ct)
    {
        logger.LogInformation(
            "Grade {GradeId} for student {StudentId} changed from {OldScore} to {NewScore}",
            notification.GradeId,
            notification.StudentId,
            notification.OldScore,
            notification.NewScore);

        var record = Notification.Create(
            notification.StudentId,
            $"Your grade was changed from {notification.OldScore} to {notification.NewScore}.");

        directory.Add(record);

        await directory.SaveChangesAsync(ct);
    }
}