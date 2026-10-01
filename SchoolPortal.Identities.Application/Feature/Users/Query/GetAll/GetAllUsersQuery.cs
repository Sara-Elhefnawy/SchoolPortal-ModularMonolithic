using SchoolPortal.Identities.Application.Directories.Dtos;
using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Users.Query.GetAll;

public sealed record GetAllUsersQuery() : IRequest<Result<IReadOnlyList<UserDto>>>;