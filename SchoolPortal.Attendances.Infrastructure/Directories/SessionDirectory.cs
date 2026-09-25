using Microsoft.EntityFrameworkCore;
using SchoolPortal.Attendances.Application.Directories;
using SchoolPortal.Attendances.Application.Features.Sessions.Query.GetByCourseSection;
using SchoolPortal.Attendances.Domain.Sessions;
using SchoolPortal.Attendances.Infrastructure.Persistence;

namespace SchoolPortal.Attendances.Infrastructure.Directories;

public sealed class SessionDirectory(AttendancesDbContext dbContext) : ISessionDirectory
{
    public async Task<IReadOnlyList<GetSessionsByCourseSectionQueryDto>>
        GetByCourseSectionAsync(int courseSectionId, CancellationToken ct) =>
        await dbContext.Sessions.AsNoTracking().Where(s => s.CourseSectionId == courseSectionId)
            .OrderBy(s => s.DayOfWeek).ThenBy(s => s.StartTime).Select(s =>
                new GetSessionsByCourseSectionQueryDto(s.Id, s.CourseSectionId, s.DayOfWeek, s.StartTime, s.EndTime))
            .ToListAsync(ct);
    
    public async Task<bool> ExistsAsync(int sessionId, CancellationToken ct) =>
        await dbContext.Sessions.AnyAsync(s => s.Id == sessionId, ct);

    public async Task<bool> ConflictExistsAsync(int courseSectionId, DayOfWeek dayOfWeek, TimeOnly startTime,
        CancellationToken ct) =>
        await dbContext.Sessions.AnyAsync(
            s => s.CourseSectionId == courseSectionId && s.DayOfWeek == dayOfWeek && s.StartTime >= startTime, ct);

    public void Add(Session session) => dbContext.Sessions.Add(session);
}