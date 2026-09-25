using SchoolPortal.SharedKernel.Mediator;
using Microsoft.AspNetCore.Identity;
using SchoolPortal.Identities.Domain.Users;
using SchoolPortal.SharedKernel.Identity.Contracts;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Users.Command.Restore;

public sealed class RestoreUserCommandHandler(
    UserManager<ApplicationUser>  userManager,
    ICurrentUser currentUser) 
    : IRequestHandler<RestoreUserCommand, Result>
{
    public async Task<Result> Handle(RestoreUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Id.ToString());
        if (user is null)
            return Result.Failure(UserErrors.NotFound);
        
        user.IsDeleted = false;
        user.IsActive = true;
        user.RestoredAt = DateTime.UtcNow;
        user.RestoredByUserId = currentUser.UserId;

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return Result.Failure(UserErrors.RestorationFailed);

        return Result.Success();
    }
}