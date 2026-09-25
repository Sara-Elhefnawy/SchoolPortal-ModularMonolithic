using SchoolPortal.Attendances.Application.Features.Sessions.Query.GetByCourseSection;
using SchoolPortal.Attendances.Domain.Sessions;

namespace SchoolPortal.Attendances.Application.Directories;

public interface ISessionDirectory
{
    Task<IReadOnlyList<GetSessionsByCourseSectionQueryDto>> GetByCourseSectionAsync(int courseSectionId, CancellationToken ct);
    Task<bool> ExistsAsync(int sessionId, CancellationToken ct);
    Task<bool> ConflictExistsAsync(int courseSectionId, DayOfWeek dayOfWeek, TimeOnly startTime, CancellationToken ct);
    void Add(Session session);
}