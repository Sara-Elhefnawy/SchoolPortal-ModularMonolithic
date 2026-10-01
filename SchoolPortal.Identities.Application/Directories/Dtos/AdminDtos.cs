namespace SchoolPortal.Identities.Application.Directories.Dtos;

public sealed record UserDto(int Id, string Name, string Email, bool IsActive, IReadOnlyList<string> Roles);
public sealed record RoleDto(int Id, string Name, bool IsDeleted);
public sealed record PermissionDto(string Name);