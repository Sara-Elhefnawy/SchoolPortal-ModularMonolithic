using MediatR;
using Microsoft.AspNetCore.Identity;
using SchoolPortal.Identities.Domain.Roles;
using SchoolPortal.SharedKernel.Identity.Users;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Roles.Command.Restore;

public sealed class RestoreRoleCommandHandler(
    RoleManager<ApplicationRole> roleManager,
    ICurrentUser currentUser) 
    : IRequestHandler<RestoreRoleCommand, Result>
{
    public async Task<Result> Handle(RestoreRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByIdAsync(request.Id.ToString());
        if (role is null || !role.IsDeleted)
            return Result.Failure(RoleErrors.NotFound);
        
        role.IsDeleted = false;
        role.RestoredAt = DateTime.UtcNow;
        role.RestoredByUserId = currentUser.UserId;

        var result = await roleManager.UpdateAsync(role);
        if (!result.Succeeded)
            return Result.Failure(RoleErrors.RestorationFailed);

        return Result.Success();
    }
}