using SchoolPortal.Attendances.Application.Persistence;

namespace SchoolPortal.Attendances.Infrastructure.Persistence;

internal sealed class AttendancesUnitOfWork(AttendancesDbContext dbContext) : IAttendancesUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken ct) => dbContext.SaveChangesAsync(ct);
}