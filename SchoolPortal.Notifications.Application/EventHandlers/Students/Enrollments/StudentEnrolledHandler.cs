using SchoolPortal.SharedKernel.Mediator;
using Microsoft.Extensions.Logging;
using SchoolPortal.Notifications.Application.Directories;
using SchoolPortal.Notifications.Domain.Notifications;
using SchoolPortal.SharedKernel.Domain.Events.Students.Enrollments;

namespace SchoolPortal.Notifications.Application.EventHandlers.Students.Enrollments;

public sealed class StudentEnrolledHandler(
    INotificationDirectory directory,
    ILogger<StudentEnrolledHandler> logger)
    : INotificationHandler<StudentEnrolled>
{
    public async Task Handle(
        StudentEnrolled notification,
        CancellationToken ct)
    {
        logger.LogInformation(
            "Student {StudentId} enrolled in course section {CourseSectionId}",
            notification.StudentId,
            notification.CourseSectionId);

        var record = Notification.Create(
            notification.StudentId,
            $"You have been enrolled in course section {notification.CourseSectionId}.");

        directory.Add(record);

        await directory.SaveChangesAsync(ct);
    }
}