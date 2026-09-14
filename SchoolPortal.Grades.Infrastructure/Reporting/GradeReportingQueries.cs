using Dapper;
using Npgsql;
using SchoolPortal.Grades.Application.Reporting.Dto;
using SchoolPortal.Grades.Application.Reporting.Queries;

namespace SchoolPortal.Grades.Infrastructure.Reporting;

// EF's job is "load one aggregate, mutate it, save it"
// GPA/ranking is the opposite: read-only, joins four tables,
//      aggregates across many students, and nothing gets tracked or mutated.
// Forcing that through DbSet<Grade>.Include(...).Select(...) works, but you
//      fight EF's tracking machinery for zero benefit, and the LINQ-to-SQL
//      translation for a weighted average across a join gets unreadable fast.
// Writing the SQL directly here isn't a shortcut — it's the right tool for
//      a genuinely different kind of operation.
internal sealed class GradeReportingQueries(NpgsqlDataSource dataSource) : IGradeReportingQueries
{
    public async Task<IReadOnlyList<StudentRankingDto>> GetClassRankingsAsync(int courseSectionId, CancellationToken ct)
    {
        // Only Published/Locked grades count toward a ranking — a Draft or
        // Submitted grade hasn't cleared the workflow your GradeStatus enum
        // encodes, so it shouldn't move anyone's rank yet. This mirrors the
        // state-machine rule already enforced in the Grade entity itself; the
        // query has to re-state it because SQL doesn't know about your domain
        // methods.
        //
        // Weighted average = sum(Score/MaximumScore * Weight) per student,
        // computed only over assessments the student actually has a
        // countable grade for — a student missing one assessment isn't
        // penalized to zero for it here (that's a policy decision worth
        // confirming with whoever "owns" grading rules, not something to
        // silently assume).
        
        // EF Core 8+ defaults to the entity class name, not the DbSet name,
        // so it'd be grades."Grade" (singular) not grades."Grades"
        const string sql = """
                           SELECT
                               g."StudentId",
                               SUM((g."Score"::numeric / NULLIF(a."MaximumScore", 0)) * a."Weight") AS "WeightedAverage",
                               RANK() OVER (ORDER BY SUM((g."Score"::numeric / NULLIF(a."MaximumScore", 0)) * a."Weight") DESC) AS "Rank"
                           FROM grades."Grades" g
                           INNER JOIN grades."Assessments" a
                               ON a."Id" = g."AssessmentId"
                           WHERE a."CourseSectionId" = @CourseSectionId
                             AND g."Status" IN ('Published', 'Locked')
                             AND g."IsDeleted" = FALSE
                             AND a."IsDeleted" = FALSE
                           GROUP BY g."StudentId"
                           ORDER BY "Rank", g."StudentId";
                           """;

        await using var connection = await dataSource.OpenConnectionAsync(ct);
        
        var command = new CommandDefinition(sql, new { CourseSectionId = courseSectionId }, cancellationToken: ct);
        
        var results = await connection.QueryAsync<StudentRankingDto>(command);
        
        return results.AsList();
    }
}