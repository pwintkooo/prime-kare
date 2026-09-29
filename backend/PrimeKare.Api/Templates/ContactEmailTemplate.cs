using System.Net;
using PrimeKare.Api.DTOs.Contact;

namespace PrimeKare.Api.Templates;

public static class ContactEmailTemplate
{
    public static string BuildNotificationEmail(
        ContactRequestDto dto,
        string frontendUrl)
    {
        var name = WebUtility.HtmlEncode(dto.Name);
        var email = WebUtility.HtmlEncode(dto.Email);
        var phone = WebUtility.HtmlEncode(dto.Phone);

        var message = WebUtility
            .HtmlEncode(dto.Message)
            .Replace("\n", "<br>");

        var content = $"""
            <h2 style="margin-top: 0;">
                New PrimeKare Contact Message
            </h2>

            <p>
                <strong>Name:</strong> {name}
            </p>

            <p>
                <strong>Email:</strong> {email}
            </p>

            <p>
                <strong>Phone:</strong> {phone}
            </p>

            <p>
                <strong>Message:</strong>
            </p>

            <p>{message}</p>
            """;

        return EmailLayoutTemplate.Build(
            frontendUrl,
            content);
    }

    public static string BuildConfirmationEmail(
        ContactRequestDto dto,
        string frontendUrl)
    {
        var name = WebUtility.HtmlEncode(dto.Name);

        var content = $"""
            <h2 style="margin-top: 0;">
                Thank you for contacting PrimeKare
            </h2>

            <p>Hi {name},</p>

            <p>
                Thank you for getting in touch with PrimeKare.
                We've received your message and will get back
                to you as soon as possible.
            </p>

            <p style="margin-top: 32px;">
                Regards,<br>
                <strong>PrimeKare</strong>
            </p>
            """;

        return EmailLayoutTemplate.Build(
            frontendUrl,
            content);
    }
}