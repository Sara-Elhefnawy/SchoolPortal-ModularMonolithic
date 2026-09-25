using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.Identities.Application.Feature.Users.Command.Activate;
using SchoolPortal.Identities.Application.Feature.Users.Command.AssignRole;
using SchoolPortal.Identities.Application.Feature.Users.Command.Create;
using SchoolPortal.Identities.Application.Feature.Users.Command.Deactivate;
using SchoolPortal.Identities.Application.Feature.Users.Command.Delete;
using SchoolPortal.Identities.Application.Feature.Users.Command.Restore;
using SchoolPortal.Identities.Application.Feature.Users.Command.Update;
using SchoolPortal.Identities.Application.Feature.Users.Query.GetAll;
using SchoolPortal.Identities.Application.Feature.Users.Query.GetById;
using SchoolPortal.SharedKernel.Identity;
using SchoolPortal.Web.Endpoints.Identity.Users.Requests;

namespace SchoolPortal.Web.Endpoints.Identity.Users;

public static class UserEndpoints
{
    public static void MapIdentityEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin/users");

        group.MapGet("/", GetUsers)
            .RequireAuthorization(Permissions.Users.View);

        group.MapGet("/{id:int}", GetUser)
            .RequireAuthorization(Permissions.Users.View);

        group.MapPost("/", CreateUser)
            .RequireAuthorization(Permissions.Users.Create);

        group.MapPut("/{id:int}", UpdateUser)
            .RequireAuthorization(Permissions.Users.Update);

        group.MapDelete("/{id:int}", DeleteUser)
            .RequireAuthorization(Permissions.Users.Delete);

        group.MapPost("/{id:int}/restore", RestoreUser)
            .RequireAuthorization(Permissions.Users.Update);

        group.MapPost("/{id:int}/deactivate", DeactivateUser)
            .RequireAuthorization(Permissions.Users.Update);

        group.MapPost("/{id:int}/activate", ActivateUser)
            .RequireAuthorization(Permissions.Users.Update);

        group.MapPost("/{id:int}/roles", AssignRole)
            .RequireAuthorization(Permissions.Users.Update);
    }

    private static async Task<IResult> GetUsers(
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(
            new GetAllUsersQuery(),
            ct);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new { error = result.Error });
    }

    private static async Task<IResult> GetUser(
        int id,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(
            new GetUserByIdQuery(id),
            ct);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.NotFound(new { error = result.Error });
    }

    private static async Task<IResult> CreateUser(
        CreateUserRequest request,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new CreateUserCommand(
            request.Email,
            request.Password,
            request.Name,
            request.Role);

        var result = await mediator.Send(command, ct);

        return result.IsSuccess
            ? Results.Created(
                $"/api/admin/users/{result.Value}",
                new { id = result.Value })
            : Results.BadRequest(new { error = result.Error });
    }

    private static async Task<IResult> UpdateUser(
        int id,
        UpdateUserRequest request,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new UpdateUserCommand(
            id,
            request.Email,
            request.Name,
            request.PhoneNumber);

        var result = await mediator.Send(command, ct);

        return result.IsSuccess
            ? Results.Ok(new
            {
                message = "User updated successfully."
            })
            : Results.BadRequest(new
            {
                error = result.Error
            });
    }

    private static async Task<IResult> DeleteUser(
        int id,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(
            new DeleteUserCommand(id),
            ct);

        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(new
            {
                error = result.Error
            });
    }

    private static async Task<IResult> RestoreUser(
        int id,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(
            new RestoreUserCommand(id),
            ct);

        return result.IsSuccess
            ? Results.Ok(new
            {
                message = "User restored successfully."
            })
            : Results.BadRequest(new
            {
                error = result.Error
            });
    }

    private static async Task<IResult> DeactivateUser(
        int id,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(
            new DeactivateUserCommand(id),
            ct);

        return result.IsSuccess
            ? Results.Ok(new
            {
                message = "User deactivated successfully."
            })
            : Results.BadRequest(new
            {
                error = result.Error
            });
    }

    private static async Task<IResult> ActivateUser(
        int id,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(
            new ActivateUserCommand(id),
            ct);

        return result.IsSuccess
            ? Results.Ok(new
            {
                message = "User activated successfully."
            })
            : Results.BadRequest(new
            {
                error = result.Error
            });
    }

    private static async Task<IResult> AssignRole(
        int id,
        AssignRoleRequest request,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new AssignRoleCommand(
            id,
            request.RoleName);

        var result = await mediator.Send(command, ct);

        return result.IsSuccess
            ? Results.Ok(new
            {
                message = "Role assigned successfully."
            })
            : Results.BadRequest(new
            {
                error = result.Error
            });
    }
}