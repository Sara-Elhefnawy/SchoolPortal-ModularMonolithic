using MediatR;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Notifications.Application.Features.Query.GetMyNotifications;

public sealed record GetMyNotificationsQuery(int RecipientId) : IRequest<Result<IReadOnlyList<GetMyNotificationsQueryDto>>>;