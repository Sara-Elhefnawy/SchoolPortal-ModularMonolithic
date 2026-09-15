using Microsoft.EntityFrameworkCore;
using SchoolPortal.Attendances.Application.Directories;
using SchoolPortal.Attendances.Domain.Sessions;
using SchoolPortal.Attendances.Infrastructure.Persistence;

namespace SchoolPortal.Attendances.Infrastructure.Directories;

public sealed class SessionDirectory(AttendancesDbContext dbContext) : ISessionDirectory
{
    public async Task<bool> ExistsAsync(int sessionId, CancellationToken ct) =>
        await dbContext.Sessions.AnyAsync(s => s.Id == sessionId, ct);

    public async Task<bool> ConflictExistsAsync(int courseSectionId, DayOfWeek dayOfWeek, TimeOnly startTime,
        CancellationToken ct) =>
        await dbContext.Sessions.AnyAsync(
            s => s.CourseSectionId == courseSectionId && s.DayOfWeek == dayOfWeek && s.StartTime >= startTime, ct);

    public void Add(Session session) => dbContext.Sessions.Add(session);
}