using SchoolPortal.Students.Application.Features.Students.Query.GetAll;
using SchoolPortal.Students.Domain.Students;

namespace SchoolPortal.Students.Application.Directories;

public interface IStudentDirectory
{
    Task<IReadOnlyList<GetAllStudentsQueryDto>> GetAllAsync(string? search = null, StudentStatus? status = null,
        CancellationToken ct = default);
    Task<Student?> GetByIdAsync(int id, CancellationToken ct);
    Task<IReadOnlyDictionary<int, string>> GetNamesAsync(IReadOnlyCollection<int> ids, CancellationToken ct);
    Task<bool> ExistsAsync(int id, CancellationToken ct);
    Task<bool> EmailExistsAsync(string email, CancellationToken ct);
    void Add(Student student);
}