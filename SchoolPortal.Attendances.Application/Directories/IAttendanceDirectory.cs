using SchoolPortal.Attendances.Application.Features.Attendances.Query.GetStudentAttendanceHistory;
using SchoolPortal.Attendances.Domain.Attendances;

namespace SchoolPortal.Attendances.Application.Directories;

public interface IAttendanceDirectory
{
    Task<Attendance?> FindStudentAndSessionOccurrenceAsync(int studentId, int sessionOccurrenceId, CancellationToken ct);
    Task<Attendance?> GetByIdAsync(int id, CancellationToken ct);
    void Add(Attendance attendance);
    Task<IReadOnlyList<Attendance>> GetForSessionAsync(int sessionOccurrenceId, CancellationToken ct);
    Task<IReadOnlyList<Attendance>> GetForStudentAsync(int studentId, CancellationToken ct);
    
    // Needs SessionOccurrence.Date, which Attendance doesn't have — so this is its own
    // method rather than a Where/Join a handler builds against an exposed IQueryable.
    Task<IReadOnlyList<GetStudentAttendanceHistoryDto>> GetHistoryForStudentAsync(int studentId, CancellationToken ct);
}