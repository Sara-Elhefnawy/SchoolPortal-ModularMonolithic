using MediatR;
using SchoolPortal.Attendance.Application.Abstractions;
using SchoolPortal.Attendance.Domain.Attendances;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Attendance.Application.Features.Query.GetAttendanceSummary;

public sealed class GetAttendanceSummaryQueryHandler(IAttendanceDirectory directory)
    : IRequestHandler<GetAttendanceSummaryQuery, Result<GetAttendanceSummaryDto>>
{
    public async Task<Result<GetAttendanceSummaryDto>> Handle(GetAttendanceSummaryQuery query, CancellationToken ct)
    {
        var records = await directory.GetForStudentAsync(query.StudentId, ct);

        // Small in-memory aggregation — fine at this scale (one student's
        // records), no need for a database-side aggregate query here.
        var present = records.Count(r => r.Status == AttendanceStatus.Present);
        var absent = records.Count(r => r.Status == AttendanceStatus.Absent);
        var late = records.Count(r => r.Status == AttendanceStatus.Late);
        var rate = records.Count == 0 ? 0 : (decimal)present / records.Count;

        return Result<GetAttendanceSummaryDto>.Success(new GetAttendanceSummaryDto(present, absent, late, rate));
    }
}