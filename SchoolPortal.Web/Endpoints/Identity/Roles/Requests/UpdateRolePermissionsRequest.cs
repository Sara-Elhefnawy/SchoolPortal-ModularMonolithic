namespace SchoolPortal.Web.Endpoints.Identity.Roles.Requests;

public sealed record UpdateRolePermissionsRequest(IReadOnlyList<int> PermissionIds);