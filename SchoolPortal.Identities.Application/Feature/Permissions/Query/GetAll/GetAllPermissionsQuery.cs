using MediatR;
using SchoolPortal.Identities.Domain.Permissions;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Permissions.Query.GetAll;

public sealed record GetAllPermissionsQuery() : IRequest<Result<IReadOnlyList<Permission>>>;