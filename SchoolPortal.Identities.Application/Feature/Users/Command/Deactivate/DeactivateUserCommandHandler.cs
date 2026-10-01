using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.Identities.Application.Directories;
using SchoolPortal.Identities.Domain.Users;
using SchoolPortal.SharedKernel.Identity.Contracts;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Users.Command.Deactivate;

public sealed class DeactivateUserCommandHandler(
    IIdentityDirectory directory,
    ICurrentUser currentUser) 
    : IRequestHandler<DeactivateUserCommand, Result>
{
    public async Task<Result> Handle(DeactivateUserCommand request, CancellationToken ct)
    {
        var user = await directory.GetUserByIdAsync(request.Id, ct);
        if (user is null || user.IsDeleted)
            return Result.Failure(UserErrors.NotFound);
        
        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;
        user.UpdatedByUserId = currentUser.UserId;
        
        await directory.SaveChangesAsync(ct);
        
        return Result.Success();
    }
}