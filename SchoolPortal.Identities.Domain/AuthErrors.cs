using SchoolPortal.SharedKernel.Results.Errors;

namespace SchoolPortal.Identities.Domain;

public static class AuthErrors
{
    public static readonly Error InvalidCredentials =
        Error.Validation("Auth.InvalidCredentials", "Invalid email or password.");

    public static readonly Error AccountLocked =
        Error.Validation("Auth.AccountLocked", "Account is locked. Try again later.");

    public static readonly Error AccountDisabled =
        Error.Validation("Auth.AccountDisabled", "Account is disabled. Try again later.");

    public static readonly Error PasswordsDoNotMatch =
        Error.Validation("Auth.PasswordsDoNotMatch", "Passwords must match.");
    
    public static readonly Error AccountExists =
        Error.Validation("Auth.AccountExists", "Account already exists.");
    
    public static readonly Error EmailAlreadyExists =
        Error.Validation("Auth.EmailAlreadyExists", "Email already exists.");
    
    public static readonly Error DefaultRoleNotAvailable =
        Error.Validation("Auth.DefaultRoleNotAvailable", "The default role is not available.");
    
    public static Error PasswordTooShort =>
        Error.Validation("Auth.PasswordTooShort", "The password does not meet the minimum length.");

    public static Error PasswordRequiresNonAlphanumeric =>
        Error.Validation("Auth.PasswordRequiresNonAlphanumeric",
            "The password must contain at least one non-alphanumeric character.");

    public static Error PasswordRequiresDigit =>
        Error.Validation("Auth.PasswordRequiresDigit", "The password must contain at least one number.");

    public static Error PasswordRequiresUpper =>
        Error.Validation(
            "Auth.PasswordRequiresUpper",
            "The password must contain at least one uppercase letter.");

    public static Error PasswordRequiresLower =>
        Error.Validation(
            "Auth.PasswordRequiresLower",
            "The password must contain at least one lowercase letter.");

    public static Error InvalidEmail =>
        Error.Validation("Auth.InvalidEmail", "The email address is invalid.");

    public static Error SignupFailed =>
        Error.Validation("Auth.SignupFailed", "The account could not be created.");

    public static Error RoleAssignmentFailed =>
        Error.Validation("Auth.RoleAssignmentFailed", "The account role could not be assigned.");

    public static Error InvalidTokenOrEmail =>
        Error.Validation("Auth.InvalidTokenOrEmail", "This reset link is invalid or has expired.");
}