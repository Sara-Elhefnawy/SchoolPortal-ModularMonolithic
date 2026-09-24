namespace SchoolPortal.SharedKernel.Identity.Contracts;

public interface ICurrentUser
{
    int? UserId { get; }
}