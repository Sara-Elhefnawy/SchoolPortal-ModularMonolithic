namespace SchoolPortal.SharedKernel.Lookups;

public interface IStudentLookup
{
    Task<int?> FindStudentIdByEmailAsync(string email, CancellationToken ct);
    
    Task<IReadOnlyDictionary<int, string>> GetNamesAsync(
        IReadOnlyCollection<int> studentIds, CancellationToken ct);
}