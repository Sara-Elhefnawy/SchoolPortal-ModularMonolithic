using MediatR;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Notifications.Application.Features.Command.MarkAsRead;

public sealed record MarkNotificationAsReadCommand(int NotificationId, int RecipientId) : IRequest<Result>;