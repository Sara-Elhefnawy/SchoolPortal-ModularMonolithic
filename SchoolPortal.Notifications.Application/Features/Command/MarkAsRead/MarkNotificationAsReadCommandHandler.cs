using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.Notifications.Application.Directories;
using SchoolPortal.Notifications.Domain.Notifications;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Notifications.Application.Features.Command.MarkAsRead;

public sealed class MarkNotificationAsReadCommandHandler(
    INotificationDirectory directory)
    : IRequestHandler<MarkNotificationAsReadCommand, Result>
{
    public async Task<Result> Handle(
        MarkNotificationAsReadCommand request,
        CancellationToken ct)
    {
        var notification = await directory.GetByIdAsync(request.NotificationId, ct);

        if (notification is null)
            return Result.Failure(NotificationErrors.NotFound);

        if (notification.RecipientId != request.RecipientId)
            return Result.Failure(NotificationErrors.NotFound);

        notification.MarkAsRead();

        await directory.SaveChangesAsync(ct);

        return Result.Success();
    }
}