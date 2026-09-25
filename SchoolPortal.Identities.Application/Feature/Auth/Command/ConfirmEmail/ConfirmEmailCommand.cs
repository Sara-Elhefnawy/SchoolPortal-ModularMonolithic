using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Auth.Command.ConfirmEmail;

public sealed record ConfirmEmailCommand(int UserId, string Token) : IRequest<Result>;