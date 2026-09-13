using SchoolPortal.Attendances.Domain.Attendances;

namespace SchoolPortal.Attendances.Application.Directories;

public interface IAttendanceDirectory
{
    Task<Attendance?> FindAsync(int studentId, int sessionOccurrenceId, CancellationToken ct);
    Task AddAsync(Attendance attendance, CancellationToken ct);
    Task UpdateAsync(Attendance attendance, CancellationToken ct);
    Task<IReadOnlyList<Attendance>> GetForSessionAsync(int sessionOccurrenceId, CancellationToken ct);
    Task<IReadOnlyList<Attendance>> GetForStudentAsync(int studentId, CancellationToken ct);
}