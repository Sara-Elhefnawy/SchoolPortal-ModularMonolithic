using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Students.Application.Features.Students.Query.GetAll;

public sealed record GetAllStudentsQuery : IRequest<Result<IReadOnlyList<GetAllStudentsQueryDto>>>;