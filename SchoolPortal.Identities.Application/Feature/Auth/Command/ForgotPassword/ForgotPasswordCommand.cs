using MediatR;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Auth.Command.ForgotPassword;

public sealed record ForgotPasswordCommand(string Email) : IRequest<Result>;