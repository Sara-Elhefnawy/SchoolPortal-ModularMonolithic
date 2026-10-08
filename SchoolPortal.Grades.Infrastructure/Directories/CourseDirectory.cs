using Dapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using SchoolPortal.Grades.Application.Directories;
using SchoolPortal.Grades.Application.Features.Courses.Query.GetAll;
using SchoolPortal.Grades.Domain.Courses;
using SchoolPortal.Grades.Infrastructure.Persistence;

namespace SchoolPortal.Grades.Infrastructure.Directories;

// Reads that return DTOs use Dapper; entity loads, guards and staging writes stay on EF.
internal sealed class CourseDirectory(GradesDbContext dbContext, NpgsqlDataSource dataSource) : ICourseDirectory
{
    // ------------------------------- READS (Dapper)
    public async Task<IReadOnlyList<GetAllCoursesQueryDto>> GetAllAsync(CancellationToken ct)
    {
        // WHY "IsDeleted" = FALSE: Dapper bypasses EF's soft-delete query filter.
        const string sql = """
                           SELECT "Id", "Name", "Code", "CreditHours"
                           FROM grades."Courses"
                           WHERE "IsDeleted" = FALSE
                           ORDER BY "Code";
                           """;

        await using var connection = await dataSource.OpenConnectionAsync(ct);

        var rows = await connection.QueryAsync<Row>(new CommandDefinition(sql, cancellationToken: ct));

        return rows.Select(r => new GetAllCoursesQueryDto(r.Id, r.Name, r.Code, r.CreditHours)).ToList();
    }

    // ------------------------------- WRITE SIDE (EF)
    public async Task<Course?> GetByIdAsync(int id, CancellationToken ct) =>
        await dbContext.Courses.FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<bool> ExistsWithCodeAsync(string code, CancellationToken ct) =>
        await dbContext.Courses.AnyAsync(c => c.Code == code, ct);

    public void Add(Course course) => dbContext.Courses.Add(course);

    private sealed record Row(int Id, string Name, string Code, decimal CreditHours);
}