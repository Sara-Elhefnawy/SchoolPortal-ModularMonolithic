using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.Identities.Application.Directories;
using SchoolPortal.Identities.Application.Directories.Dtos;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Roles.Query.GetAll;

public sealed class GetAllRolesQueryHandler(
    IIdentityDirectory directory) 
    : IRequestHandler<GetAllRolesQuery, Result<IReadOnlyList<RoleDto>>>
{
    public async Task<Result<IReadOnlyList<RoleDto>>> Handle(GetAllRolesQuery request, CancellationToken ct)
    {
        var roles = await directory.GetAllRoleAsync(ct);

        IReadOnlyList<RoleDto> list = roles.Select(r => new RoleDto(r.Id, r.Name!, r.IsDeleted)).ToList();
        
        return Result<IReadOnlyList<RoleDto>>.Success(list);
    }
}