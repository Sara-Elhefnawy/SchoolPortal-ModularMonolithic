using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Auth.Command.Login;

public sealed record LoginCommand(string Email, string Password) : IRequest<Result>;