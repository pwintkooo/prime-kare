using FluentValidation;
using PrimeKare.Api.DTOs.Contact;
using PrimeKare.Api.Templates;

namespace PrimeKare.Api.Services;

public class ContactService : IContactService
{
    private readonly IEmailService _emailService;
    private readonly IValidator<ContactRequestDto> _validator;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ContactService> _logger;

    public ContactService(
        IEmailService emailService,
        IValidator<ContactRequestDto> validator,
        IConfiguration configuration,
        ILogger<ContactService> logger)
    {
        _emailService = emailService;
        _validator = validator;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendContactMessageAsync(
        ContactRequestDto dto)
    {
        await _validator.ValidateAndThrowAsync(dto);

        var receiverEmail =
            _configuration["Contact:ReceiverEmail"]
            ?? throw new InvalidOperationException(
                "Contact receiver email is not configured.");

        var frontendUrl =
            _configuration["FrontendUrl"]
            ?? throw new InvalidOperationException(
                "Frontend URL is not configured.");

        // Email #1: notification to PrimeKare
        // This is the important email, so let failures propagate.
        var notificationBody =
            ContactEmailTemplate.BuildNotificationEmail(
                dto,
                frontendUrl);

        await _emailService.SendEmailAsync(
            receiverEmail,
            $"New enquiry from {dto.Name}",
            notificationBody,
            dto.Email);

        // Email #2: confirmation to customer
        // Failure here should not make the contact request fail.
        try
        {
            var confirmationBody =
                ContactEmailTemplate.BuildConfirmationEmail(
                    dto,
                    frontendUrl);

            await _emailService.SendEmailAsync(
                dto.Email,
                "Thanks for contacting PrimeKare",
                confirmationBody);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to send contact confirmation email to {Email}.",
                dto.Email);
        }
    }
}