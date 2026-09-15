using MediatR;
using Microsoft.Extensions.Logging;
using SchoolPortal.Notifications.Application.Persistence;
using SchoolPortal.Notifications.Domain.Notifications;
using SchoolPortal.SharedKernel.Domain.Events.Students.Enrollments;

namespace SchoolPortal.Notifications.Application.EventHandlers.Students.Enrollments;

public sealed class EnrollmentWithdrawnHandler(
    INotificationDirectory directory,
    ILogger<EnrollmentWithdrawnHandler> logger)
    : INotificationHandler<EnrollmentWithdrawn>
{
    public async Task Handle(
        EnrollmentWithdrawn notification,
        CancellationToken ct)
    {
        logger.LogInformation(
            "Enrollment {EnrollmentId} for student {StudentId} was withdrawn effective {EffectiveDate}",
            notification.EnrollmentId,
            notification.StudentId,
            notification.EffectiveDate);

        var record = Notification.Create(
            notification.StudentId,
            $"Your enrollment in course section {notification.CourseSectionId} " +
            $"was withdrawn effective {notification.EffectiveDate:yyyy-MM-dd}.");

        directory.Add(record);

        await directory.SaveChangesAsync(ct);
    }
}