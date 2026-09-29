using FluentValidation;
using PrimeKare.Api.DTOs.Bookings;

namespace PrimeKare.Api.Validators.Bookings;

public class CreateBookingDtoValidator
    : AbstractValidator<CreateBookingDto>
{
    public CreateBookingDtoValidator()
    {
        RuleFor(x => x.ServiceId)
            .GreaterThan(0)
            .WithMessage("Service is required.");

        RuleFor(x => x.BookingDate)
            .NotEmpty()
            .WithMessage("Booking date is required.")
            .Must(date => date >= DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Booking date cannot be in the past.");

        RuleFor(x => x.BookingTime)
            .NotEmpty()
            .WithMessage("Booking time is required.");

        RuleFor(x => x.VehicleId)
            .GreaterThan(0)
            .When(x => x.VehicleId.HasValue)
            .WithMessage("Invalid vehicle.");

        RuleFor(x => x.CustomerName)
            .MaximumLength(100)
            .WithMessage("Name cannot exceed 100 characters.");

        RuleFor(x => x.CustomerEmail)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.CustomerEmail))
            .WithMessage("Please enter a valid email address.")
            .Matches(@"^[^@\s]+@[^@\s]+\.[A-Za-z]{2,}$")
            .WithMessage(
                "Email must contain a valid domain."
            )
            .MaximumLength(254)
            .WithMessage(
                "Email must not exceed 254 characters."
            );

        RuleFor(x => x.CustomerPhone)
            .MaximumLength(20)
            .WithMessage("Phone number cannot exceed 20 characters.");

        RuleFor(x => x.VehiclePlateNumber)
            .MaximumLength(20)
            .WithMessage("Plate number cannot exceed 20 characters.");

        RuleFor(x => x.VehicleMake)
            .MaximumLength(50)
            .WithMessage("Vehicle make cannot exceed 50 characters.");

        RuleFor(x => x.VehicleModel)
            .MaximumLength(50)
            .WithMessage("Vehicle model cannot exceed 50 characters.");

        RuleFor(x => x.Notes)
            .MaximumLength(1000)
            .WithMessage("Notes cannot exceed 1000 characters.");
    }
}