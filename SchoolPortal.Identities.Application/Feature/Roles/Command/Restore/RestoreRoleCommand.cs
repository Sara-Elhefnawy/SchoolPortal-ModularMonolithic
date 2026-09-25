using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Roles.Command.Restore;

public sealed record RestoreRoleCommand(int Id) : IRequest<Result>;