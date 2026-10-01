using Microsoft.EntityFrameworkCore;
using SchoolPortal.Students.Application.Directories;
using SchoolPortal.Students.Application.Features.Students.Query.GetAll;
using SchoolPortal.Students.Domain.Students;
using SchoolPortal.Students.Infrastructure.Persistence;

namespace SchoolPortal.Students.Infrastructure.Directories;

internal sealed class StudentDirectory(StudentsDbContext dbContext) : IStudentDirectory
{
    public async Task<IReadOnlyList<GetAllStudentsQueryDto>> GetAllAsync(string? search, StudentStatus? status,
        CancellationToken ct)
    {
        var query = dbContext.Students.AsNoTracking();

        if (status.HasValue)
            query = query.Where(s => s.Status == status.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            // ILike, not Contains: PostgreSQL's LIKE is case-sensitive, so "sara" would never
            // match "Sara". ILike does the case-insensitive match in the database.
            var pattern = $"%{search.Trim()}%";
            query = query.Where(s => EF.Functions.ILike(s.Name, pattern) || EF.Functions.ILike(s.Email, pattern));
        }

        // Project to plain columns first, then turn the enum into text in memory. That avoids
        // depending on EF translating Enum.ToString() into SQL.
        var rows = await query.OrderBy(s => s.Name).Select(s => new
        {
            s.Id,
            s.Name,
            s.Email,
            s.PhoneNumber,
            s.Status,
            s.DateOfBirth,
            s.Gender,
            s.AdmissionDate
        }).ToListAsync(ct);

        return rows.Select(r => new GetAllStudentsQueryDto(r.Id, r.Name, r.Email, r.PhoneNumber, r.Status.ToString(),
            r.DateOfBirth, r.Gender, r.AdmissionDate)).ToList();
    }

    public async Task<Student?> GetByIdAsync(int id, CancellationToken ct) =>
        await dbContext.Students.Include(s => s.Address).FirstOrDefaultAsync(a => a.Id == id, ct);

    public async Task<IReadOnlyDictionary<int, string>> GetNamesAsync(IReadOnlyCollection<int> ids,
        CancellationToken ct) =>
        // Projection to just Id + Name: no need to load whole Student rows (and their owned Address) to show a label.
        await dbContext.Students.AsNoTracking().Where(s => ids.Contains(s.Id))
            .ToDictionaryAsync(s => s.Id, s => s.Name, ct);
    
    public async Task<bool> ExistsAsync(int id, CancellationToken ct) =>
        await dbContext.Students.AnyAsync(a => a.Id == id, ct);

    public async Task<bool> EmailExistsAsync(string email, CancellationToken ct) =>
        await dbContext.Students.AnyAsync(a => a.Email == email, ct);

    public void Add(Student student) => dbContext.Students.Add(student);
}