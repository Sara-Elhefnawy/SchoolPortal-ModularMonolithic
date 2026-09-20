using MediatR;
using SchoolPortal.Notifications.Application.Directories;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Notifications.Application.Features.Query.GetMyNotifications;

public sealed class GetMyNotificationsQueryHandler(
    INotificationDirectory directory) 
    : IRequestHandler<GetMyNotificationsQuery, Result<IReadOnlyList<GetMyNotificationsQueryDto>>>
{
    public async Task<Result<IReadOnlyList<GetMyNotificationsQueryDto>>> Handle(GetMyNotificationsQuery request, CancellationToken ct)
    {
        var notifications = await directory.GetByRecipientIdAsync(request.RecipientId, ct);

        var result = notifications.Select(x => new GetMyNotificationsQueryDto(x.Id, x.Message, x.CreatedAtUtc, x.IsRead)).ToList();

        return Result<IReadOnlyList<GetMyNotificationsQueryDto>>.Success(result);
    }
}