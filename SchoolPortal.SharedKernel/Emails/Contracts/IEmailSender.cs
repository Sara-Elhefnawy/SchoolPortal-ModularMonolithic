using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.SharedKernel.Emails.Contracts;

public interface IEmailSender
{
    Task<Result> SendResetPasswordEmailAsync(string toAddress, string resetLink, CancellationToken ct);

    Task<Result> SendConfirmEmailAsync(string toAddress, string confirmLink, CancellationToken ct);

    Task<Result> SendGradePublishedEmailAsync(string toAddress, string link, CancellationToken ct);
}