using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Grades.Application.Features.AcademicYears.Command.MarkAsCurrent;

public sealed record MarkAcademicYearAsCurrentCommand(int AcademicYearId) : IRequest<Result>;