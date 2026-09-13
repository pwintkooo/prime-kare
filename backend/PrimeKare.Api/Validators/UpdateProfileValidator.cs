using FluentValidation;
using PrimeKare.Api.DTOs.Profile;

namespace PrimeKare.Api.Validators.Profile;

public class UpdateProfileValidator
    : AbstractValidator<UpdateProfileDto>
{
    public UpdateProfileValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(100)
            .WithMessage("Name must not exceed 100 characters");

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
    }
}