using Dapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using SchoolPortal.Grades.Application.Directories;
using SchoolPortal.Grades.Application.Features.Grades.Query.GetByAssessment;
using SchoolPortal.Grades.Application.Features.Grades.Query.GetForStudent;
using SchoolPortal.Grades.Domain.Grades;
using SchoolPortal.Grades.Infrastructure.Persistence;

namespace SchoolPortal.Grades.Infrastructure.Directories;

// Reads that return DTOs use Dapper; entity loads and staging writes stay on EF.
internal sealed class GradeDirectory(GradesDbContext dbContext, NpgsqlDataSource dataSource) : IGradeDirectory
{
    // ------------------------------- READS (Dapper)
    public async Task<string?> GetAssessmentNameAsync(int assessmentId, CancellationToken ct)
    {
        // WHY "IsDeleted" = FALSE: Dapper bypasses EF's soft-delete query filter.
        const string sql = """
                           SELECT "Name"
                           FROM grades."Assessments"
                           WHERE "Id" = @AssessmentId
                             AND "IsDeleted" = FALSE;
                           """;

        await using var connection = await dataSource.OpenConnectionAsync(ct);

        // WHY QueryFirstOrDefault: no row -> null, matching the old FirstOrDefaultAsync behavior.
        return await connection.QueryFirstOrDefaultAsync<string>(
            new CommandDefinition(sql, new { AssessmentId = assessmentId }, cancellationToken: ct));
    }

    public async Task<IReadOnlyList<GetGradesByAssessmentQueryDto>> GetByAssessmentIdAsync(int assessmentId,
        CancellationToken ct)
    {
        // WHY one LEFT JOIN instead of the old two queries: one round trip, and the assessment name
        // arrives on every row. LEFT (not INNER) so grades still come back if the assessment is
        // soft-deleted; COALESCE then gives the empty string the old `?? string.Empty` produced.
        // The assessment's IsDeleted check lives in ON (not WHERE) precisely so a deleted assessment
        // does not remove the grade rows.
        const string sql = """
                           SELECT
                               g."Id",
                               g."StudentId",
                               g."AssessmentId",
                               COALESCE(a."Name", '') AS "AssessmentName",
                               g."Score",
                               g."Status"
                           FROM grades."Grades" g
                           LEFT JOIN grades."Assessments" a
                               ON a."Id" = g."AssessmentId" AND a."IsDeleted" = FALSE
                           WHERE g."AssessmentId" = @AssessmentId
                             AND g."IsDeleted" = FALSE;
                           """;

        await using var connection = await dataSource.OpenConnectionAsync(ct);

        var rows = await connection.QueryAsync<ByAssessmentRow>(
            new CommandDefinition(sql, new { AssessmentId = assessmentId }, cancellationToken: ct));

        // "Status" is stored as text (HasConversion<string>), so it already equals Status.ToString().
        return rows.Select(r => new GetGradesByAssessmentQueryDto(r.Id, r.StudentId,
            $"Student #{r.StudentId}", // replace with real name lookup
            r.AssessmentId, r.AssessmentName, r.Score, r.Status)).ToList();
    }

    public async Task<IReadOnlyList<GetGradesForStudentQueryDto>> GetForStudentAsync(int studentId,
        CancellationToken ct)
    {
        // WHY COALESCE(UpdatedAt, CreatedAt): SQL spelling of the old `UpdatedAt ?? CreatedAt`.
        // It is used both for sorting and for the value returned, so it is computed once as "UpdatedAt".
        // WHY IsDeleted on both tables: EF applied the soft-delete filter to the joined table as well.
        const string sql = """
                           SELECT
                               g."Id",
                               g."StudentId",
                               g."AssessmentId",
                               a."Name" AS "AssessmentName",
                               g."Score",
                               g."Status",
                               COALESCE(g."UpdatedAt", g."CreatedAt") AS "UpdatedAt"
                           FROM grades."Grades" g
                           INNER JOIN grades."Assessments" a
                               ON a."Id" = g."AssessmentId" AND a."IsDeleted" = FALSE
                           WHERE g."StudentId" = @StudentId
                             AND g."IsDeleted" = FALSE
                           ORDER BY COALESCE(g."UpdatedAt", g."CreatedAt") DESC;
                           """;

        await using var connection = await dataSource.OpenConnectionAsync(ct);

        var rows = await connection.QueryAsync<ForStudentRow>(
            new CommandDefinition(sql, new { StudentId = studentId }, cancellationToken: ct));

        return rows.Select(r => new GetGradesForStudentQueryDto(Id: r.Id, StudentId: r.StudentId,
            AssessmentId: r.AssessmentId, AssessmentName: r.AssessmentName, Score: r.Score, Status: r.Status,
            UpdatedAt: r.UpdatedAt)).ToList();
    }

    // ------------------------------- WRITE SIDE (EF)
    public async Task<Grade?> GetByIdAsync(int id, CancellationToken ct) =>
        await dbContext.Grades.FirstOrDefaultAsync(g => g.Id == id, ct);

    public async Task<Grade?> FindStudentAssessmentGradeAsync(int studentId, int assessmentId, CancellationToken ct) =>
        await dbContext.Grades.FirstOrDefaultAsync(g => g.StudentId == studentId && g.AssessmentId == assessmentId, ct);

    public void Add(Grade grade) => dbContext.Grades.Add(grade);

    private sealed record ByAssessmentRow(
        int Id,
        int StudentId,
        int AssessmentId,
        string AssessmentName,
        decimal Score,
        string Status);

    private sealed record ForStudentRow(
        int Id,
        int StudentId,
        int AssessmentId,
        string AssessmentName,
        decimal Score,
        string Status,
        DateTime UpdatedAt);
}