using SchoolPortal.SharedKernel.Lookups;
using SchoolPortal.Students.Application.Directories;

namespace SchoolPortal.Students.Infrastructure.Persistence.Lookups;

internal sealed class StudentLookup(IStudentDirectory students) : IStudentLookup
{
    public async Task<int?> FindStudentIdByEmailAsync(string email, CancellationToken ct)
    {
        var all = await students.GetAllAsync(ct: ct);

        return all.FirstOrDefault(s => string.Equals(s.Email, email, StringComparison.OrdinalIgnoreCase))?.Id;
    }

    public Task<IReadOnlyDictionary<int, string>> GetNamesAsync(IReadOnlyCollection<int> studentIds,
        CancellationToken ct) =>
        students.GetNamesAsync(studentIds, ct);
}