using FluentValidation;
using PrimeKare.Api.DTOs.Auth;

namespace PrimeKare.Api.Validators;

public class ForgotPasswordValidator
    : AbstractValidator<ForgotPasswordRequestDto>
{
    public ForgotPasswordValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Email format is invalid.");
    }
}