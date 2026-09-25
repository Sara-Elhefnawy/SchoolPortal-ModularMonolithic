using SchoolPortal.SharedKernel.Mediator;
using Microsoft.AspNetCore.Identity;
using SchoolPortal.Identities.Domain.Users;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Auth.Command.Logout;

public sealed class LogoutCommandHandler(SignInManager<ApplicationUser> signInManager) : IRequestHandler<LogoutCommand, Result>
{
    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        // Identity removes the authentication cookie from the response.
        // Can't fail and Result wrapper stays only for consistency
        await signInManager.SignOutAsync();
        return Result.Success();
    }
}