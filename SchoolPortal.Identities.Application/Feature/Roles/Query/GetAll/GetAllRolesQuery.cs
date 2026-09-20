using MediatR;
using SchoolPortal.Identities.Domain.Roles;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Roles.Query.GetAll;

public record GetAllRolesQuery() : IRequest<Result<IReadOnlyList<ApplicationRole>>>;