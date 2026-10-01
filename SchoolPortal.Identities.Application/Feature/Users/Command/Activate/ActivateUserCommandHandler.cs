using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.Identities.Application.Directories;
using SchoolPortal.Identities.Domain.Users;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Users.Command.Activate;

public sealed class ActivateUserCommandHandler(
    IIdentityDirectory directory)
    : IRequestHandler<ActivateUserCommand, Result>
{
    public async Task<Result> Handle(ActivateUserCommand request, CancellationToken ct)
    {
        var user = await directory.GetUserByIdAsync(request.Id, ct);
        if (user is null || user.IsDeleted)
            return Result.Failure(UserErrors.NotFound);

        // ADJUST: if ApplicationUser exposes Activate()/Deactivate() methods
        // instead of a public setter, call those. Rules belong on the entity.
        user.IsActive = true;

        await directory.SaveChangesAsync(ct);
        
        return Result.Success();
    }
}