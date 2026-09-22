using System.Net;
using FluentValidation;
using PrimeKare.Api.DTOs.Contact;

namespace PrimeKare.Api.Services;

public class ContactService : IContactService
{
    private readonly IEmailService _emailService;
    private readonly IValidator<ContactRequestDto> _validator;
    private readonly IConfiguration _configuration;

    public ContactService(
        IEmailService emailService,
        IValidator<ContactRequestDto> validator,
        IConfiguration configuration)
    {
        _emailService = emailService;
        _validator = validator;
        _configuration = configuration;
    }

    public async Task SendContactMessageAsync(
        ContactRequestDto dto)
    {
        await _validator.ValidateAndThrowAsync(dto);

        var receiverEmail =
            _configuration["Contact:ReceiverEmail"]
            ?? throw new InvalidOperationException(
                "Contact receiver email is not configured.");

        var name = WebUtility.HtmlEncode(dto.Name);
        var email = WebUtility.HtmlEncode(dto.Email);
        var phone = WebUtility.HtmlEncode(dto.Phone);
        var message = WebUtility.HtmlEncode(dto.Message)
            .Replace("\n", "<br>");

        // Email #1: notification to you
        var notificationBody = $"""
            <h2>New PrimeKare Contact Message</h2>

            <p><strong>Name:</strong> {name}</p>
            <p><strong>Email:</strong> {email}</p>
            <p><strong>Phone:</strong> {phone}</p>

            <p><strong>Message:</strong></p>
            <p>{message}</p>
            """;

        await _emailService.SendEmailAsync(
            receiverEmail,
            $"New enquiry from {dto.Name}",
            notificationBody,
            dto.Email
        );

        // Email #2: automatic confirmation to customer
        var confirmationBody = $"""
            <h2>Thank you for contacting PrimeKare</h2>

            <p>Hi {name},</p>

            <p>
                Thank you for getting in touch with PrimeKare.
                We've received your message and will get back
                to you as soon as possible.
            </p>

            <p>
                Regards,<br>
                PrimeKare
            </p>
            """;

        await _emailService.SendEmailAsync(
            dto.Email,
            "Thanks for contacting PrimeKare",
            confirmationBody
        );
    }
}