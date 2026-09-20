using MediatR;
using Microsoft.AspNetCore.Identity;
using SchoolPortal.Identities.Domain.Users;
using SchoolPortal.SharedKernel.Identity.Users;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Users.Command.Delete;

public sealed class DeleteUserCommandHandler(
    UserManager<ApplicationUser>  userManager,
    ICurrentUser currentUser) 
    : IRequestHandler<DeleteUserCommand, Result>
{
    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Id.ToString());
        if (user is null || user.IsDeleted)
            return Result.Failure(UserErrors.NotFound);
        
        user.IsDeleted = true;
        user.IsActive = false;
        user.DeletedAt = DateTime.UtcNow;
        user.DeletedByUserId = currentUser.UserId;

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return Result.Failure(UserErrors.DeletionFailed);

        return Result.Success();
    }
}