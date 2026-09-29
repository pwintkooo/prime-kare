using PrimeKare.Api.DTOs.Bookings;

namespace PrimeKare.Api.Services;

public interface IBookingDocumentService
{
    byte[] GenerateBookingConfirmation(
        BookingDto booking);
}