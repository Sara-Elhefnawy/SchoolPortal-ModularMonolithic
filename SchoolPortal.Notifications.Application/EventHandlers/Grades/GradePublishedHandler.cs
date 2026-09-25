using SchoolPortal.SharedKernel.Mediator;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SchoolPortal.Notifications.Application.Directories;
using SchoolPortal.Notifications.Domain.Notifications;
using SchoolPortal.SharedKernel.Domain.Events.Grades;
using SchoolPortal.SharedKernel.Emails;
using SchoolPortal.SharedKernel.Emails.Options;
using SchoolPortal.SharedKernel.Identity.Contracts;

namespace SchoolPortal.Notifications.Application.EventHandlers.Grades;

public sealed class GradePublishedHandler(
    INotificationDirectory directory,
    IUserContactDirectory contactDirectory,
    IEmailSender emailSender,
    IOptions<AppOptions> appOptions,
    ILogger<GradePublishedHandler> logger) : INotificationHandler<GradePublished>
{
    public async Task Handle(GradePublished notification, CancellationToken ct)
    {
        // Notification record first, unconditionally
        // this should never silently not happen just because Gmail is slow or down.
        var record = Notification.Create(notification.StudentId, "A grade has been published to your student record.");

        directory.Add(record);
        await directory.SaveChangesAsync(ct);

        // Email second, best-effort
        // a failure here is logged, not thrown, so it can never roll back or block the notification write above.
        var email = await contactDirectory.GetEmailAsync(notification.StudentId, ct);
        if (email is null)
        {
            logger.LogWarning("No email on file for student {StudentId}, skipping grade-published email",
                notification.StudentId);
            return;
        }

        var link = $"{appOptions.Value.PublicBaseUrl}/grades/{notification.GradeId}";
        
        var result = await emailSender.SendGradePublishedEmailAsync(email, link, ct);
        if (result.IsFailure)
            logger.LogWarning("Grade-published email failed to send to {Email}", email);
    }
}