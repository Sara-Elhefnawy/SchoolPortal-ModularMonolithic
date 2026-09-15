using SchoolPortal.Grades.Application.Persistence;

namespace SchoolPortal.Grades.Infrastructure.Persistence;

internal sealed class GradesUnitOfWork(GradesDbContext dbContext) : IGradesUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken ct) => dbContext.SaveChangesAsync(ct);
}