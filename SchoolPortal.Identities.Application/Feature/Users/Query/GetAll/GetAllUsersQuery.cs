using MediatR;
using SchoolPortal.Identities.Domain.Users;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Users.Query.GetAll;

public sealed record GetAllUsersQuery() : IRequest<Result<IReadOnlyList<ApplicationUser>>>;