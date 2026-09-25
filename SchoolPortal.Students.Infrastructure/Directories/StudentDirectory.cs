using SchoolPortal.Students.Application.Dtos;
using Microsoft.EntityFrameworkCore;
using SchoolPortal.Students.Application.Directories;
using SchoolPortal.Students.Application.Features.Students.Query.GetAll;
using SchoolPortal.Students.Domain.Students;
using SchoolPortal.Students.Infrastructure.Persistence;

namespace SchoolPortal.Students.Infrastructure.Directories;

internal sealed class StudentDirectory(StudentsDbContext dbContext) : IStudentDirectory
{
    public async Task<IReadOnlyList<GetAllStudentsQueryDto>> GetAllAsync(CancellationToken ct) =>
        await dbContext.Students.AsNoTracking().OrderBy(s => s.Name).Select(s =>
                new GetAllStudentsQueryDto(s.Id, s.Name, s.Email, s.PhoneNumber, s.DateOfBirth, s.Gender,
                    s.AdmissionDate))
            .ToListAsync(ct);

    public async Task<Student?> GetByIdAsync(int id, CancellationToken ct) =>
        await dbContext.Students.FirstOrDefaultAsync(a => a.Id == id, ct);

    public async Task<bool> ExistsAsync(int id, CancellationToken ct) =>
        await dbContext.Students.AnyAsync(a => a.Id == id, ct);

    public async Task<bool> EmailExistsAsync(string email, CancellationToken ct) =>
        await dbContext.Students.AnyAsync(a => a.Email == email, ct);

    public void Add(Student student) => dbContext.Students.Add(student);
    
    

    public async Task<StudentSummaryDto?> GetSummaryAsync(int studentId, CancellationToken ct) =>
        await dbContext.Students.Where(s => s.Id == studentId).Select(s => new StudentSummaryDto(s.Id, s.Name, s.Email))
            .FirstOrDefaultAsync(ct);
}