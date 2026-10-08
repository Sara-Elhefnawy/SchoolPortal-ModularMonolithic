using Dapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using SchoolPortal.Grades.Application.Directories;
using SchoolPortal.Grades.Application.Features.Assessments.Query.GetByCourseSection;
using SchoolPortal.Grades.Domain.Assessments;
using SchoolPortal.Grades.Infrastructure.Persistence;

namespace SchoolPortal.Grades.Infrastructure.Directories;

// Reads that return DTOs use Dapper; entity loads and staging writes stay on EF.
internal sealed class AssessmentDirectory(GradesDbContext dbContext, NpgsqlDataSource dataSource) : IAssessmentDirectory
{
    // ------------------------------- READS (Dapper)
    public async Task<IReadOnlyList<GetAssessmentsByCourseSectionQueryDto>> GetBySectionAsync(int courseSectionId,
        CancellationToken ct)
    {
        // WHY "IsDeleted" = FALSE: Dapper bypasses EF's soft-delete query filter.
        const string sql = """
                           SELECT "Id", "CourseSectionId", "Name", "Type", "MaximumScore", "Weight", "Date"
                           FROM grades."Assessments"
                           WHERE "CourseSectionId" = @CourseSectionId
                             AND "IsDeleted" = FALSE
                           ORDER BY "Date";
                           """;

        await using var connection = await dataSource.OpenConnectionAsync(ct);

        var rows = await connection.QueryAsync<Row>(new CommandDefinition(sql,
            new { CourseSectionId = courseSectionId }, cancellationToken: ct));

        return rows.Select(r => new GetAssessmentsByCourseSectionQueryDto(r.Id, r.CourseSectionId, r.Name, r.Type,
            r.MaximumScore, r.Weight, r.Date)).ToList();
    }

    // ------------------------------- WRITE SIDE (EF)
    public async Task<Assessment?> GetByIdAsync(int id, CancellationToken ct) =>
        await dbContext.Assessments.FirstOrDefaultAsync(a => a.Id == id, ct);

    public void Add(Assessment assessment) => dbContext.Assessments.Add(assessment);

    private sealed record Row(
        int Id,
        int CourseSectionId,
        string Name,
        string Type,
        decimal MaximumScore,
        decimal Weight,
        DateOnly Date);
}