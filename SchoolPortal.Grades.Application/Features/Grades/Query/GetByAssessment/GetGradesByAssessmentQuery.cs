using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Grades.Application.Features.Grades.Query.GetByAssessment;

public sealed record GetGradesByAssessmentQuery(int AssessmentId)
    : IRequest<Result<IReadOnlyList<GetGradesByAssessmentQueryDto>>>;