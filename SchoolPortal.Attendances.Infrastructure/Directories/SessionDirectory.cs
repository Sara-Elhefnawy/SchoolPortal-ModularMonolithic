using Microsoft.EntityFrameworkCore;
using SchoolPortal.Attendances.Application.Directories;
using SchoolPortal.Attendances.Application.Features.Sessions.Query.GetByCourseSection;
using SchoolPortal.Attendances.Application.Features.Sessions.Query.GetToday;
using SchoolPortal.Attendances.Domain.Sessions;
using SchoolPortal.Attendances.Infrastructure.Persistence;

namespace SchoolPortal.Attendances.Infrastructure.Directories;

public sealed class SessionDirectory(AttendancesDbContext dbContext) : ISessionDirectory
{
    public async Task<IReadOnlyList<GetTodaysSessionsQueryDto>> GetForDateAsync(DateOnly date, CancellationToken ct)
    {
        // Pulled into a local: EF can't translate date.DayOfWeek inside the query
        // against a column stored as text, but a captured value becomes a plain parameter.
        var day = date.DayOfWeek;

        return await (
            from s in dbContext.Sessions.AsNoTracking()
            where s.DayOfWeek == day
            // Filter the occurrences to today BEFORE the join, so the left join
            // yields "today's occurrence or null", not every occurrence ever.
            join o in dbContext.SessionOccurrences.AsNoTracking().Where(x => x.Date == date) on s.Id equals o.SessionId
                into todays
            from o in todays.DefaultIfEmpty()
            orderby s.StartTime
            select new GetTodaysSessionsQueryDto(s.Id, s.CourseSectionId, s.StartTime, s.EndTime,
                o != null ? o.Id : (int?)null)).ToListAsync(ct);
    }

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