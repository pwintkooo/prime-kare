using System.Net;
using PrimeKare.Api.DTOs.Bookings;

namespace PrimeKare.Api.Templates;

public static class BookingEmailTemplate
{
    public static string BuildConfirmationEmail(
        BookingDto booking,
        string frontendUrl)
    {
        var customerName =
            WebUtility.HtmlEncode(booking.CustomerName);

        var referenceNumber =
            WebUtility.HtmlEncode(booking.ReferenceNumber);

        var serviceName =
            WebUtility.HtmlEncode(booking.ServiceName);

        var vehicleMake =
            WebUtility.HtmlEncode(booking.VehicleMake);

        var vehicleModel =
            WebUtility.HtmlEncode(booking.VehicleModel);

        var plateNumber =
            WebUtility.HtmlEncode(booking.VehiclePlateNumber);

        var content = $"""
            <h2 style="margin-top: 0;">
                Booking Received
            </h2>

            <p>Hi {customerName},</p>

            <p>
                We've received your appointment request.
                Our team will review your booking and confirm
                your appointment.
            </p>

            <div style="
                margin: 24px 0;
                padding: 20px;
                background-color: #f8fafc;
                border: 1px solid #e2e8f0;
                border-radius: 8px;
            ">
                <p style="margin-top: 0;">
                    <strong>Booking Reference</strong><br>
                    {referenceNumber}
                </p>

                <p>
                    <strong>Service</strong><br>
                    {serviceName}
                </p>

                <p>
                    <strong>Vehicle</strong><br>
                    {vehicleMake} {vehicleModel}
                    ({plateNumber})
                </p>

                <p>
                    <strong>Date</strong><br>
                    {booking.BookingDate:dd MMMM yyyy}
                </p>

                <p>
                    <strong>Time</strong><br>
                    {FormatTime(booking.BookingTime)}
                </p>

                <p style="margin-bottom: 0;">
                    <strong>Status</strong><br>
                    Pending
                </p>
            </div>

            <p style="
                font-size: 14px;
                color: #64748b;
            ">
                Please keep your booking reference for
                future enquiries about your appointment.
            </p>

            <p style="margin-top: 32px;">
                Regards,<br>
                <strong>PrimeKare</strong>
            </p>
            """;

        return EmailLayoutTemplate.Build(
            frontendUrl,
            content);
    }

    private static string FormatTime(TimeSpan time)
    {
        return DateTime.Today
            .Add(time)
            .ToString("h:mm tt");
    }
}