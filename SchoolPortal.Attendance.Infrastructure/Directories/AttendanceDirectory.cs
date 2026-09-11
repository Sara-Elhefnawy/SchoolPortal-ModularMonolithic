using Microsoft.EntityFrameworkCore;
using SchoolPortal.Attendance.Application.Abstractions;
using SchoolPortal.Attendance.Domain.Attendances;

namespace SchoolPortal.Attendance.Infrastructure.Directories;

internal sealed class AttendanceDirectory(AttendanceDbContext dbContext) : IAttendanceDirectory
{
    public async Task<AttendanceRecord?> FindAsync(int studentId, int sessionOccurrenceId, CancellationToken ct) =>
        await dbContext.AttendanceRecords.FirstOrDefaultAsync(
            a => a.StudentId == studentId && a.SessionOccurrenceId == sessionOccurrenceId, ct);

    public async Task AddAsync(AttendanceRecord attendance, CancellationToken ct)
    {
        dbContext.Add(attendance);
        await dbContext.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(AttendanceRecord record, CancellationToken ct) =>
        await dbContext.SaveChangesAsync(ct); // already tracked

    public async Task<IReadOnlyList<AttendanceRecord>>
        GetForSessionAsync(int sessionOccurrenceId, CancellationToken ct) =>
        await dbContext.AttendanceRecords.Where(a => a.SessionOccurrenceId == sessionOccurrenceId).ToListAsync(ct);

    public async Task<IReadOnlyList<AttendanceRecord>> GetForStudentAsync(int studentId, CancellationToken ct) =>
        await dbContext.AttendanceRecords.Where(a => a.StudentId == studentId).ToListAsync(ct);
}