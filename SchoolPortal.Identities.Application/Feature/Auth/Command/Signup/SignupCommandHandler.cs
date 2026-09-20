using MediatR;
using Microsoft.AspNetCore.Identity;
using SchoolPortal.Identities.Domain;
using SchoolPortal.Identities.Domain.Users;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Auth.Command.Signup;

public sealed class SignupCommandHandler(
    UserManager<ApplicationUser> userManager,
    RoleManager<Domain.Roles.ApplicationRole> roleManager,
    SignInManager<ApplicationUser> signInManager)
    : IRequestHandler<SignupCommand, Result>
{
    public async Task<Result> Handle(
        SignupCommand request,
        CancellationToken ct)
    {
        if (request.Password != request.ConfirmPassword)
            return Result.Failure(AuthErrors.PasswordsDoNotMatch);

        var existingUser = await userManager.FindByEmailAsync(request.Email);

        if (existingUser is not null)
        {
            if (existingUser.IsDeleted)
                return Result.Failure(AuthErrors.AccountExists);

            return Result.Failure(AuthErrors.EmailAlreadyExists);
        }

        var studentRole = await roleManager.FindByNameAsync(SharedKernel.Identity.Roles.Student);

        if (studentRole is null || studentRole.IsDeleted)
            return Result.Failure(AuthErrors.DefaultRoleNotAvailable);

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            Name = request.FullName,
            IsActive = true,
            EmailConfirmed = false
        };

        var createResult = await userManager.CreateAsync(user, request.Password);

        if (!createResult.Succeeded)
        {
            var error = createResult.Errors.FirstOrDefault();

            return Result.Failure(
                error is null
                    ? AuthErrors.SignupFailed
                    : IdentityErrorMapper.Map(error));
        }

        var roleResult = await userManager.AddToRoleAsync(user, SharedKernel.Identity.Roles.Student);

        if (!roleResult.Succeeded)
        {
            await userManager.DeleteAsync(user);

            var error = roleResult.Errors.FirstOrDefault();

            return Result.Failure(
                AuthErrors.RoleAssignmentFailed);
        }

        await signInManager.SignInAsync(user, isPersistent: false);

        return Result.Success();
    }
}