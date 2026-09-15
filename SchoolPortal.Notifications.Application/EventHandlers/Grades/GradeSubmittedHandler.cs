using MediatR;
using Microsoft.Extensions.Logging;
using SchoolPortal.Notifications.Application.Persistence;
using SchoolPortal.Notifications.Domain.Notifications;
using SchoolPortal.SharedKernel.Domain.Events.Grades;

namespace SchoolPortal.Notifications.Application.EventHandlers.Grades;

public sealed class GradeSubmittedHandler(
    INotificationDirectory directory,
    ILogger<GradeSubmittedHandler> logger)
    : INotificationHandler<GradeSubmitted>
{
    public async Task Handle(
        GradeSubmitted notification,
        CancellationToken ct)
    {
        logger.LogInformation(
            "Grade {GradeId} for student {StudentId} was submitted",
            notification.GradeId,
            notification.StudentId);

        var record = Notification.Create(
            notification.StudentId,
            "Your grade has been submitted for review.");

        directory.Add(record);

        await directory.SaveChangesAsync(ct);
    }
}