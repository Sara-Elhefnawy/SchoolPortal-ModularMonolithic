using SchoolPortal.Students.Application.Dtos;

namespace SchoolPortal.Students.Application.Abstractions;

public interface IStudentDirectory
{
    Task<StudentSummaryDto?> GetSummaryAsync(int studentId, CancellationToken ct);
}