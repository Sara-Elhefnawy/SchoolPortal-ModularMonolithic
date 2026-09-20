namespace SchoolPortal.Identities.Application.Feature.Permissions.Query.GetRolePermissions;

public sealed record GetRolePermissionsQueryDto(
    int RoleId,
    string RoleName,
    int PermissionId,
    string PermissionName);