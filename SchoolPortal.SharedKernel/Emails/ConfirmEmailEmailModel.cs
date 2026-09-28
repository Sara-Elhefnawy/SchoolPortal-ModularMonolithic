namespace SchoolPortal.SharedKernel.Emails;

public sealed class ConfirmEmailEmailModel
{
    public string ConfirmLink { get; init; } = null!;
}