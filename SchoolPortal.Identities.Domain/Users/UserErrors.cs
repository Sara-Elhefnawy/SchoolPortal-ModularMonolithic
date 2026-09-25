using SchoolPortal.SharedKernel.Results.Errors;

namespace SchoolPortal.Identities.Domain.Users;

public static class UserErrors
{
    public static readonly Error NotFound =
        Error.Validation("Users.NotFound", "The user could not be found.");

    public static readonly Error CreationFailed =
        Error.Validation("Users.CreationFailed", "The user could not be created.");

    public static readonly Error UpdateFailed =
        Error.Validation("Users.UpdateFailed", "The user could not be updated.");

    public static readonly Error DeletionFailed =
        Error.Validation("Users.DeletionFailed", "The user could not be deleted.");

    public static readonly Error RestorationFailed =
        Error.Validation("Users.RestorationFailed", "The user could not be restored.");

    public static readonly Error RoleAssignmentFailed =
        Error.Validation("Users.RoleAssignmentFailed", "The user's role could not be assigned.");

    public static readonly Error EmailConfirmationFailed =
        Error.Validation("Users.EmailConfirmationFailed", "Failed to create email confirmation.");
}