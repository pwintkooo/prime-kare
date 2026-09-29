using FluentValidation;
using PrimeKare.Api.DTOs.Bookings;

namespace PrimeKare.Api.Validators.Bookings;

public class UpdateBookingDtoValidator
    : AbstractValidator<UpdateBookingDto>
{
    public UpdateBookingDtoValidator()
    {
        RuleFor(x => x.VehicleId)
            .NotNull()
            .WithMessage("Vehicle is required.")
            .GreaterThan(0)
            .WithMessage("Invalid vehicle.");

        RuleFor(x => x.ServiceId)
            .GreaterThan(0)
            .WithMessage("Service is required.");

        RuleFor(x => x.BookingDate)
            .NotEmpty()
            .WithMessage("Booking date is required.");

        RuleFor(x => x.BookingTime)
            .NotEmpty()
            .WithMessage("Booking time is required.");

        RuleFor(x => x.Notes)
            .MaximumLength(1000)
            .WithMessage("Notes cannot exceed 1000 characters.");
    }
}