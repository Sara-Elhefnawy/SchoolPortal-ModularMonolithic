using MediatR;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Roles.Command.Create;

public sealed record CreateRoleCommand(string Name) : IRequest<Result>;