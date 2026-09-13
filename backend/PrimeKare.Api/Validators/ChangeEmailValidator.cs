using FluentValidation;
using PrimeKare.Api.DTOs.Profile;

namespace PrimeKare.Api.Validators.Profile;

public class ChangeEmailValidator
    : AbstractValidator<ChangeEmailDto>
{
    public ChangeEmailValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty()
            .WithMessage("Current Password is required.");

        RuleFor(x => x.NewEmail)
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
    }
}