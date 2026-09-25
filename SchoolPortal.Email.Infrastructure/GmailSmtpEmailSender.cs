using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using SchoolPortal.Email.Domain;
using SchoolPortal.SharedKernel.Emails.Contracts;
using SchoolPortal.SharedKernel.Results;
using SchoolPortal.SharedKernel.Emails;

namespace SchoolPortal.Email.Infrastructure;

// Note the extra dependency: IEmailTemplateRenderer. This class no longer
// knows or cares that the templates are .cshtml files rendered by Web -
// it just calls the abstraction, same as it doesn't know MailKit's
// internals belong to Gmail specifically.
public class GmailSmtpEmailSender(
    IOptions<EmailOptions> options,
    ILogger<GmailSmtpEmailSender> logger,
    IEmailTemplateRenderer templateRenderer) : IEmailSender
{
    public async Task<Result> SendResetPasswordEmailAsync(string toAddress, string resetLink, CancellationToken ct)
    {
        var html = await templateRenderer.RenderAsync("/Views/Shared/EmailTemplates/ResetPassword.cshtml",
            new ResetPasswordEmailModel { ResetLink = resetLink });
        return await SendAsync(toAddress, "Reset your SchoolPortal password", html, ct);
    }

    public async Task<Result> SendConfirmEmailAsync(string toAddress, string confirmLink, CancellationToken ct)
    {
        var html = await templateRenderer.RenderAsync("/Views/Shared/EmailTemplates/ConfirmEmail.cshtml",
            new ConfirmEmailEmailModel { ConfirmLink = confirmLink });
        return await SendAsync(toAddress, "Confirm your SchoolPortal account", html, ct);
    }

    public async Task<Result> SendGradePublishedEmailAsync(string toAddress, string link, CancellationToken ct)
    {
        var html = await templateRenderer.RenderAsync("/Views/Shared/EmailTemplates/GradePublished.cshtml",
            new GradePublishedEmailModel { Link = link });
        return await SendAsync(toAddress, "New grade published", html, ct);
    }

    private async Task<Result> SendAsync(string toAddress, string subject, string htmlBody, CancellationToken ct)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(options.Value.SenderDisplayName, options.Value.SenderAddress));
        message.To.Add(MailboxAddress.Parse(toAddress));
        message.Subject = subject;
        message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

        using var client = new SmtpClient();
        
        logger.LogInformation(
            "SMTP configuration: Sender={Sender}, Host={Host}, Port={Port}, AppPasswordConfigured={Configured}",
            options.Value.SenderAddress,
            options.Value.Host,
            options.Value.Port,
            !string.IsNullOrWhiteSpace(options.Value.AppPassword));

        try
        {
            await client.ConnectAsync(options.Value.Host, options.Value.Port, SecureSocketOptions.StartTls, ct);
            await client.AuthenticateAsync(options.Value.SmtpUsername, options.Value.AppPassword, ct);
            await client.SendAsync(message, ct);
            await client.DisconnectAsync(true, ct);
            return Result.Success();
        }
        catch (AuthenticationException ex)
        {
            logger.LogError(ex, "Gmail SMTP authentication failed for {Sender}", options.Value.SenderAddress);
            return Result.Failure(EmailErrors.EmailNotAuthenticated);
        }
        catch (SmtpCommandException ex)
        {
            logger.LogError(ex, "Gmail SMTP rejected the message to {Recipient}", toAddress);
            return Result.Failure(EmailErrors.EmailSendFailed);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected failure sending email to {Recipient}", toAddress);
            return Result.Failure(EmailErrors.EmailSendFailed);
        }
    }
}