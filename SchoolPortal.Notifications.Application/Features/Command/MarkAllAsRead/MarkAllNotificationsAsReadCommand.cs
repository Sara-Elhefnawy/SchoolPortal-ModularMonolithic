using MediatR;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Notifications.Application.Features.Command.MarkAllAsRead;

public sealed record MarkAllNotificationsAsReadCommand(int RecipientId) : IRequest<Result>;