using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Grades.Application.Features.Terms.Query.GetByAcademicYear;

public sealed record GetTermsByAcademicYearQuery(int AcademicYearId) : IRequest<Result<IReadOnlyList<GetTermsByAcademicYearQueryDto>>>;