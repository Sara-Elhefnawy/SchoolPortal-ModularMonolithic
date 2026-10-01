using Microsoft.Extensions.Logging;
using SchoolPortal.Identities.Application.Directories;
using SchoolPortal.Identities.Application.Services;
using SchoolPortal.SharedKernel.Domain.Events.Students;
using SchoolPortal.SharedKernel.Mediator;

namespace SchoolPortal.Identities.Application.EventHandlers.Students;

internal sealed class StudentCreatedLinkUserHandler(
    IUserLinkService userLink,
    ILogger<StudentCreatedLinkUserHandler> logger) : INotificationHandler<StudentCreated>
{
    public async Task Handle(StudentCreated notification, CancellationToken ct)
    {
        var result = await userLink.LinkByEmailAsync(notification.Email, notification.StudentId, ct);

        if (result.IsFailure)
        {
            // Not an error. The common case is an admin-created student that
            // has no user account yet. Log at information so it's discoverable
            // but doesn't page anyone.
            logger.LogInformation("Student {StudentId} ({Email}) was created without a matching user: {Reason}",
                notification.StudentId, notification.Email, result.Error);
            return;
        }

        logger.LogInformation("Student {StudentId} linked to user by email {Email}.", notification.StudentId,
            notification.Email);
    }
}