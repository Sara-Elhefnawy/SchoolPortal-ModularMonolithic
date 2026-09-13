using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolPortal.Attendances.Application.DbContexts;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Attendances.Application.Features.Attendances.Query.GetStudentAttendanceHistory;

public sealed class GetStudentAttendanceHistoryQueryHandler(IAttendancesDbContext db)
    : IRequestHandler<GetStudentAttendanceHistoryQuery, Result<List<AttendanceHistoryRowDto>>>
{
    public async Task<Result<List<AttendanceHistoryRowDto>>> Handle(
        GetStudentAttendanceHistoryQuery request, CancellationToken ct)
    {
        var rows = await db.Attendances
            .Where(a => a.StudentId == request.StudentId)
            .Join(db.SessionOccurrences, a => a.SessionOccurrenceId, o => o.Id,
                (a, o) => new AttendanceHistoryRowDto(o.Id, o.Date, a.Status, a.Note))
            .OrderByDescending(r => r.Date)
            .ToListAsync(ct);

        return Result<List<AttendanceHistoryRowDto>>.Success(rows);
    }
}