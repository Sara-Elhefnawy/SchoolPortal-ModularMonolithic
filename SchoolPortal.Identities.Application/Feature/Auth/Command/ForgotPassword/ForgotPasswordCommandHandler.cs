using SchoolPortal.SharedKernel.Mediator;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SchoolPortal.Identities.Domain.Users;
using SchoolPortal.SharedKernel.Emails.Contracts;
using SchoolPortal.SharedKernel.Emails.Options;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Auth.Command.ForgotPassword;

public sealed class ForgotPasswordCommandHandler(
    UserManager<ApplicationUser>  userManager,
    IEmailSender emailSender,
    IOptions<AppOptions> appOptions,
    ILogger<ForgotPasswordCommandHandler> logger)
    : IRequestHandler<ForgotPasswordCommand, Result>
{
    public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null || user.IsDeleted || !user.IsActive)
            return Result.Success();
        
        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var link = $"{appOptions.Value.PublicBaseUrl}/reset-password?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(request.Email)}";
        
        var sendResult = await emailSender.SendResetPasswordEmailAsync(user.Email!, link, ct);
        if (sendResult.IsFailure)
            logger.LogWarning("Password reset email failed to send to {Email}", request.Email);

        return Result.Success();
    }
}