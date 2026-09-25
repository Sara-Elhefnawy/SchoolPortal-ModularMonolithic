using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Grades.Application.Features.Courses.Query.GetAll;

public sealed record GetAllCoursesQuery : IRequest<Result<IReadOnlyList<GetAllCoursesQueryDto>>>;