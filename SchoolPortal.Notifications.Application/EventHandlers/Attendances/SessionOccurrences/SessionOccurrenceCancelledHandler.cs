using MediatR;
using Microsoft.Extensions.Logging;
using SchoolPortal.SharedKernel.Domain.Events.Attendances.SessionOccurrences;

namespace SchoolPortal.Notifications.Application.EventHandlers.Attendances.SessionOccurrences;

public sealed class SessionOccurrenceCancelledHandler(
    ILogger<SessionOccurrenceCancelledHandler> logger)
    : INotificationHandler<SessionOccurrenceCancelled>
{
    public Task Handle(SessionOccurrenceCancelled notification, CancellationToken ct)
    {
        logger.LogInformation(
            "Session occurrence {SessionOccurrenceId} " + "for session {SessionId} was cancelled on {Date}.",
            notification.Id, notification.SessionId, notification.Date);

        return Task.CompletedTask;
    }
}