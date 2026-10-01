using SchoolPortal.Identities.Application.Directories.Dtos;
using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Permissions.Query.GetAll;

public sealed record GetAllPermissionsQuery() : IRequest<Result<IReadOnlyList<PermissionDto>>>;