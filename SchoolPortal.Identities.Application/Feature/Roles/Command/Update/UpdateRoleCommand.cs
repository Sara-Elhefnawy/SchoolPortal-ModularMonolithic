using MediatR;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Roles.Command.Update;

public sealed record UpdateRoleCommand(int Id, string Name) : IRequest<Result>;