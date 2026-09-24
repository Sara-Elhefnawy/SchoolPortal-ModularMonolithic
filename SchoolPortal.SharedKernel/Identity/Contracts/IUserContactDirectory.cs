namespace SchoolPortal.SharedKernel.Identity.Contracts;

public interface IUserContactDirectory
{
    Task<string?> GetEmailAsync(int userId, CancellationToken ct);
}