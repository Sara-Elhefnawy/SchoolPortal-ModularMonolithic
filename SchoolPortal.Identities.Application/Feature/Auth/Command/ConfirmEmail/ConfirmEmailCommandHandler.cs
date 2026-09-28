using SchoolPortal.SharedKernel.Mediator;
using Microsoft.AspNetCore.Identity;
using SchoolPortal.Identities.Domain.Users;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Auth.Command.ConfirmEmail;

public sealed class ConfirmEmailCommandHandler(
    UserManager<ApplicationUser> userManager)
    : IRequestHandler<ConfirmEmailCommand, Result>
{
    public async Task<Result> Handle(ConfirmEmailCommand request, CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());
        if (user is null)
            return Result.Failure(UserErrors.NotFound);

        if (user.EmailConfirmed)
            return Result.Success(); // idempotent

        var result = await userManager.ConfirmEmailAsync(user, request.Token);
        if (!result.Succeeded)
            return Result.Failure(UserErrors.EmailConfirmationFailed);
        
        return Result.Success();
    }
}