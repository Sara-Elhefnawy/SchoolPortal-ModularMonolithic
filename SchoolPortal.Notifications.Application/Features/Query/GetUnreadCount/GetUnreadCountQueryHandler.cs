using MediatR;
using SchoolPortal.Notifications.Application.Directories;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Notifications.Application.Features.Query.GetUnreadCount;

public sealed class GetUnreadCountQueryHandler(
    INotificationDirectory directory)
    : IRequestHandler<GetUnreadCountQuery, Result<int>>
{
    public async Task<Result<int>> Handle(GetUnreadCountQuery request, CancellationToken ct)
    {
        var count = await directory.GetUnreadCountAsync(request.RecipientId, ct);

        return Result<int>.Success(count);
    }
}