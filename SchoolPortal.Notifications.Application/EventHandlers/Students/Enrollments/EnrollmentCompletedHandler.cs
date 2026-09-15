using MediatR;
using Microsoft.Extensions.Logging;
using SchoolPortal.Notifications.Application.Persistence;
using SchoolPortal.Notifications.Domain.Notifications;
using SchoolPortal.SharedKernel.Domain.Events.Students.Enrollments;

namespace SchoolPortal.Notifications.Application.EventHandlers.Students.Enrollments;

public sealed class EnrollmentCompletedHandler(
    INotificationDirectory directory,
    ILogger<EnrollmentCompletedHandler> logger)
    : INotificationHandler<EnrollmentCompleted>
{
    public async Task Handle(
        EnrollmentCompleted notification,
        CancellationToken ct)
    {
        logger.LogInformation(
            "Enrollment {EnrollmentId} for student {StudentId} was completed",
            notification.EnrollmentId,
            notification.StudentId);

        var record = Notification.Create(
            notification.StudentId,
            $"Your enrollment in course section {notification.CourseSectionId} has been completed.");

        directory.Add(record);

        await directory.SaveChangesAsync(ct);
    }
}