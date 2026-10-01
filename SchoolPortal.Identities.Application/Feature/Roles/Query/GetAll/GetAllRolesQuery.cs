using SchoolPortal.Identities.Application.Directories.Dtos;
using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Roles.Query.GetAll;

public record GetAllRolesQuery() : IRequest<Result<IReadOnlyList<RoleDto>>>;