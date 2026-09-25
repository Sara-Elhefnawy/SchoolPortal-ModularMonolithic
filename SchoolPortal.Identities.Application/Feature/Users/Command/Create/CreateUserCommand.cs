using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Users.Command.Create;

public sealed record CreateUserCommand(
    string Email,
    string Password,
    string Name,
    string RoleName) : IRequest<Result<int>>;