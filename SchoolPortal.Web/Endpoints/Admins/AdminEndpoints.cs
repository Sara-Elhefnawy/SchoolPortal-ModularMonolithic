using SchoolPortal.Identities.Application.Feature.Permissions.Command.SavePermissions;
using SchoolPortal.Identities.Application.Feature.Permissions.Query.GetAll;
using SchoolPortal.Identities.Application.Feature.Permissions.Query.GetRolePermissions;
using SchoolPortal.Identities.Application.Feature.Roles.Command.Create;
using SchoolPortal.Identities.Application.Feature.Roles.Query.GetAll;
using SchoolPortal.Identities.Application.Feature.Users.Command.Activate;
using SchoolPortal.Identities.Application.Feature.Users.Command.Create;
using SchoolPortal.Identities.Application.Feature.Users.Command.Deactivate;
using SchoolPortal.Identities.Application.Feature.Users.Query.GetAll;
using SchoolPortal.Identities.Application.Feature.Users.Query.GetById;
using SchoolPortal.SharedKernel.Identity;
using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.Web.Endpoints.Admins.Requests;

namespace SchoolPortal.Web.Endpoints.Admins;

public static class AdminEndpoints
{
    public static void MapAdminEndpoints(this IEndpointRouteBuilder app)
    {
        // WHY one group with a role rule: every admin URL is Admin-only, so the
        // rule is written once and no endpoint can forget it.
        var group = app.MapGroup("/api/admin").WithTags("Admin")
            .RequireAuthorization(policy => policy.RequireRole(Roles.Admin));

        group.MapGet("/users", GetUsers);
        group.MapGet("/users/{id:int}", GetUser);
        group.MapPost("/users", CreateUser);
        group.MapPost("/users/{id:int}/activate", ActivateUser);
        group.MapPost("/users/{id:int}/deactivate", DeactivateUser);

        group.MapGet("/roles", GetRoles);
        group.MapPost("/roles", CreateRole);
        group.MapGet("/roles/{id:int}/permissions", GetRolePermissions);
        group.MapPut("/roles/{id:int}/permissions", SaveRolePermissions);

        group.MapGet("/permissions", GetPermissions);
    }

    // ---------- Users ----------

    private static async Task<IResult> GetUsers(IMediator m, CancellationToken ct)
    {
        var r = await m.Send(new GetAllUsersQuery(), ct);

        return r.IsSuccess ? Results.Ok(r.Value) : Results.BadRequest(r.Error);
    }

    private static async Task<IResult> GetUser(int id, IMediator m, CancellationToken ct)
    {
        var r = await m.Send(new GetUserByIdQuery(id), ct);

        // WHY NotFound here: the only way this query fails is "no such user".
        return r.IsSuccess ? Results.Ok(r.Value) : Results.NotFound(r.Error);
    }

    private static async Task<IResult> CreateUser(CreateUserRequest body, IMediator m, CancellationToken ct)
    {
        // WHY map the request to a command: the request is what the browser sends,
        // the command is what the application understands. They can change separately.
        var r = await m.Send(new CreateUserCommand(body.Name, body.Email, body.Password, body.Role), ct);

        return r.IsSuccess
            ? Results.Created($"/api/admin/users/{r.Value}", r.Value)
            : Results.BadRequest(r.Error);
    }

    private static async Task<IResult> ActivateUser(int id, IMediator m, CancellationToken ct)
    {
        var r = await m.Send(new ActivateUserCommand(id), ct);

        return r.IsSuccess ? Results.NoContent() : Results.BadRequest(r.Error);
    }

    private static async Task<IResult> DeactivateUser(int id, IMediator m, CancellationToken ct)
    {
        var r = await m.Send(new DeactivateUserCommand(id), ct);

        return r.IsSuccess ? Results.NoContent() : Results.BadRequest(r.Error);
    }

    // ---------- Roles ----------

    private static async Task<IResult> GetRoles(IMediator m, CancellationToken ct)
    {
        var r = await m.Send(new GetAllRolesQuery(), ct);

        return r.IsSuccess ? Results.Ok(r.Value) : Results.BadRequest(r.Error);
    }

    private static async Task<IResult> CreateRole(CreateRoleRequest body, IMediator m, CancellationToken ct)
    {
        var r = await m.Send(new CreateRoleCommand(body.Name), ct);

        return r.IsSuccess
            ? Results.Created($"/api/admin/roles/{r.Value}", r.Value)
            : Results.BadRequest(r.Error);
    }

    // ---------- Permissions ----------

    private static async Task<IResult> GetPermissions(IMediator m, CancellationToken ct)
    {
        var r = await m.Send(new GetAllPermissionsQuery(), ct);

        return r.IsSuccess ? Results.Ok(r.Value) : Results.BadRequest(r.Error);
    }

    private static async Task<IResult> GetRolePermissions(int id, IMediator m, CancellationToken ct)
    {
        var r = await m.Send(new GetRolePermissionsQuery(id), ct);

        return r.IsSuccess ? Results.Ok(r.Value) : Results.BadRequest(r.Error);
    }

    private static async Task<IResult> SaveRolePermissions(int id, SaveRolePermissionsRequest body, IMediator m,
        CancellationToken ct)
    {
        // WHY id and list come separately: the id is in the URL, the list is in the body.
        var r = await m.Send(new SaveRolePermissionsCommand(id, body.Permissions), ct);

        return r.IsSuccess ? Results.NoContent() : Results.BadRequest(r.Error);
    }
}