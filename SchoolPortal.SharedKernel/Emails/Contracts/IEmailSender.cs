using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.SharedKernel.Emails;

public interface IEmailSender
{
    Task<Result> SendForgotPasswordEmailAsync(string toAddress, string resetLink, CancellationToken ct);

    Task<Result> SendConfirmEmailAsync(string toAddress, string confirmLink, CancellationToken ct);

    Task<Result> SendGradePublishedEmailAsync(string toAddress, string link, CancellationToken ct);
}