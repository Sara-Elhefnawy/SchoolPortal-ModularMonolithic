using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.Identities.Domain.Roles;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Roles.Query.GetById;

public sealed record GetRoleByIdQuery(int Id) : IRequest<Result<ApplicationRole>>;