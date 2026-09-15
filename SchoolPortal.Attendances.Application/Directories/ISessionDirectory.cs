using SchoolPortal.Attendances.Domain.Sessions;

namespace SchoolPortal.Attendances.Application.Directories;

public interface ISessionDirectory
{
    Task<bool> ExistsAsync(int sessionId, CancellationToken ct);
    Task<bool> ConflictExistsAsync(int courseSectionId, DayOfWeek dayOfWeek, TimeOnly startTime, CancellationToken ct);
    void Add(Session session);
}