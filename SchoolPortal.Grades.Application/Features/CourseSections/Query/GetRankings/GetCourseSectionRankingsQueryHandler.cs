using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.Grades.Application.Directories;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Grades.Application.Features.CourseSections.Query.GetRankings;

public sealed class GetCourseSectionRankingsQueryHandler(ICourseSectionDirectory directory)
    : IRequestHandler<GetCourseSectionRankingsQuery, Result<IReadOnlyList<GetCourseSectionRankingsQueryDto>>>
{
    public async Task<Result<IReadOnlyList<GetCourseSectionRankingsQueryDto>>> Handle(
        GetCourseSectionRankingsQuery request,
        CancellationToken ct)
    {
        var rankings = await directory.GetRankingsAsync(request.CourseSectionId, ct);

        return Result<IReadOnlyList<GetCourseSectionRankingsQueryDto>>.Success(rankings);
    }
}