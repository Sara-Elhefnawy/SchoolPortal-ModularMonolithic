namespace SchoolPortal.SharedKernel.Identity.Users;

public interface ICurrentUser
{
    int? UserId { get; }
}