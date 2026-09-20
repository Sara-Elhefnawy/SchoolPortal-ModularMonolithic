using SchoolPortal.SharedKernel.Results.Errors;

namespace SchoolPortal.Identities.Domain.Permissions;

public static class PermissionErrors
{
    public static readonly Error NotFound =
        Error.Validation("Permissions.NotFound", "The permission could not be found.");
}