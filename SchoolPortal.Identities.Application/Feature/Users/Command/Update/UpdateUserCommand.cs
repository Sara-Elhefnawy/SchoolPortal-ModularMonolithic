using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Users.Command.Update;

public sealed record UpdateUserCommand(int Id, string Email, string? Name, string? PhoneNumber) : IRequest<Result>;