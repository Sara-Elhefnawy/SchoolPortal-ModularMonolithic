using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.Identities.Application.Directories;
using SchoolPortal.Identities.Application.Directories.Dtos;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Permissions.Query.GetAll;

public sealed class GetAllPermissionsQueryHandler(
    IIdentityDirectory directory) 
    : IRequestHandler<GetAllPermissionsQuery, Result<IReadOnlyList<PermissionDto>>>
{
    public async Task<Result<IReadOnlyList<PermissionDto>>> Handle(GetAllPermissionsQuery request, CancellationToken ct)
    {
        var permissions = await directory.GetAllPermissionsAsync(ct);
        
        IReadOnlyList<PermissionDto> list = permissions.Select(p => new PermissionDto(p.Name)).ToList();
        
        return Result<IReadOnlyList<PermissionDto>>.Success(list);
    }
}