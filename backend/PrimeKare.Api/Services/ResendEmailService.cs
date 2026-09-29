using Resend;
using PrimeKare.Api.DTOs.Email;

namespace PrimeKare.Api.Services;

public class ResendEmailService : IEmailService
{
    private readonly IResend _resend;
    private readonly IConfiguration _configuration;

    public ResendEmailService(
        IResend resend,
        IConfiguration configuration)
    {
        _resend = resend;
        _configuration = configuration;
    }

    public async Task SendEmailAsync(
        string to,
        string subject,
        string htmlBody,
        string? replyTo = null,
        EmailFileAttachment? attachment = null)
    {
        var fromEmail =
            _configuration["Resend:FromEmail"]
            ?? throw new InvalidOperationException(
                "Resend sender email is not configured.");

        var message = new EmailMessage
        {
            From = fromEmail,
            Subject = subject,
            HtmlBody = htmlBody,
            To = { to }
        };

        if (!string.IsNullOrWhiteSpace(replyTo))
        {
            message.ReplyTo = replyTo;
        }

        if (attachment != null)
        {
            message.Attachments =
            [
                new EmailAttachment
                {
                    Filename = attachment.FileName,
                    Content = attachment.Content,
                    ContentType = attachment.ContentType
                }
            ];
        }

        await _resend.EmailSendAsync(message);
    }
}