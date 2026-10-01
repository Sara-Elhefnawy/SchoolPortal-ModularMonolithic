using SchoolPortal.Grades.Application.Directories;
using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Grades.Application.Features.Grades.Query.GetForStudent;

internal sealed class GetGradesForStudentQueryHandler(
    IGradeDirectory directory)
    : IRequestHandler<GetGradesForStudentQuery, Result<IReadOnlyList<GetGradesForStudentQueryDto>>>
{
    public async Task<Result<IReadOnlyList<GetGradesForStudentQueryDto>>> Handle(
        GetGradesForStudentQuery request,
        CancellationToken ct)
    {
        var rows = await directory.GetForStudentAsync(request.StudentId, ct);

        return Result<IReadOnlyList<GetGradesForStudentQueryDto>>.Success(rows);
    }
}