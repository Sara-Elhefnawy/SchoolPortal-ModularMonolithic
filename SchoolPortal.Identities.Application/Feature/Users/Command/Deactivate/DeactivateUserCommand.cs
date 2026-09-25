using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Users.Command.Deactivate;

public sealed record DeactivateUserCommand(int Id) : IRequest<Result>;