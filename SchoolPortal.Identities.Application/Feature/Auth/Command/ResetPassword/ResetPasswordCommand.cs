using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Auth.Command.ResetPassword;

public sealed record ResetPasswordCommand(
    string Email,
    string Token,
    string NewPassword,
    string ConfirmPassword) : IRequest<Result>;