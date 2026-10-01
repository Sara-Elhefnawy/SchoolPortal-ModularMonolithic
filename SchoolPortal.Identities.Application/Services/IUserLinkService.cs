using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Services;

public interface IUserLinkService
{
    Task<Result> LinkByEmailAsync(string email, int studentId, CancellationToken ct);
}