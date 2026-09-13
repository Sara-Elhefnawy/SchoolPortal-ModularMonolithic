using Microsoft.EntityFrameworkCore;
using SchoolPortal.Attendances.Domain.Attendances;
using SchoolPortal.Attendances.Domain.Sessions;
using SchoolPortal.Attendances.Domain.SessionOccurrences;

namespace SchoolPortal.Attendances.Application.DbContexts;

public interface IAttendancesDbContext
{
    DbSet<Session> Sessions { get; }
    DbSet<SessionOccurrence> SessionOccurrences { get; }
    DbSet<Attendance> Attendances { get; }

    Task<int> SaveChangesAsync(CancellationToken ct);
}