using MediatR;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Auth.Command.ChangePassword;

public sealed record ChangePasswordCommand(string Email, string Password, string NewPassword) : IRequest<Result>;