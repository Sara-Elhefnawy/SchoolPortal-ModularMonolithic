namespace SchoolPortal.Attendance.Application.Abstractions;
using Domain.Attendances;

public interface IAttendanceDirectory
{
    Task<AttendanceRecord?> FindAsync(int studentId, int sessionOccurrenceId, CancellationToken ct);
    Task AddAsync(AttendanceRecord attendance, CancellationToken ct);
    Task UpdateAsync(AttendanceRecord attendance, CancellationToken ct);
    Task<IReadOnlyList<AttendanceRecord>> GetForSessionAsync(int sessionOccurrenceId, CancellationToken ct);
    Task<IReadOnlyList<AttendanceRecord>> GetForStudentAsync(int studentId, CancellationToken ct);
}