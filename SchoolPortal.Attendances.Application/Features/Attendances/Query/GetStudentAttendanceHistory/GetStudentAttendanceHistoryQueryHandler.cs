using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.Attendances.Application.Directories;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Attendances.Application.Features.Attendances.Query.GetStudentAttendanceHistory;

public sealed class GetStudentAttendanceHistoryQueryHandler(IAttendanceDirectory directory)
    : IRequestHandler<GetStudentAttendanceHistoryQuery, Result<IReadOnlyList<GetStudentAttendanceHistoryDto>>>
{
    public async Task<Result<IReadOnlyList<GetStudentAttendanceHistoryDto>>> Handle(
        GetStudentAttendanceHistoryQuery request, CancellationToken ct)
    {
        // GetHistoryForStudentAsync does the SessionOccurrence join inside the directory
        // implementation instead, where the DbContext actually lives.
        var rows = await directory.GetHistoryForStudentAsync(request.StudentId, ct);

        var orderedRows = rows
            .OrderByDescending(r => r.Date)
            .ToList();

        return Result<IReadOnlyList<GetStudentAttendanceHistoryDto>>.Success(orderedRows);
    }
}