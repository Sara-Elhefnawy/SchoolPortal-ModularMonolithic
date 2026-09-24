using MediatR;
using Microsoft.Extensions.Logging;
using SchoolPortal.Notifications.Application.Directories;
using SchoolPortal.Notifications.Domain.Notifications;
using SchoolPortal.SharedKernel.Domain.Events.Grades;

namespace SchoolPortal.Notifications.Application.EventHandlers.Grades;

public sealed class GradeDraftedHandler(
    INotificationDirectory directory,
    ILogger<GradeSubmittedHandler> logger)
    : INotificationHandler<GradeDrafted>
{
    public async Task Handle(
        GradeDrafted notification,
        CancellationToken ct)
    {
        logger.LogInformation(
            "Grade {GradeId} for student {StudentId} was drafted",
            notification.GradeId,
            notification.StudentId);

        var record = Notification.Create(
            notification.StudentId,
            "Your grade has been drafted.");

        directory.Add(record);

        await directory.SaveChangesAsync(ct);
    }
}