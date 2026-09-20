using MediatR;
using Microsoft.AspNetCore.Identity;
using SchoolPortal.Identities.Domain;
using SchoolPortal.Identities.Domain.Users;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Auth.Command.Login;

public sealed class LoginCommandHandler(
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager) 
    : IRequestHandler<LoginCommand, Result>
{
    public async Task<Result> Handle(LoginCommand request, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return Result.Failure(AuthErrors.InvalidCredentials);
        
        if (user.IsDeleted || !user.IsActive)
            return Result.Failure(AuthErrors.AccountDisabled);
        
        // when successful, creates the authentication cookie.
        // Because we store Email as UserName, request.Email can be used as the first argument here.
        var result = await signInManager.PasswordSignInAsync(
            user.UserName!, request.Password, isPersistent: false, lockoutOnFailure: true);

        if (result.IsLockedOut)
            return Result.Failure(AuthErrors.AccountLocked);

        if (!result.Succeeded)
            return Result.Failure(AuthErrors.InvalidCredentials);

        return Result.Success();
    }
}