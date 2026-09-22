using Resend;

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
        string? replyTo = null)
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

        await _resend.EmailSendAsync(message);
    }
}