using MediatR;
using Microsoft.AspNetCore.Identity;
using SchoolPortal.Identities.Domain.Roles;
using SchoolPortal.SharedKernel.Identity.Contracts;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Roles.Command.Create;

public sealed class CreateRoleCommandHandler(
    RoleManager<ApplicationRole> roleManager,
    ICurrentUser currentUser)
    : IRequestHandler<CreateRoleCommand, Result>
{
    public async Task<Result> Handle(CreateRoleCommand request, CancellationToken ct)
    {
        var existingRole =
            await roleManager.FindByNameAsync(request.Name);
        if (existingRole is not null)
            return Result.Failure(RoleErrors.AlreadyExists);
        
        var role = new ApplicationRole
        {
            Name = request.Name,
            CreatedAt =  DateTime.UtcNow,
            CreatedByUserId =  currentUser.UserId
        };

        var result = await roleManager.CreateAsync(role);
        if (!result.Succeeded)
            return Result.Failure(RoleErrors.CreationFailed);

        return Result.Success();
    }
}