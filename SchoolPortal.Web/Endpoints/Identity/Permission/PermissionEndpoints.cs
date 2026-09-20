using MediatR;
using SchoolPortal.Identities.Application.Feature.Permissions.Query.GetAll;
using SchoolPortal.SharedKernel.Identity;

namespace SchoolPortal.Web.Endpoints.Identity.Permission;

public static class PermissionEndpoints
{
    public static void MapPermissionEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin/permissions");

        group.MapGet("/", GetPermissions)
            .RequireAuthorization(Permissions.Users.View);
    }

    private static async Task<IResult> GetPermissions(
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(
            new GetAllPermissionsQuery(),
            ct);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new { error = result.Error });
    }
}