using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Notifications.Application.Features.Query.GetUnreadCount;

public sealed record GetUnreadCountQuery(int RecipientId) : IRequest<Result<int>>;