using SchoolPortal.Attendances.Domain.SessionOccurrences;

namespace SchoolPortal.Attendances.Application.Directories;

public interface ISessionOccurrenceDirectory
{
    Task<bool> ExistsByIdAsync(int sessionOccurrenceId, CancellationToken ct);
    Task<bool> ExistsForDateAsync(int sessionId, DateOnly date, CancellationToken ct);
    Task AddAsync(SessionOccurrence occurrence, CancellationToken ct);
}