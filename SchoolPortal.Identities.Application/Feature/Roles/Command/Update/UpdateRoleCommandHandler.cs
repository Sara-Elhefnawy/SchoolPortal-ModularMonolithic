using MediatR;
using Microsoft.AspNetCore.Identity;
using SchoolPortal.Identities.Domain.Roles;
using SchoolPortal.SharedKernel.Identity.Contracts;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Roles.Command.Update;

public sealed class UpdateRoleCommandHandler(
    RoleManager<ApplicationRole> roleManager,
    ICurrentUser currentUser)
    : IRequestHandler<UpdateRoleCommand, Result>
{
    public async Task<Result> Handle(UpdateRoleCommand request, CancellationToken ct)
    {
        var role = await roleManager.FindByIdAsync(request.Id.ToString());
        if (role is null || role.IsDeleted)
            return Result.Failure(RoleErrors.NotFound);

        var roleWithSameName = await roleManager.FindByNameAsync(request.Name);

        if (roleWithSameName is not null && roleWithSameName.Id != role.Id)
            return Result.Failure(RoleErrors.AlreadyExists);

        role.Name = request.Name;
        role.UpdatedAt = DateTime.UtcNow;
        role.UpdatedByUserId = currentUser.UserId;

        var result = await roleManager.UpdateAsync(role);
        if (!result.Succeeded)
            return Result.Failure(RoleErrors.UpdateFailed);

        return Result.Success();
    }
}