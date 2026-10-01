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

    public static readonly Error InvalidStudentId =
        Error.Validation("Users.InvalidStudentId", "The student could not be found.");

    public static readonly Error StudentAlreadyLinked =
        Error.Validation("Users.StudentAlreadyLinked", "The student is already linked in DB.");

    public static readonly Error LinkFailed =
        Error.Validation("Users.LinkFailed", "The user could not be linked to a student record.");

    public static readonly Error InvalidEmail =
        Error.Validation("Users.InvalidEmail", "Email is required.");
    
    public static readonly Error NameRequired =
        Error.Validation("Users.NameRequired", "Name is required.");
}