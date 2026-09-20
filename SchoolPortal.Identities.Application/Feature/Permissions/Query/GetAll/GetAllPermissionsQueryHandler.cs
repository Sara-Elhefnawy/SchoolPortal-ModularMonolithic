using MediatR;
using SchoolPortal.Identities.Application.Directories;
using SchoolPortal.Identities.Domain.Permissions;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Permissions.Query.GetAll;

public sealed class GetAllPermissionsQueryHandler(
    IIdentityDirectory directory) 
    : IRequestHandler<GetAllPermissionsQuery, Result<IReadOnlyList<Permission>>>
{
    public async Task<Result<IReadOnlyList<Permission>>> Handle(GetAllPermissionsQuery request, CancellationToken ct)
    {
        var permissions = await directory.GetAllPermissionsAsync(ct);
        
        return Result<IReadOnlyList<Permission>>.Success(permissions);
    }
}