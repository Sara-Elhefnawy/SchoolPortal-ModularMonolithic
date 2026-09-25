using SchoolPortal.Attendances.Application.Directories;
using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Attendances.Application.Features.Sessions.Query.GetByCourseSection;

public sealed class GetSessionsByCourseSectionQueryHandler(ISessionDirectory sessions)
    : IRequestHandler<GetSessionsByCourseSectionQuery, Result<IReadOnlyList<GetSessionsByCourseSectionQueryDto>>>
{
    public async Task<Result<IReadOnlyList<GetSessionsByCourseSectionQueryDto>>> Handle(
        GetSessionsByCourseSectionQuery request,
        CancellationToken ct = default)
    {
        var list = await sessions.GetByCourseSectionAsync(request.CourseSectionId, ct);

        return Result<IReadOnlyList<GetSessionsByCourseSectionQueryDto>>.Success(list);
    }
}