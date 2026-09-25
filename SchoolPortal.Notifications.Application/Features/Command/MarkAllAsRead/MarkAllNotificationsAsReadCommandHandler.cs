using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.Notifications.Application.Directories;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Notifications.Application.Features.Command.MarkAllAsRead;

public sealed class MarkAllNotificationsAsReadCommandHandler(
    INotificationDirectory directory)
    : IRequestHandler<MarkAllNotificationsAsReadCommand, Result>
{
    public async Task<Result> Handle(MarkAllNotificationsAsReadCommand request, CancellationToken ct)
    {
        await directory.MarkAllAsReadAsync(request.RecipientId, ct);

        return Result.Success();
    }
}