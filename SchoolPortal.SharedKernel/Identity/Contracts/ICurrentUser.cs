namespace SchoolPortal.SharedKernel.Identity.Contracts;

public interface ICurrentUser
{
    int? UserId { get; }
    int? StudentId { get; }
    
    string? Name { get; }
    
    IReadOnlyList<string> Roles { get; }
    IReadOnlyList<string> Permissions { get; }
}