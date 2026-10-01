using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.Identities.Application.Directories;
using SchoolPortal.Identities.Application.Directories.Dtos;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Users.Query.GetAll;

public sealed class GetAllUsersQueryHandler(
    IIdentityDirectory directory) 
    : IRequestHandler<GetAllUsersQuery, Result<IReadOnlyList<UserDto>>>
{
    public async Task<Result<IReadOnlyList<UserDto>>> Handle(GetAllUsersQuery request, CancellationToken ct)
    {
        var users = await directory.GetAllUsersAsync(ct);
        var roles = await directory.GetRoleNamesByUserAsync(ct);

        IReadOnlyList<UserDto> list = users
            .Where(u => !u.IsDeleted)
            .Select(u => new UserDto(u.Id, u.Name, u.Email!, u.IsActive,
                roles.GetValueOrDefault(u.Id, [])))
            .ToList();
        
        return Result<IReadOnlyList<UserDto>>.Success(list);
    }
}