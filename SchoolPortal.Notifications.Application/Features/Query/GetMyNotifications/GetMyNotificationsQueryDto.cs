namespace SchoolPortal.Notifications.Application.Features.Query.GetMyNotifications;

public sealed record GetMyNotificationsQueryDto(int Id, string Message, DateTime CreatedAtUtc,  bool IsRead);