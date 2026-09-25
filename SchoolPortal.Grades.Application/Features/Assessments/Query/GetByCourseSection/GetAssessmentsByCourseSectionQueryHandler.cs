using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.Grades.Application.Directories;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Grades.Application.Features.Assessments.Query.GetByCourseSection;

public sealed class GetAssessmentsByCourseSectionQueryHandler (IAssessmentDirectory directory)
    : IRequestHandler<GetAssessmentsBySectionQuery, Result<IReadOnlyList<GetAssessmentsByCourseSectionQueryDto>>>
{
    public async Task<Result<IReadOnlyList<GetAssessmentsByCourseSectionQueryDto>>> Handle(
        GetAssessmentsBySectionQuery request,
        CancellationToken ct)
    {
        var items = await directory.GetBySectionAsync(request.CourseSectionId, ct);

        return Result<IReadOnlyList<GetAssessmentsByCourseSectionQueryDto>>.Success(items);
    }
}