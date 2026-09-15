using Microsoft.EntityFrameworkCore;
using SchoolPortal.Attendances.Application.Directories;
using SchoolPortal.Attendances.Application.Features.Attendances.Query.GetStudentAttendanceHistory;
using SchoolPortal.Attendances.Domain.Attendances;
using SchoolPortal.Attendances.Infrastructure.Persistence;

namespace SchoolPortal.Attendances.Infrastructure.Directories;

internal sealed class AttendanceDirectory(AttendancesDbContext dbContext) : IAttendanceDirectory
{
    public async Task<Attendance?> FindStudentAndSessionOccurrenceAsync(int studentId, int sessionOccurrenceId, CancellationToken ct) =>
        await dbContext.Attendances.FirstOrDefaultAsync(
            a => a.StudentId == studentId && a.SessionOccurrenceId == sessionOccurrenceId, ct);

    public async Task<Attendance?> GetByIdAsync(int id, CancellationToken ct)
        => await dbContext.Attendances.FirstOrDefaultAsync(
            a => a.Id == id,
            ct);

    public void Add(Attendance attendance) => dbContext.Attendances.Add(attendance);
    
    
    
    
    
    

    public async Task<IReadOnlyList<Attendance>>
        GetForSessionAsync(int sessionOccurrenceId, CancellationToken ct) =>
        await dbContext.Attendances.Where(a => a.SessionOccurrenceId == sessionOccurrenceId).ToListAsync(ct);

    public async Task<IReadOnlyList<Attendance>> GetForStudentAsync(int studentId, CancellationToken ct) =>
        await dbContext.Attendances.Where(a => a.StudentId == studentId).ToListAsync(ct);

    public async Task<IReadOnlyList<GetStudentAttendanceHistoryDto>> GetHistoryForStudentAsync(
        int studentId,
        CancellationToken ct)
        => await (
            from attendance in dbContext.Attendances.AsNoTracking()
            join occurrence in dbContext.SessionOccurrences.AsNoTracking()
                on attendance.SessionOccurrenceId equals occurrence.Id
            where attendance.StudentId == studentId
            select new GetStudentAttendanceHistoryDto(
                attendance.SessionOccurrenceId,
                occurrence.Date,
                attendance.Status,
                attendance.Note)
        ).ToListAsync(ct);
}