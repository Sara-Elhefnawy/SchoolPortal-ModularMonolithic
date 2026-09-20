using MediatR;
using Microsoft.AspNetCore.Identity;
using SchoolPortal.Identities.Domain.Users;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Auth.Command.ForgotPassword;

public sealed class ForgotPasswordCommandHandler(
    UserManager<ApplicationUser>  userManager)
    : IRequestHandler<ForgotPasswordCommand, Result>
{
    public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null || user.IsDeleted || !user.IsActive)
            return Result.Success();
        
        var token = await userManager.GeneratePasswordResetTokenAsync(user);

        // talk to email service to confirm email.

        return Result.Success();
    }
}