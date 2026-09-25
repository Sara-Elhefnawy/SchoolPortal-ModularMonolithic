using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.Identities.Domain.Users;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Users.Query.GetById;

public sealed record GetUserByIdQuery(int Id) : IRequest<Result<ApplicationUser>>;