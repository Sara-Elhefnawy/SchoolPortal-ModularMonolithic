using MediatR;
using SchoolPortal.Identities.Application.Feature.Roles.Command.Create;
using SchoolPortal.Identities.Application.Feature.Roles.Command.Delete;
using SchoolPortal.Identities.Application.Feature.Roles.Command.Restore;
using SchoolPortal.Identities.Application.Feature.Roles.Command.Update;
using SchoolPortal.Identities.Application.Feature.Roles.Command.UpdatePermissions;
using SchoolPortal.Identities.Application.Feature.Roles.Query.GetAll;
using SchoolPortal.Identities.Application.Feature.Roles.Query.GetById;
using SchoolPortal.Identities.Application.Feature.Permissions.Query.GetRolePermissions;
using SchoolPortal.SharedKernel.Identity;
using SchoolPortal.Web.Endpoints.Identity.Roles.Requests;

namespace SchoolPortal.Web.Endpoints.Identity.Roles;

public static class RoleEndpoints
{
    public static void MapRoleEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin/roles");

        group.MapGet("/", GetRoles)
            .RequireAuthorization(Permissions.Users.View);

        group.MapGet("/{id:int}", GetRole)
            .RequireAuthorization(Permissions.Users.View);

        group.MapPost("/", CreateRole)
            .RequireAuthorization(Permissions.Users.Create);

        group.MapPut("/{id:int}", UpdateRole)
            .RequireAuthorization(Permissions.Users.Update);

        group.MapDelete("/{id:int}", DeleteRole)
            .RequireAuthorization(Permissions.Users.Delete);

        group.MapPost("/{id:int}/restore", RestoreRole)
            .RequireAuthorization(Permissions.Users.Update);

        group.MapGet("/{id:int}/permissions", GetRolePermissions)
            .RequireAuthorization(Permissions.Users.View);

        group.MapPut("/{id:int}/permissions", UpdateRolePermissions)
            .RequireAuthorization(Permissions.Users.Update);
    }

    private static async Task<IResult> GetRoles(
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(
            new GetAllRolesQuery(),
            ct);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new
            {
                error = result.Error
            });
    }

    private static async Task<IResult> GetRole(
        int id,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(
            new GetRoleByIdQuery(id),
            ct);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.NotFound(new
            {
                error = result.Error
            });
    }

    private static async Task<IResult> CreateRole(
        CreateRoleRequest request,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new CreateRoleCommand(
            request.Name);

        var result = await mediator.Send(command, ct);

        return result.IsSuccess
            ? Results.Created(
                "/api/admin/roles",
                new { message = "Role created successfully." })
            : Results.BadRequest(new
            {
                error = result.Error
            });
    }

    private static async Task<IResult> UpdateRole(
        int id,
        UpdateRoleRequest request,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new UpdateRoleCommand(
            id,
            request.Name);

        var result = await mediator.Send(command, ct);

        return result.IsSuccess
            ? Results.Ok(new
            {
                message = "Role updated successfully."
            })
            : Results.BadRequest(new
            {
                error = result.Error
            });
    }

    private static async Task<IResult> DeleteRole(
        int id,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(
            new DeleteRoleCommand(id),
            ct);

        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(new
            {
                error = result.Error
            });
    }

    private static async Task<IResult> RestoreRole(
        int id,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(
            new RestoreRoleCommand(id),
            ct);

        return result.IsSuccess
            ? Results.Ok(new
            {
                message = "Role restored successfully."
            })
            : Results.BadRequest(new
            {
                error = result.Error
            });
    }

    private static async Task<IResult> GetRolePermissions(
        int id,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(
            new GetRolePermissionsQuery(id),
            ct);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new
            {
                error = result.Error
            });
    }

    private static async Task<IResult> UpdateRolePermissions(
        int id,
        UpdateRolePermissionsRequest request,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new UpdateRolesPermissionsCommand(
            id,
            request.PermissionIds);

        var result = await mediator.Send(command, ct);

        return result.IsSuccess
            ? Results.Ok(new
            {
                message =
                    "Role permissions updated successfully."
            })
            : Results.BadRequest(new
            {
                error = result.Error
            });
    }
}