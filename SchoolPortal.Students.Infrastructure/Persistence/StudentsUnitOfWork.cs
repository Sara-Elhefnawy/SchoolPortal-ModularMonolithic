using SchoolPortal.Students.Application.Persistence;

namespace SchoolPortal.Students.Infrastructure.Persistence;

internal sealed class StudentsUnitOfWork(StudentsDbContext dbContext) : IStudentsUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken ct) => dbContext.SaveChangesAsync(ct);
}