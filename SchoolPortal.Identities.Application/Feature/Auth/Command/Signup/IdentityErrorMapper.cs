using Microsoft.AspNetCore.Identity;
using SchoolPortal.Identities.Domain;
using SchoolPortal.SharedKernel.Results.Errors;

namespace SchoolPortal.Identities.Application.Feature.Auth.Command.Signup;

public static class IdentityErrorMapper
{
    public static Error Map(IdentityError error)
    {
        return error.Code switch
        {
            "DuplicateUserName" =>
                AuthErrors.EmailAlreadyExists,

            "DuplicateEmail" =>
                AuthErrors.EmailAlreadyExists,

            "InvalidEmail" =>
                AuthErrors.InvalidEmail,

            "PasswordTooShort" =>
                AuthErrors.PasswordTooShort,

            "PasswordRequiresNonAlphanumeric" =>
                AuthErrors.PasswordRequiresNonAlphanumeric,

            "PasswordRequiresDigit" =>
                AuthErrors.PasswordRequiresDigit,

            "PasswordRequiresUpper" =>
                AuthErrors.PasswordRequiresUpper,

            "PasswordRequiresLower" =>
                AuthErrors.PasswordRequiresLower,

            _ =>
                AuthErrors.SignupFailed
        };
    }
}