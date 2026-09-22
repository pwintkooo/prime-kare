using FluentValidation;
using PrimeKare.Api.DTOs.Contact;

namespace PrimeKare.Api.Validators;

public class ContactMessageValidator
    : AbstractValidator<ContactRequestDto>
{
    public ContactMessageValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Email format is invalid.")
            .Matches(@"^[^@\s]+@[^@\s]+\.[A-Za-z]{2,}$")
            .WithMessage(
                "Email must contain a valid domain."
            )
            .MaximumLength(254)
            .WithMessage(
                "Email must not exceed 254 characters."
            );

        RuleFor(x => x.Phone)
            .Matches(@"^\+?[0-9\s\-()]+$")
            .WithMessage(
                "Phone number format is invalid."
            )
            .MaximumLength(20)
            .WithMessage(
                "Phone number must not exceed 20 characters."
            )
            .When(x =>
                !string.IsNullOrWhiteSpace(x.Phone)
            );

        RuleFor(x => x.Message)
            .NotEmpty()
            .WithMessage("Message is required.")
            .MaximumLength(2000)
            .WithMessage("Message must not exceed 2000 characters.");
    }
}