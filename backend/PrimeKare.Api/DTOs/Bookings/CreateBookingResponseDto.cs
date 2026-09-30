namespace PrimeKare.Api.DTOs.Bookings;

public class CreateBookingResponseDto
{
    public BookingDto Booking { get; set; } = null!;

    public string? GuestAccessToken { get; set; }
}