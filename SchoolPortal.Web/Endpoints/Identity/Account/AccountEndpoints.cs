using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.Identities.Application.Feature.Auth.Command.ChangePassword;
using SchoolPortal.Identities.Application.Feature.Auth.Command.ConfirmEmail;
using SchoolPortal.Identities.Application.Feature.Auth.Command.ForgotPassword;
using SchoolPortal.Identities.Application.Feature.Auth.Command.Login;
using SchoolPortal.Identities.Application.Feature.Auth.Command.Logout;
using SchoolPortal.Identities.Application.Feature.Auth.Command.ResetPassword;
using SchoolPortal.Identities.Application.Feature.Auth.Command.Signup;
using SchoolPortal.Web.Endpoints.Identity.Account.Requests;

namespace SchoolPortal.Web.Endpoints.Identity.Account;

public static class AccountEndpoints
{
    public static void MapAccountEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/account");

        group.MapPost("/login", Login);
        group.MapPost("/signup", Signup);
        group.MapPost("/logout", Logout)
            .RequireAuthorization();

        group.MapPost("/change-password", ChangePassword)
            .RequireAuthorization();

        group.MapPost("/forgot-password", ForgotPassword);

        group.MapPost("/reset-password", ResetPassword);
        
        group.MapPost("/confirm-email", ConfirmEmail);

        group.MapPost("/me", Me);
    }

    private static async Task<IResult> ConfirmEmail(
        ConfirmEmailCommand request,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(request, ct);

        return result.IsSuccess
            ? Results.Ok(new { message = "Email confirmed." })
            : Results.BadRequest(new { error = "This confirmation link is invalid or has expired." });
    }

    private static async Task<IResult> Signup(
        SignupRequest request,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new SignupCommand(
            request.Name,
            request.Email,
            request.Password,
            request.ConfirmPassword);

        var result = await mediator.Send(command, ct);

        return result.IsSuccess ? Results.Ok() : Results.BadRequest(new { error = result.Error });
    }

    private static async Task<IResult> Login(
        LoginRequest request,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new LoginCommand(
            request.Email,
            request.Password);

        var result = await mediator.Send(command, ct);

        return result.IsSuccess
            ? Results.Ok(new { message = "Login successful." })
            : Results.Unauthorized();
    }

    private static async Task<IResult> Logout(
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(
            new LogoutCommand(),
            ct);

        return result.IsSuccess
            ? Results.Ok(new { message = "Logged out successfully." })
            : Results.BadRequest(new { error = result.Error });
    }

    private static async Task<IResult> ChangePassword(
        ChangePasswordRequest request,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new ChangePasswordCommand(
            request.Email,
            request.Password,
            request.NewPassword);

        var result = await mediator.Send(command, ct);

        return result.IsSuccess
            ? Results.Ok(new { message = "Password changed successfully." })
            : Results.BadRequest(new { error = result.Error });
    }

    private static async Task<IResult> ForgotPassword(
        ForgotPasswordRequest request,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new ForgotPasswordCommand(
            request.Email);

        var result = await mediator.Send(command, ct);

        // Deliberately don't reveal whether the account exists.
        return result.IsSuccess
            ? Results.Ok(new
            {
                message =
                    "If an account exists for this email, " +
                    "password reset instructions will be sent."
            })
            : Results.BadRequest(new { error = result.Error });
    }

    private static async Task<IResult> ResetPassword(ResetPasswordRequest request, IMediator mediator,
        CancellationToken ct)
    {
        var command = new ResetPasswordCommand(
            request.Email, request.Token, request.NewPassword, request.ConfirmPassword);

        var result = await mediator.Send(command, ct);

        return result.IsSuccess
            ? Results.Ok(new { message = "Password has been reset successfully." })
            : Results.BadRequest(new { error = result.Error });
    }

    private static async Task<IResult> Me(
        ForgotPasswordRequest request,
        IMediator mediator,
        CancellationToken ct)
    {
        /*"id": 1,
        "name": "Sara",
        "email": "sara@example.com",
        "roles": [
        "Admin"
            ],
        "permissions": [
        "Users.View",
        "Users.Create",
        "Users.Update",
        "Users.Delete",
        "Grades.View",
        "Grades.Create",
        "Grades.Update",
        "Grades.Unlock",
        "Attendance.View",
        "Attendance.Mark",
        "Attendance.Update",
        "Students.View",
        "Students.Create",
        "Students.Update",
        "Students.Delete"
            ]*/
        
        var command = new ForgotPasswordCommand(
            request.Email);

        var result = await mediator.Send(command, ct);

        // Deliberately don't reveal whether the account exists.
        return result.IsSuccess
            ? Results.Ok(new
            {
                message =
                    "If an account exists for this email, " +
                    "password reset instructions will be sent."
            })
            : Results.BadRequest(new { error = result.Error });
    }
}