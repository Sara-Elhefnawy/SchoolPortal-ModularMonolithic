using MediatR;
using Microsoft.Extensions.Logging;
using SchoolPortal.Notifications.Application.Directories;
using SchoolPortal.Notifications.Domain.Notifications;
using SchoolPortal.SharedKernel.Domain.Events.Grades;

namespace SchoolPortal.Notifications.Application.EventHandlers.Grades;

public sealed class GradeUnpublishedHandler(
    INotificationDirectory directory,
    ILogger<GradeUnpublishedHandler> logger)
    : INotificationHandler<GradeUnpublished>
{
    public async Task Handle(
        GradeUnpublished notification,
        CancellationToken ct)
    {
        logger.LogInformation(
            "Grade {GradeId} for student {StudentId} was unpublished",
            notification.GradeId,
            notification.StudentId);

        var record = Notification.Create(
            notification.StudentId,
            "A previously published grade has been unpublished.");

        directory.Add(record);

        await directory.SaveChangesAsync(ct);
    }
}