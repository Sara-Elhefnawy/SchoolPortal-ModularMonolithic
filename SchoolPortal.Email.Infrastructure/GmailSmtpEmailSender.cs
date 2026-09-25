using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using SchoolPortal.Email.Domain;
using SchoolPortal.SharedKernel.Emails;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Email.Infrastructure;

public class GmailSmtpEmailSender(IOptions<EmailOptions> options, ILogger<GmailSmtpEmailSender> logger) : IEmailSender
{
    public Task<Result> SendForgotPasswordEmailAsync(string toAddress, string resetLink, CancellationToken ct)
    {
        var html = LoadTemplate("ForgotPassword.html").Replace("{{Link}}", resetLink);

        return SendAsync(toAddress, "Reset your SchoolPortal password", html, ct);
    }

    public Task<Result> SendConfirmEmailAsync(string toAddress, string confirmLink, CancellationToken ct)
    {
        var html = LoadTemplate("ConfirmEmail.html").Replace("{{Link}}", confirmLink);

        return SendAsync(toAddress, "Confirm your SchoolPortal account", html, ct);
    }

    public Task<Result> SendGradePublishedEmailAsync(string toAddress, string link,
        CancellationToken ct)
    {
        var html = LoadTemplate("GradePublished.html").Replace("{{Link}}", link);

        return SendAsync(toAddress, "New grade published", html, ct);
    }

    // Templates/*.html must be marked <EmbeddedResource Include="Templates\*.html" />
    // Baked into the DLL this way so there's no separate "did the deploy copy the template files too?" failure mode to worry about.
    private static string LoadTemplate(string fileName)
    {
        var assembly = typeof(GmailSmtpEmailSender).Assembly;
        var resourceName = $"{assembly.GetName().Name}.Templates.{fileName}";

        using var stream = assembly.GetManifestResourceStream(resourceName) ?? throw new InvalidOperationException(
            $"Email template '{fileName}' not found as embedded resource '{resourceName}'. " +
            "Check the .csproj EmbeddedResource glob and that the file name matches exactly.");

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    private async Task<Result> SendAsync(string toAddress, string subject, string htmlBody, CancellationToken ct)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(options.Value.SenderDisplayName, options.Value.SenderAddress));
        message.To.Add(MailboxAddress.Parse(toAddress));
        message.Subject = subject;
        message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

        using var client = new SmtpClient();

        try
        {
            await client.ConnectAsync(options.Value.Host, options.Value.Port, SecureSocketOptions.StartTls, ct);
            await client.AuthenticateAsync(options.Value.SenderAddress, options.Value.AppPassword, ct);
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