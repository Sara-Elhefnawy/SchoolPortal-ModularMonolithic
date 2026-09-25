using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.Identities.Application.Directories;
using SchoolPortal.Identities.Domain.Users;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Users.Query.GetAll;

public sealed class GetAllUsersQueryHandler(
    IIdentityDirectory directory) 
    : IRequestHandler<GetAllUsersQuery, Result<IReadOnlyList<ApplicationUser>>>
{
    public async Task<Result<IReadOnlyList<ApplicationUser>>> Handle(GetAllUsersQuery request, CancellationToken ct)
    {
        var users = await directory.GetAllUsersAsync(ct);
        
        return Result<IReadOnlyList<ApplicationUser>>.Success(users);
    }
}