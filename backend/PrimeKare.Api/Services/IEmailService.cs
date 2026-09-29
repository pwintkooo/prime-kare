using PrimeKare.Api.DTOs.Email;

namespace PrimeKare.Api.Services;

public interface IEmailService
{
    Task SendEmailAsync(
        string to,
        string subject,
        string htmlBody,
        string? replyTo = null,
        EmailFileAttachment? attachment = null);
}