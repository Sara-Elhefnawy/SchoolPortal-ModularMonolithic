using SchoolPortal.Students.Application.Dtos;
using SchoolPortal.Students.Domain.Students;

namespace SchoolPortal.Students.Application.Directories;

public interface IStudentDirectory
{
    Task<Student?> FindByIdAsync(int studentId, CancellationToken ct);
    Task<bool> ExistsAsync(int studentId, CancellationToken ct);
    Task<bool> EmailExistsAsync(string email, CancellationToken ct);
    Task<StudentSummaryDto?> GetSummaryAsync(int studentId, CancellationToken ct);
    Task AddAsync(Student student, CancellationToken ct);
    Task UpdateAsync(CancellationToken ct);
}