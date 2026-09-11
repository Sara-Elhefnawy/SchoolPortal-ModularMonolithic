using SchoolPortal.Students.Application.Dtos;

namespace SchoolPortal.Students.Application.Directories;

public interface IStudentDirectory
{
    Task<StudentSummaryDto?> GetSummaryAsync(int studentId, CancellationToken ct);
}