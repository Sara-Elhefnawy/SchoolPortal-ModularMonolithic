using MediatR;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Users.Command.AssignRole;

public sealed record AssignRoleCommand(int Id, string RoleName) : IRequest<Result>;