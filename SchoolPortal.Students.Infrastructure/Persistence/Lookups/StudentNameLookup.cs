using SchoolPortal.SharedKernel.Lookups;
using SchoolPortal.Students.Application.Directories;

namespace SchoolPortal.Students.Infrastructure.Persistence.Lookups;

internal sealed class StudentNameLookup(IStudentDirectory students) : IStudentNameLookup
{
    public Task<IReadOnlyDictionary<int, string>> GetNamesAsync(IReadOnlyCollection<int> studentIds,
        CancellationToken ct) =>
        students.GetNamesAsync(studentIds, ct);
}