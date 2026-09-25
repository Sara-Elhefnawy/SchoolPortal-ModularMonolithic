namespace SchoolPortal.SharedKernel.Emails;

public sealed class ResetPasswordEmailModel
{
    public string ResetLink { get; init; } = null!;
}