namespace SchoolPortal.SharedKernel.Persistence;

// The shared contract — just "you can save." Nothing module-specific lives
// here on purpose: a handler that only needs to commit shouldn't need to
// know which module's DbContext is behind the call.
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
    /*
    Task SaveChangesAsync(CancellationToken ct);
*/
}