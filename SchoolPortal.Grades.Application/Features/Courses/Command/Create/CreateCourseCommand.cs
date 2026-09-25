using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Grades.Application.Features.Courses.Command.Create;

public sealed record CreateCourseCommand(string Name, string Code, decimal CreditHours) : IRequest<Result<int>>;