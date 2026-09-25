using SchoolPortal.SharedKernel.Mediator;
using Microsoft.AspNetCore.Identity;
using SchoolPortal.Identities.Domain.Users;
using SchoolPortal.SharedKernel.Identity.Contracts;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Users.Command.Activate;

public sealed class ActivateUserCommandHandler (
    UserManager<ApplicationUser>  userManager,
    ICurrentUser currentUser)
    : IRequestHandler<ActivateUserCommand, Result>
{
    public async Task<Result> Handle(ActivateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Id.ToString());
        if (user is null || user.IsDeleted)
            return Result.Failure(UserErrors.NotFound);
        
        user.IsActive = true;
        user.UpdatedAt = DateTime.UtcNow;
        user.UpdatedByUserId = currentUser.UserId;

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return Result.Failure(UserErrors.UpdateFailed);
        
        return Result.Success();
    }
}