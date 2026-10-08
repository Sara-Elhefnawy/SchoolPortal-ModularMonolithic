using Dapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using SchoolPortal.Grades.Application.Directories;
using SchoolPortal.Grades.Application.Features.AcademicYears.Query.GetAll;
using SchoolPortal.Grades.Domain.AcademicYears;
using SchoolPortal.Grades.Infrastructure.Persistence;

namespace SchoolPortal.Grades.Infrastructure.Directories;

// WHY two dependencies: this class has two different jobs.
//   - Display queries (return DTOs, nothing mutated)  -> Dapper via NpgsqlDataSource
//   - Loading an entity to mutate, existence guards, staging writes -> EF via GradesDbContext
internal sealed class AcademicYearDirectory(GradesDbContext dbContext, NpgsqlDataSource dataSource)
    : IAcademicYearDirectory
{
    // ------------------------------- READS (Dapper)
    public async Task<IReadOnlyList<GetAllAcademicYearsQueryDto>> GetAllAsync(CancellationToken ct)
    {
        // WHY "IsDeleted" = FALSE by hand: HasQueryFilter is an EF feature.
        // Dapper never sees it, so every raw query must repeat the soft-delete rule
        // or deleted rows leak into the UI.
        const string sql = """
                           SELECT "Id", "Name", "StartDate", "EndDate", "IsCurrent"
                           FROM grades."AcademicYears"
                           WHERE "IsDeleted" = FALSE
                           ORDER BY "StartDate" DESC;
                           """;

        // WHY open per call: NpgsqlDataSource hands out pooled connections, so this is cheap,
        // and it keeps reads independent of the DbContext's connection and tracker.
        await using var connection = await dataSource.OpenConnectionAsync(ct);

        var rows = await connection.QueryAsync<Row>(new CommandDefinition(sql, cancellationToken: ct));

        // WHY a private Row type instead of mapping straight to the DTO?
        // Dapper fills a positional record by matching column names to constructor parameter names.
        // Going through Row means the match depends only on names I control here,
        // not on how the DTO happens to name its parameters.
        return rows.Select(r => new GetAllAcademicYearsQueryDto(r.Id, r.Name, r.StartDate, r.EndDate, r.IsCurrent))
            .ToList();
    }

    // ------------------------------- WRITE SIDE (EF)
    public async Task<AcademicYear?> GetByIdAsync(int id, CancellationToken ct) =>
        await dbContext.AcademicYears.FirstOrDefaultAsync(a => a.Id == id, ct);

    public async Task<AcademicYear?> FindCurrentAsync(CancellationToken ct) =>
        await dbContext.AcademicYears.FirstOrDefaultAsync(a => a.IsCurrent, ct);

    public async Task<bool> NameExistsAsync(string name, CancellationToken ct) =>
        await dbContext.AcademicYears.AnyAsync(a => a.Name == name, ct);

    public void Add(AcademicYear academicYear) => dbContext.AcademicYears.Add(academicYear);

    // Kept, unlike Grade's — this one takes the entity as a parameter
    // rather than assuming the caller already holds a tracked reference
    // from FindAsync, so an explicit Update() is the safer default here.
    // Still no SaveChangesAsync: staging only.
    public void Update(AcademicYear academicYear) => dbContext.AcademicYears.Update(academicYear);

    // Shape of the SELECT above. Property names match the column names (Dapper matches case-insensitively).
    private sealed record Row(int Id, string Name, DateOnly StartDate, DateOnly EndDate, bool IsCurrent);
}