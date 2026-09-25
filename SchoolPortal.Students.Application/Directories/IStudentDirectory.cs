using SchoolPortal.Students.Application.Dtos;
using SchoolPortal.Students.Application.Features.Students.Query.GetAll;
using SchoolPortal.Students.Domain.Students;

namespace SchoolPortal.Students.Application.Directories;

public interface IStudentDirectory
{
    Task<IReadOnlyList<GetAllStudentsQueryDto>> GetAllAsync(CancellationToken ct);
    Task<Student?> GetByIdAsync(int studentId, CancellationToken ct);
    Task<bool> ExistsAsync(int studentId, CancellationToken ct);
    Task<bool> EmailExistsAsync(string email, CancellationToken ct);
    void Add(Student student);
    Task<StudentSummaryDto?> GetSummaryAsync(int studentId, CancellationToken ct);
}