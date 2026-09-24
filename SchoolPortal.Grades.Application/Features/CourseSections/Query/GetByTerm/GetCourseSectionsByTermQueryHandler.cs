using MediatR;
using SchoolPortal.Grades.Application.Directories;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Grades.Application.Features.CourseSections.Query.GetByTerm;

public sealed class GetCourseSectionsByTermQueryHandler(ICourseSectionDirectory directory)
    : IRequestHandler<GetCourseSectionsByTermQuery, Result<IReadOnlyList<GetCourseSectionsByTermQueryDto>>>
{
    public async Task<Result<IReadOnlyList<GetCourseSectionsByTermQueryDto>>> Handle(
        GetCourseSectionsByTermQuery request,
        CancellationToken ct)
    {
        // Join Course and Term to build a friendly display name
        // (the Razor dropdown uses `textField: "name"`).
        var sections = await directory.GetByTermIdAsync(request.TermId, ct);

        return Result<IReadOnlyList<GetCourseSectionsByTermQueryDto>>.Success(sections);
    }
}