using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SchoolPortal.Identities.Domain;
using SchoolPortal.Identities.Domain.Users;
using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Auth.Command.ResetPassword;

public sealed class ResetPasswordCommandHandler(
    UserManager<ApplicationUser> userManager,
    ILogger<ResetPasswordCommandHandler> logger) : IRequestHandler<ResetPasswordCommand, Result>
{
    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken ct)
    {
        if (request.NewPassword != request.ConfirmPassword)
            return Result.Failure(AuthErrors.PasswordsDoNotMatch);

        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null || user.IsDeleted || !user.IsActive)
            return Result.Failure(AuthErrors.InvalidTokenOrEmail);

        var identityResult = await userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);

        if (identityResult.Succeeded)
            return Result.Success();

        // Identity's own error codes (InvalidToken, PasswordTooShort, etc.)
        // are worth having in the logs for debugging, but not worth echoing
        // back to the caller - same "don't leak which check failed" reasoning.
        logger.LogWarning("Password reset failed for {Email}: {Errors}", request.Email,
            string.Join(", ", identityResult.Errors.Select(e => e.Description)));

        return Result.Failure(AuthErrors.InvalidTokenOrEmail);
    }
}