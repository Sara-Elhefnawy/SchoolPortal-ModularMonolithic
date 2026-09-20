using SchoolPortal.SharedKernel.Results.Errors;

namespace SchoolPortal.Notifications.Domain.Notifications;

public static class NotificationErrors
{
    public static readonly Error NotFound =
        Error.Validation("Notifications.NotFound", "The notification could not be found.");
}