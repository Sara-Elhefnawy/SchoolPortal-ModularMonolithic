using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.Identities.Application.Directories;
using SchoolPortal.Identities.Application.Directories.Dtos;
using SchoolPortal.Identities.Domain.Users;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Users.Query.GetById;

public sealed class GetUserByIdQueryHandler (
    IIdentityDirectory directory)
    : IRequestHandler<GetUserByIdQuery, Result<UserDto>> 
{
    public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken ct)
    {
        var user = await directory.GetUserByIdAsync(request.Id, ct);
        if (user is null || user.IsDeleted)
            return Result<UserDto>.Failure(UserErrors.NotFound);
        
        var roles = await directory.GetRoleNamesForUserAsync(user.Id, ct);

        return Result<UserDto>.Success(new UserDto(user.Id, user.Name, user.Email!, user.IsActive, roles));
    }
}