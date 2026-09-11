using SchoolPortal.Students.Application.Dtos;
using Microsoft.EntityFrameworkCore;
using SchoolPortal.Students.Application.Directories;

namespace SchoolPortal.Students.Infrastructure.Directories;

internal sealed class StudentDirectory(StudentsDbContext db) : IStudentDirectory
{
    public async Task<StudentSummaryDto?> GetSummaryAsync(int studentId, CancellationToken ct)
    {
        return await db.Students
            .Where(s => s.Id == studentId)
            .Select(s => new StudentSummaryDto(s.Id, s.Name, s.Email))
            .FirstOrDefaultAsync(ct);
    }
}