using MediatR;
using SchoolPortal.Grades.Application.Directories;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Grades.Application.Features.Courses.Query.GetAll;

public sealed class GetAllCoursesQueryHandler(ICourseDirectory directory)
    : IRequestHandler<GetAllCoursesQuery, Result<IReadOnlyList<GetAllCoursesQueryDto>>>
{
    public async Task<Result<IReadOnlyList<GetAllCoursesQueryDto>>> Handle(
        GetAllCoursesQuery request,
        CancellationToken ct)
    {
        var courses = await directory.GetAllAsync(ct);

        return Result<IReadOnlyList<GetAllCoursesQueryDto>>.Success(courses);
    }
}