using Microsoft.EntityFrameworkCore;
using SchoolPortal.Attendances.Application.Directories;
using SchoolPortal.Attendances.Domain.Attendances;
using SchoolPortal.Attendances.Infrastructure.Persistence;

namespace SchoolPortal.Attendances.Infrastructure.Directories;

internal sealed class AttendanceDirectory(AttendancesDbContext dbContext) : IAttendanceDirectory
{
    public async Task<Attendance?> FindAsync(int studentId, int sessionOccurrenceId, CancellationToken ct) =>
        await dbContext.Attendances.FirstOrDefaultAsync(
            a => a.StudentId == studentId && a.SessionOccurrenceId == sessionOccurrenceId, ct);

    public async Task AddAsync(Attendance attendance, CancellationToken ct)
    {
        dbContext.Add(attendance);
        await dbContext.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Attendance record, CancellationToken ct) =>
        await dbContext.SaveChangesAsync(ct); // already tracked

    public async Task<IReadOnlyList<Attendance>>
        GetForSessionAsync(int sessionOccurrenceId, CancellationToken ct) =>
        await dbContext.Attendances.Where(a => a.SessionOccurrenceId == sessionOccurrenceId).ToListAsync(ct);

    public async Task<IReadOnlyList<Attendance>> GetForStudentAsync(int studentId, CancellationToken ct) =>
        await dbContext.Attendances.Where(a => a.StudentId == studentId).ToListAsync(ct);
}