using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Grades.Application.Features.Grades.Query.GetForStudent;

public sealed record GetGradesForStudentQuery(int StudentId)
    : IRequest<Result<IReadOnlyList<GetGradesForStudentQueryDto>>>;