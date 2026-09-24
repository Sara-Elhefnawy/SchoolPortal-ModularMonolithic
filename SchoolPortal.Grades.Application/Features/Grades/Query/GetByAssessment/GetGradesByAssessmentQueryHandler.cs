using MediatR;
using SchoolPortal.Grades.Application.Directories;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Grades.Application.Features.Grades.Query.GetByAssessment;

public sealed class GetGradesByAssessmentQueryHandler(IGradeDirectory directory)
    : IRequestHandler<GetGradesByAssessmentQuery, Result<IReadOnlyList<GetGradesByAssessmentQueryDto>>>
{
    public async Task<Result<IReadOnlyList<GetGradesByAssessmentQueryDto>>> Handle(
        GetGradesByAssessmentQuery request,
        CancellationToken ct)
    {
        // Students live in a different module/db context, so the handler joins
        // through a read-model or calls the Students query. Adjust to your
        // actual cross-module read strategy.
        var grades = await directory.GetByAssessmentIdAsync(request.AssessmentId, ct);

        return Result<IReadOnlyList<GetGradesByAssessmentQueryDto>>.Success(grades);
    }
}