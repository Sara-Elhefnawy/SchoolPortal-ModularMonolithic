using MediatR;
using Microsoft.AspNetCore.Identity;
using SchoolPortal.Identities.Domain.Roles;
using SchoolPortal.Identities.Domain.Users;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Users.Command.AssignRole;

public sealed class AssignRoleCommandHandler(
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager) 
    : IRequestHandler<AssignRoleCommand, Result>
{
    public async Task<Result> Handle(AssignRoleCommand request, CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(request.Id.ToString());
        if (user is null || user.IsDeleted)
            return Result.Failure(UserErrors.NotFound);

        var role = await roleManager.FindByNameAsync(request.RoleName);
        if (role is null || role.IsDeleted)
            return Result.Failure(RoleErrors.NotFound);

        var alreadyAssigned = await userManager.IsInRoleAsync(user, role.Name!);
        if (alreadyAssigned)
            return Result.Failure(RoleErrors.AlreadyAssigned);

        var result = await userManager.AddToRoleAsync(user, role.Name!);
        if (!result.Succeeded)
            return Result.Failure(RoleErrors.AssignmentFailed);

        return Result.Success();
    }
}