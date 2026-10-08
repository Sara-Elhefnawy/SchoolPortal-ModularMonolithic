using Dapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using SchoolPortal.Grades.Application.Directories;
using SchoolPortal.Grades.Application.Features.CourseSections.Query.GetByTerm;
using SchoolPortal.Grades.Application.Features.CourseSections.Query.GetRankings;
using SchoolPortal.Grades.Domain.CourseSections;
using SchoolPortal.Grades.Infrastructure.Persistence;

namespace SchoolPortal.Grades.Infrastructure.Directories;

// Reads that return DTOs use Dapper; entity loads, guards and staging writes stay on EF.
internal sealed class CourseSectionDirectory(GradesDbContext dbContext, NpgsqlDataSource dataSource)
    : ICourseSectionDirectory
{
    // ------------------------------- READS (Dapper)
    public async Task<IReadOnlyList<GetCourseSectionRankingsQueryDto>> GetRankingsAsync(int courseSectionId,
        CancellationToken ct)
    {
        // WHY this moved to SQL: the old version pulled every assessment AND every grade into memory, then grouped and averaged in C#.
        // A weighted average per student is exactly what the database is good at, and only one row per student crosses the wire now.
        //
        // WHY ROW_NUMBER and not RANK: the old code gave position i + 1, so tied students got
        // different ranks. ROW_NUMBER keeps that; "StudentId" is the tie-break so the order is stable.
        // Swap in RANK() if you want tied students to share a position.
        const string sql = """
                           WITH averages AS (
                               SELECT
                                   g."StudentId",
                                   COALESCE(
                                       ROUND(
                                           SUM(COALESCE(g."Score" / NULLIF(a."MaximumScore", 0), 0) * a."Weight")
                                           / NULLIF(SUM(a."Weight"), 0) * 100, 2),
                                       0) AS "Average"
                               FROM grades."Grades" g
                               INNER JOIN grades."Assessments" a
                                   ON a."Id" = g."AssessmentId"
                               WHERE a."CourseSectionId" = @CourseSectionId
                                 AND g."Status" IN ('Published', 'Locked')
                                 AND g."IsDeleted" = FALSE
                                 AND a."IsDeleted" = FALSE
                               GROUP BY g."StudentId"
                           )
                           SELECT
                               (ROW_NUMBER() OVER (ORDER BY "Average" DESC, "StudentId"))::int AS "Rank",
                               "StudentId",
                               "Average"
                           FROM averages
                           ORDER BY "Rank";
                           """;

        await using var connection = await dataSource.OpenConnectionAsync(ct);

        var rows = await connection.QueryAsync<RankingRow>(new CommandDefinition(sql,
            new { CourseSectionId = courseSectionId }, cancellationToken: ct));

        return rows.Select(r => new GetCourseSectionRankingsQueryDto(r.Rank, r.StudentId,
            $"Student #{r.StudentId}",
            r.Average)).ToList();
    }

    public async Task<IReadOnlyList<GetCourseSectionsByTermQueryDto>> GetByTermIdAsync(int termId, CancellationToken ct)
    {
        // WHY IsDeleted sits in each ON clause: EF applied the soft-delete filter to all three tables
        // in the old query. For an INNER JOIN, putting it in ON or WHERE gives the same rows;
        // ON keeps each rule next to the table it protects.
        //
        // WHY the label is built in C#, not SQL: keeps the display format (the en dash, the brackets)
        // in one readable line of code instead of string concatenation inside the query.
        const string sql = """
                           SELECT
                               s."Id",
                               s."CourseId",
                               c."Code" AS "CourseCode",
                               c."Name" AS "CourseName",
                               t."Name" AS "TermName",
                               s."TermId"
                           FROM grades."CourseSections" s
                           INNER JOIN grades."Courses" c
                               ON c."Id" = s."CourseId" AND c."IsDeleted" = FALSE
                           INNER JOIN grades."Terms" t
                               ON t."Id" = s."TermId" AND t."IsDeleted" = FALSE
                           WHERE s."TermId" = @TermId
                             AND s."IsDeleted" = FALSE;
                           """;

        await using var connection = await dataSource.OpenConnectionAsync(ct);

        var rows = await connection.QueryAsync<SectionRow>(new CommandDefinition(sql, new { TermId = termId },
            cancellationToken: ct));

        return rows.Select(r => new GetCourseSectionsByTermQueryDto(r.Id, r.CourseId,
            $"{r.CourseCode} – {r.CourseName} ({r.TermName})", r.TermId)).ToList();
    }

    // ------------------------------- WRITE SIDE (EF)
    public async Task<CourseSection?> GetByIdAsync(int id, CancellationToken ct) =>
        await dbContext.CourseSections.FirstOrDefaultAsync(cs => cs.Id == id, ct);

    public async Task<bool> CourseAndTermExistsAsync(int courseId, int termId, CancellationToken ct) =>
        await dbContext.CourseSections.AnyAsync(cs => cs.CourseId == courseId && cs.TermId == termId, ct);

    public void Add(CourseSection courseSection) => dbContext.CourseSections.Add(courseSection);

    private sealed record RankingRow(int Rank, int StudentId, decimal Average);

    private sealed record SectionRow(
        int Id,
        int CourseId,
        string CourseCode,
        string CourseName,
        string TermName,
        int TermId);
}