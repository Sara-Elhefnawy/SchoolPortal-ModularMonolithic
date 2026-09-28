using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using SchoolPortal.SharedKernel.Mediator;
using Microsoft.AspNetCore.Identity;
using SchoolPortal.Identities.Domain;
using SchoolPortal.Identities.Domain.Users;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Auth.Command.Login;

public sealed class LoginCommandHandler(
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager) : IRequestHandler<LoginCommand, Result>
{
    public async Task<Result> Handle(LoginCommand request, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return Result.Failure(AuthErrors.InvalidCredentials);

        if (!user.EmailConfirmed)
            return Result.Failure(AuthErrors.EmailNotConfirmed);

        if (user.IsDeleted || !user.IsActive)
            return Result.Failure(AuthErrors.AccountDisabled);

        // Check the password without creating the authentication cookie yet.
        var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

        if (result.IsLockedOut)
            return Result.Failure(AuthErrors.AccountLocked);

        if (!result.Succeeded)
            return Result.Failure(AuthErrors.InvalidCredentials);

        // Create the normal Identity principal to show the username on dashboard instead of email.
        // This includes the roles and permission claims already provided by our custom claims factory.
        var principal = await signInManager.CreateUserPrincipalAsync(user);

        var identity = (ClaimsIdentity)principal.Identity!;

        // UserName is the email, so replace Identity's Name claim with the actual display name.
        foreach (var claim in identity.FindAll(ClaimTypes.Name).ToList())
            identity.RemoveClaim(claim);

        identity.AddClaim(new Claim(ClaimTypes.Name, user.Name));

        // Write the corrected principal into the authentication cookie.
        await signInManager.Context.SignInAsync(IdentityConstants.ApplicationScheme, principal,
            new AuthenticationProperties { IsPersistent = false });

        return Result.Success();
    }
}