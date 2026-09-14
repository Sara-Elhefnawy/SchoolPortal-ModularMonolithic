using SchoolPortal.Students.Application.Dtos;
using Microsoft.EntityFrameworkCore;
using SchoolPortal.Students.Application.Directories;
using SchoolPortal.Students.Domain.Students;
using SchoolPortal.Students.Infrastructure.Persistence;

namespace SchoolPortal.Students.Infrastructure.Directories;

internal sealed class StudentDirectory(StudentsDbContext dbContext) : IStudentDirectory
{
    public async Task<Student?> FindByIdAsync(int studentId, CancellationToken ct) =>
        await dbContext.Students.FirstOrDefaultAsync(a => a.Id == studentId, ct);

    public async Task<bool> ExistsAsync(int studentId, CancellationToken ct) =>
        await dbContext.Students.AnyAsync(a => a.Id == studentId, ct);

    public async Task<bool> EmailExistsAsync(string email, CancellationToken ct) =>
        await dbContext.Students.AnyAsync(a => a.Email == email, ct);

    public async Task<StudentSummaryDto?> GetSummaryAsync(int studentId, CancellationToken ct) =>
        await dbContext.Students.Where(s => s.Id == studentId).Select(s => new StudentSummaryDto(s.Id, s.Name, s.Email))
            .FirstOrDefaultAsync(ct);

    public async Task AddAsync(Student student, CancellationToken ct)
    {
        dbContext.Students.Add(student);
        await dbContext.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(CancellationToken ct) => await dbContext.SaveChangesAsync(ct); // already tracked
}