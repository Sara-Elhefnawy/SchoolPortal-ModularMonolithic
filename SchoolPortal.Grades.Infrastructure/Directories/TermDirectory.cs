using Dapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using SchoolPortal.Grades.Application.Directories;
using SchoolPortal.Grades.Application.Features.Terms.Query.GetByAcademicYear;
using SchoolPortal.Grades.Domain.Terms;
using SchoolPortal.Grades.Infrastructure.Persistence;

namespace SchoolPortal.Grades.Infrastructure.Directories;

// Reads that return DTOs use Dapper; entity loads, guards and staging writes stay on EF.
internal sealed class TermDirectory(GradesDbContext dbContext, NpgsqlDataSource dataSource) : ITermDirectory
{
    // ------------------------------- READS (Dapper)
    public async Task<IReadOnlyList<GetTermsByAcademicYearQueryDto>> GetByAcademicYearAsync(int academicYearId,
        CancellationToken ct)
    {
        // WHY "Type" is selected as-is: TermConfiguration stores the enum with HasConversion<string>(),
        // so the column already holds the text that x.Type.ToString() used to produce.
        // WHY "IsDeleted" = FALSE: Dapper bypasses EF's soft-delete query filter.
        const string sql = """
                           SELECT "Id", "Name", "Type", "StartDate", "EndDate", "AcademicYearId", "HasEnded"
                           FROM grades."Terms"
                           WHERE "AcademicYearId" = @AcademicYearId
                             AND "IsDeleted" = FALSE
                           ORDER BY "StartDate";
                           """;

        await using var connection = await dataSource.OpenConnectionAsync(ct);

        var rows = await connection.QueryAsync<Row>(new CommandDefinition(sql, new { AcademicYearId = academicYearId },
            cancellationToken: ct));

        return rows.Select(r => new GetTermsByAcademicYearQueryDto(r.Id, r.Name, r.Type, r.StartDate, r.EndDate,
            r.AcademicYearId, r.HasEnded)).ToList();
    }

    // ------------------------------- WRITE SIDE (EF)
    public async Task<Term?> GetByIdAsync(int id, CancellationToken ct) =>
        await dbContext.Terms.FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task<bool> AcademicYearAndNameExistsAsync(int academicYearId, string name, CancellationToken ct) =>
        await dbContext.Terms.AnyAsync(t => t.AcademicYearId == academicYearId && t.Name == name, ct);

    public void Add(Term term) => dbContext.Terms.Add(term);

    private sealed record Row(
        int Id,
        string Name,
        string Type,
        DateOnly StartDate,
        DateOnly EndDate,
        int AcademicYearId,
        bool HasEnded);
}