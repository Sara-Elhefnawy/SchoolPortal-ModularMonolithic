using MediatR;
using Microsoft.Extensions.Logging;
using SchoolPortal.Notifications.Application.Directories;
using SchoolPortal.Notifications.Domain.Notifications;
using SchoolPortal.SharedKernel.Domain.Events.Grades;

namespace SchoolPortal.Notifications.Application.EventHandlers.Grades;

public sealed class GradePublishedHandler(
    INotificationDirectory directory,
    ILogger<GradePublishedHandler> logger)
    : INotificationHandler<GradePublished>
{
    public async Task Handle(
        GradePublished notification,
        CancellationToken ct)
    {
        logger.LogInformation(
            "Grade {GradeId} for student {StudentId} was published",
            notification.GradeId,
            notification.StudentId);

        var record = Notification.Create(
            notification.StudentId,
            "A grade has been published to your student record.");

        directory.Add(record);

        await directory.SaveChangesAsync(ct);
    }
}