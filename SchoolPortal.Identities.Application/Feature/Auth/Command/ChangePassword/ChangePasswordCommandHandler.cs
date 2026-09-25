using SchoolPortal.SharedKernel.Mediator;
using Microsoft.AspNetCore.Identity;
using SchoolPortal.Identities.Domain;
using SchoolPortal.Identities.Domain.Users;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Auth.Command.ChangePassword;

public sealed class ChangePasswordCommandHandler(
    UserManager<ApplicationUser> userManager) 
    : IRequestHandler<ChangePasswordCommand, Result>
{
    public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null || user.IsDeleted || !user.IsActive)
            return Result.Failure(AuthErrors.InvalidCredentials);

        var newPassword = await userManager.ChangePasswordAsync(user, request.Password, request.NewPassword);
        if (!newPassword.Succeeded)
            return Result.Failure(AuthErrors.InvalidCredentials);

        return Result.Success();
    }
}