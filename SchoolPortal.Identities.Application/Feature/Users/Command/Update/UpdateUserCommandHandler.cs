using SchoolPortal.SharedKernel.Mediator;
using Microsoft.AspNetCore.Identity;
using SchoolPortal.Identities.Domain.Users;
using SchoolPortal.SharedKernel.Identity.Contracts;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Users.Command.Update;

public sealed class UpdateUserCommandHandler (
    UserManager<ApplicationUser> userManager,
    ICurrentUser currentUser)
    : IRequestHandler<UpdateUserCommand, Result>
{
    public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Id.ToString());
        if (user is null || user.IsDeleted)
            return Result.Failure(UserErrors.NotFound);
        
        if (request.PhoneNumber is not null) 
            user.PhoneNumber = request.PhoneNumber;
        
        if (request.Name is not null) 
            user.Name = request.Name;
        
        user.UpdatedAt = DateTime.UtcNow;
        user.UpdatedByUserId = currentUser.UserId;

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return Result.Failure(UserErrors.UpdateFailed);

        return Result.Success();
    }
}