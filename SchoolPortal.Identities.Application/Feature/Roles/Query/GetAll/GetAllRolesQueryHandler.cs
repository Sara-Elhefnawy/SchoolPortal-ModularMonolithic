using MediatR;
using SchoolPortal.Identities.Application.Directories;
using SchoolPortal.Identities.Domain.Roles;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Roles.Query.GetAll;

public sealed class GetAllRolesQueryHandler(
    IIdentityDirectory directory) 
    : IRequestHandler<GetAllRolesQuery, Result<IReadOnlyList<ApplicationRole>>>
{
    public async Task<Result<IReadOnlyList<ApplicationRole>>> Handle(GetAllRolesQuery request, CancellationToken ct)
    {
        var roles = await directory.GetAllRoleAsync(ct);

        return Result<IReadOnlyList<ApplicationRole>>.Success(roles);
    }
}