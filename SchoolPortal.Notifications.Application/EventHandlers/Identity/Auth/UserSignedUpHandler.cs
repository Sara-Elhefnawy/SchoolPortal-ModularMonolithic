using Microsoft.Extensions.Logging;
using SchoolPortal.SharedKernel.Events.Identity.Auth;
using SchoolPortal.SharedKernel.Mediator;

namespace SchoolPortal.Notifications.Application.EventHandlers.Identity.Auth;

public sealed class UserSignedUpHandler(ILogger<UserSignedUpHandler> logger) : INotificationHandler<UserSignedUp>
{
    public Task Handle(UserSignedUp notification, CancellationToken ct)
    {
        logger.LogInformation(
            "UserSignedUp notification received. UserId: {UserId}, Email: {Email}",
            notification.UserId,
            notification.Email);
        
        return Task.CompletedTask;
    }
}