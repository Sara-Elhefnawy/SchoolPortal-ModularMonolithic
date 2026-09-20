using MediatR;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Roles.Command.Delete;

public sealed record DeleteRoleCommand(int Id) : IRequest<Result>;