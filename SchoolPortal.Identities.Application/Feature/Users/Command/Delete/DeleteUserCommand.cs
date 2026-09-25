using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Users.Command.Delete;

public sealed record DeleteUserCommand(int Id) : IRequest<Result>;