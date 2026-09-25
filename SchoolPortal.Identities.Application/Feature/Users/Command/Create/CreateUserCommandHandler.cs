using SchoolPortal.SharedKernel.Mediator;
using Microsoft.AspNetCore.Identity;
using SchoolPortal.Identities.Domain.Users;
using SchoolPortal.SharedKernel.Domain.Events.Identity.Users;
using SchoolPortal.SharedKernel.Identity.Contracts;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Users.Command.Create;

public sealed class CreateUserCommandHandler(
    UserManager<ApplicationUser> userManager, 
    IPublisher publisher,
    ICurrentUser currentUser) 
    : IRequestHandler<CreateUserCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateUserCommand request, CancellationToken ct)
    {
        // Creates the user and hashes the password through ASP.NET Core Identity.
        var user = new ApplicationUser
        {
            Email = request.Email,
            UserName = request.Email,
            Name = request.Name,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = currentUser.UserId
        };
        
        var userResult = await userManager.CreateAsync(user, request.Password);
        if (!userResult.Succeeded)
        {
            foreach (var error in userResult.Errors)
            {
                Console.WriteLine($"Code: {error.Code}, Description: {error.Description}");
            }
            return Result<int>.Failure(UserErrors.CreationFailed);
        }
        
        // Assign the requested role after the user has been successfully created.
        var roleResult = await userManager.AddToRoleAsync(user, request.RoleName);
        if (!roleResult.Succeeded)
        {
            // If role assignment fails, remove the user so we don't leave an incomplete Identity record behind.
            await userManager.DeleteAsync(user);
            return Result<int>.Failure(UserErrors.RoleAssignmentFailed);
        }

        // Notify the rest of the modular monolith only after the Identity operations succeeded.
        await publisher.Publish(new UserCreated(user.Id, request.RoleName), ct);

        return Result<int>.Success(user.Id);
    }
}