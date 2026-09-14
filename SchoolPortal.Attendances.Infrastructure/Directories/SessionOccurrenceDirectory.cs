using Microsoft.EntityFrameworkCore;
using SchoolPortal.Attendances.Application.Directories;
using SchoolPortal.Attendances.Domain.SessionOccurrences;
using SchoolPortal.Attendances.Infrastructure.Persistence;

namespace SchoolPortal.Attendances.Infrastructure.Directories;

public sealed class SessionOccurrenceDirectory(AttendancesDbContext dbContext) : ISessionOccurrenceDirectory
{
    public async Task<bool> ExistsByIdAsync(int sessionOccurrenceId, CancellationToken ct) =>
        await dbContext.SessionOccurrences.AnyAsync(s => s.Id == sessionOccurrenceId, ct);

    public async Task<bool> ExistsForDateAsync(int sessionId, DateOnly date, CancellationToken ct) =>
        await dbContext.SessionOccurrences.AnyAsync(s => s.SessionId == sessionId && s.Date == date, ct);

    public async Task AddAsync(SessionOccurrence occurrence, CancellationToken ct)
    {
        dbContext.SessionOccurrences.Add(occurrence);
        await dbContext.SaveChangesAsync(ct);
    }
}