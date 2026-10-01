using SchoolPortal.SharedKernel.Mediator;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SchoolPortal.Identities.Domain;
using SchoolPortal.Identities.Domain.Users;
using SchoolPortal.SharedKernel.Emails.Contracts;
using SchoolPortal.SharedKernel.Emails.Options;
using SchoolPortal.SharedKernel.Events.Identity.Auth;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Auth.Command.Signup;

public sealed class SignupCommandHandler(
    UserManager<ApplicationUser> userManager,
    RoleManager<Domain.Roles.ApplicationRole> roleManager,
    IEmailSender emailSender,
    IPublisher publisher,
    IOptions<AppOptions> appOptions) : IRequestHandler<SignupCommand, Result>
{
    public async Task<Result> Handle(SignupCommand request, CancellationToken ct)
    {
        if (request.Password != request.ConfirmPassword)
            return Result.Failure(AuthErrors.PasswordsDoNotMatch);

        var existingUser = await userManager.FindByEmailAsync(request.Email);
        if (existingUser is not null)
            return existingUser.IsDeleted
                ? Result.Failure(AuthErrors.AccountExists)
                : Result.Failure(AuthErrors.EmailAlreadyExists);

        var studentRole = await roleManager.FindByNameAsync(SharedKernel.Identity.Roles.Student);
        if (studentRole is null || studentRole.IsDeleted)
            return Result.Failure(AuthErrors.DefaultRoleNotAvailable);

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            Name = request.Name,
            IsActive = true,
            EmailConfirmed = false
        };

        var createResult = await userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            var error = createResult.Errors.FirstOrDefault();

            return Result.Failure(error is null ? AuthErrors.SignupFailed : IdentityErrorMapper.Map(error));
        }

        var roleResult = await userManager.AddToRoleAsync(user, SharedKernel.Identity.Roles.Student);
        if (!roleResult.Succeeded)
        {
            await userManager.DeleteAsync(user);

            return Result.Failure(AuthErrors.RoleAssignmentFailed);
        }

        // Notify other modules that a new Student-role user
        // has successfully registered.
        await publisher.Publish(
            new UserSignedUp(user.Id, user.Name, user.Email!, request.PhoneNumber, request.DateOfBirth, request.Gender,
                request.AdmissionDate, request.City, request.State, request.ZipCode, request.Country,
                request.BuildingNumber), ct);
        
        // Generate the confirmation token and send the email.
        var confirmToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var link = $"{appOptions.Value.PublicBaseUrl}/confirm-email" +
                   $"?userId={user.Id}&token={Uri.EscapeDataString(confirmToken)}";
        var sendResult = await emailSender.SendConfirmEmailAsync(user.Email!, link, ct);
        if (sendResult.IsFailure)
            return sendResult;

        // Do not sign the user in here. The user must confirm the email first, then explicitly log in.

        return Result.Success();
    }
}