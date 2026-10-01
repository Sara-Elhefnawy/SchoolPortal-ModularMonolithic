namespace SchoolPortal.SharedKernel.Lookups;

public interface IStudentNameLookup
{
    Task<IReadOnlyDictionary<int, string>> GetNamesAsync(
        IReadOnlyCollection<int> studentIds, CancellationToken ct);
}