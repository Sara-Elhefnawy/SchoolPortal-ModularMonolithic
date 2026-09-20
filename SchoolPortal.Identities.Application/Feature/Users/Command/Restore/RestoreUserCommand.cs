using MediatR;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Users.Command.Restore;

public sealed record RestoreUserCommand(int Id) : IRequest<Result>;