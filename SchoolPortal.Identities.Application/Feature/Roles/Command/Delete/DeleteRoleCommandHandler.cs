using MediatR;
using Microsoft.AspNetCore.Identity;
using SchoolPortal.Identities.Domain.Roles;
using SchoolPortal.SharedKernel.Identity.Contracts;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Roles.Command.Delete;

public sealed class DeleteRoleCommandHandler(
    RoleManager<ApplicationRole> roleManager,
    ICurrentUser currentUser)
    : IRequestHandler<DeleteRoleCommand, Result>
{
    public async Task<Result> Handle(DeleteRoleCommand request, CancellationToken ct)
    {
        var role = await roleManager.FindByIdAsync(request.Id.ToString());
        if (role is null || role.IsDeleted)
            return Result.Failure(RoleErrors.NotFound);
        
        role.IsDeleted = true;
        role.DeletedAt = DateTime.UtcNow;
        role.DeletedByUserId = currentUser.UserId;

        var result = await roleManager.UpdateAsync(role);
        if (!result.Succeeded)
            return Result.Failure(RoleErrors.DeletionFailed);

        return Result.Success();
    }
}