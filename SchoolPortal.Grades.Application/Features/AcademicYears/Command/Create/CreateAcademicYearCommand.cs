using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Grades.Application.Features.AcademicYears.Command.Create;

public sealed record CreateAcademicYearCommand(string Name, DateOnly StartDate, DateOnly EndDate) : IRequest<Result<int>>;