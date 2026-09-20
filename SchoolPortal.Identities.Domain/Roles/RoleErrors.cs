using SchoolPortal.SharedKernel.Results.Errors;

namespace SchoolPortal.Identities.Domain.Roles;

public static class RoleErrors
{
    public static readonly Error NotFound = Error.Validation("Roles.NotFound", "The role could not be found.");

    public static readonly Error AlreadyAssigned =
        Error.Validation("Roles.AlreadyAssigned", "The role is already assigned.");

    public static readonly Error AlreadyExists =
        Error.Validation("Roles.AlreadyExists", "The role already exists.");

    public static readonly Error AssignmentFailed =
        Error.Validation("Roles.AssignmentFailed", "The role could not be assigned.");

    public static readonly Error CreationFailed =
        Error.Validation("Roles.CreationFailed", "The role could not be created.");

    public static readonly Error UpdateFailed =
        Error.Validation("Roles.UpdateFailed", "The role could not be updated.");

    public static readonly Error DeletionFailed =
        Error.Validation("Roles.DeletionFailed", "The role could not be deleted.");

    public static readonly Error RestorationFailed =
        Error.Validation("Roles.RestorationFailed", "The role could not be restored.");
}