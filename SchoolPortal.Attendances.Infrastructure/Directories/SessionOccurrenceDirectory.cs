using Microsoft.EntityFrameworkCore;
using SchoolPortal.Attendances.Application.Directories;
using SchoolPortal.Attendances.Application.Features.SessionOccurrences.Query.GetBySession;
using SchoolPortal.Attendances.Domain.SessionOccurrences;
using SchoolPortal.Attendances.Infrastructure.Persistence;

namespace SchoolPortal.Attendances.Infrastructure.Directories;

public sealed class SessionOccurrenceDirectory(AttendancesDbContext dbContext) : ISessionOccurrenceDirectory
{
    public async Task<SessionOccurrence?> GetByIdAsync(int id, CancellationToken ct) =>
        await dbContext.SessionOccurrences.FirstOrDefaultAsync(a => a.Id == id, ct);

    public async Task<IReadOnlyList<GetOccurrencesBySessionQueryDto>>
        GetOccurrencesAsync(int sessionId, CancellationToken ct) =>
        await dbContext.SessionOccurrences.AsNoTracking().Where(o => o.SessionId == sessionId)
            .OrderByDescending(o => o.Date).Select(o => new GetOccurrencesBySessionQueryDto(o.Id, o.SessionId, o.Date))
            .ToListAsync(ct);

    public async Task<bool> ExistsByIdAsync(int sessionOccurrenceId, CancellationToken ct) =>
        await dbContext.SessionOccurrences.AnyAsync(s => s.Id == sessionOccurrenceId, ct);

    public async Task<bool> ExistsForDateAsync(int sessionId, DateOnly date, CancellationToken ct) =>
        await dbContext.SessionOccurrences.AnyAsync(s => s.SessionId == sessionId && s.Date == date, ct);

    public void Add(SessionOccurrence occurrence) => dbContext.SessionOccurrences.Add(occurrence);
}