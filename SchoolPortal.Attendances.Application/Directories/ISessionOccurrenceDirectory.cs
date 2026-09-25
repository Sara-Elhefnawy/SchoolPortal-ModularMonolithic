using SchoolPortal.Attendances.Application.Features.SessionOccurrences.Query.GetBySession;
using SchoolPortal.Attendances.Domain.SessionOccurrences;

namespace SchoolPortal.Attendances.Application.Directories;

public interface ISessionOccurrenceDirectory
{
    Task<SessionOccurrence?> GetByIdAsync(int sessionOccurrenceId, CancellationToken ct);
    Task<IReadOnlyList<GetOccurrencesBySessionQueryDto>> GetOccurrencesAsync(int sessionId, CancellationToken ct);
    Task<bool> ExistsByIdAsync(int sessionOccurrenceId, CancellationToken ct);
    Task<bool> ExistsForDateAsync(int sessionId, DateOnly date, CancellationToken ct);
    void Add(SessionOccurrence occurrence);
}