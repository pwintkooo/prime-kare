using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PrimeKare.Api.DTOs.Bookings;

namespace PrimeKare.Api.Services;

public class BookingDocumentService : IBookingDocumentService
{
    private readonly IWebHostEnvironment _environment;

    public BookingDocumentService(
        IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public byte[] GenerateBookingConfirmation(
        BookingDto booking)
    {
        using var document = new PdfDocument();

        document.Info.Title =
            $"PrimeKare Booking {booking.ReferenceNumber}";

        var page = document.AddPage();
        page.Size = PdfSharp.PageSize.A4;

        using var graphics = XGraphics.FromPdfPage(page);

        var regularFont =
    new XFont(
        "Noto Sans",
        10,
        XFontStyleEx.Regular);

        var smallFont =
            new XFont(
                "Noto Sans",
                9,
                XFontStyleEx.Regular);

        var headingFont =
            new XFont(
                "Noto Sans",
                18,
                XFontStyleEx.Bold);

        var sectionFont =
            new XFont(
                "Noto Sans",
                12,
                XFontStyleEx.Bold);

        var labelFont =
            new XFont(
                "Noto Sans",
                10,
                XFontStyleEx.Bold);

        const double margin = 50;

        var pageWidth = page.Width.Point;
        var contentWidth = pageWidth - (margin * 2);

        double y = 40;

        DrawHeader(
            graphics,
            pageWidth,
            margin,
            contentWidth,
            ref y);

        y += 25;

        graphics.DrawString(
            "Booking Confirmation",
            headingFont,
            XBrushes.Black,
            new XRect(
                margin,
                y,
                contentWidth,
                25),
            XStringFormats.TopLeft);

        y += 30;

        graphics.DrawString(
            $"Reference: {booking.ReferenceNumber}",
            regularFont,
            XBrushes.DimGray,
            new XRect(
                margin,
                y,
                contentWidth,
                20),
            XStringFormats.TopLeft);

        y += 35;

        DrawSection(
            graphics,
            "Customer Details",
            [
                ("Name", booking.CustomerName),
                ("Email", booking.CustomerEmail),
                ("Phone", booking.CustomerPhone)
            ],
            sectionFont,
            labelFont,
            regularFont,
            margin,
            contentWidth,
            ref y);

        y += 18;

        DrawSection(
            graphics,
            "Vehicle Details",
            [
                (
                    "Vehicle",
                    $"{booking.VehicleMake} {booking.VehicleModel}"
                ),
                (
                    "Plate Number",
                    booking.VehiclePlateNumber
                )
            ],
            sectionFont,
            labelFont,
            regularFont,
            margin,
            contentWidth,
            ref y);

        y += 18;

        DrawSection(
            graphics,
            "Appointment Details",
            [
                ("Service", booking.ServiceName),
                (
                    "Date",
                    booking.BookingDate
                        .ToString("dd MMMM yyyy")
                ),
                (
                    "Time",
                    FormatTime(booking.BookingTime)
                ),
                (
                    "Status",
                    FormatStatus(booking.Status)
                )
            ],
            sectionFont,
            labelFont,
            regularFont,
            margin,
            contentWidth,
            ref y);

        if (!string.IsNullOrWhiteSpace(booking.Notes))
        {
            y += 18;

            DrawSection(
                graphics,
                "Notes",
                [
                    ("Notes", booking.Notes)
                ],
                sectionFont,
                labelFont,
                regularFont,
                margin,
                contentWidth,
                ref y);
        }

        y += 30;

        graphics.DrawString(
            "Please keep this document for your records.",
            smallFont,
            XBrushes.DimGray,
            new XRect(
                margin,
                y,
                contentWidth,
                20),
            XStringFormats.TopLeft);

        using var stream = new MemoryStream();

        document.Save(stream, false);

        return stream.ToArray();
    }

    private void DrawHeader(
        XGraphics graphics,
        double pageWidth,
        double margin,
        double contentWidth,
        ref double y)
    {
        const double headerHeight = 70;

        var navyBrush =
            new XSolidBrush(
                XColor.FromArgb(2, 6, 23));

        graphics.DrawRectangle(
            navyBrush,
            0,
            0,
            pageWidth,
            headerHeight);

        var logoPath = Path.Combine(
            _environment.ContentRootPath,
            "Assets",
            "primekare-dark-logo.png");

        if (File.Exists(logoPath))
        {
            using var logo =
                XImage.FromFile(logoPath);

            const double logoHeight = 42;

            var ratio =
                logo.PixelWidth /
                (double)logo.PixelHeight;

            var logoWidth =
                logoHeight * ratio;

            graphics.DrawImage(
                logo,
                margin,
                14,
                logoWidth,
                logoHeight);
        }

        y = headerHeight;
    }

    private static void DrawSection(
        XGraphics graphics,
        string title,
        IEnumerable<(string Label, string? Value)> rows,
        XFont sectionFont,
        XFont labelFont,
        XFont regularFont,
        double x,
        double width,
        ref double y)
    {
        var rowList = rows.ToList();

        const double padding = 15;
        const double titleHeight = 20;
        const double rowHeight = 22;

        var height =
            padding +
            titleHeight +
            (rowList.Count * rowHeight) +
            padding;

        var borderPen =
            new XPen(
                XColor.FromArgb(226, 232, 240),
                1);

        graphics.DrawRectangle(
            borderPen,
            x,
            y,
            width,
            height);

        var currentY = y + padding;

        graphics.DrawString(
            title,
            sectionFont,
            XBrushes.Black,
            new XRect(
                x + padding,
                currentY,
                width - (padding * 2),
                titleHeight),
            XStringFormats.TopLeft);

        currentY += titleHeight + 5;

        foreach (var (label, value) in rowList)
        {
            graphics.DrawString(
                label,
                labelFont,
                XBrushes.DimGray,
                new XRect(
                    x + padding,
                    currentY,
                    120,
                    rowHeight),
                XStringFormats.TopLeft);

            graphics.DrawString(
                value ?? "-",
                regularFont,
                XBrushes.Black,
                new XRect(
                    x + 140,
                    currentY,
                    width - 155,
                    rowHeight),
                XStringFormats.TopLeft);

            currentY += rowHeight;
        }

        y += height;
    }

    private static string FormatTime(
        TimeSpan time)
    {
        return DateTime.Today
            .Add(time)
            .ToString("h:mm tt");
    }

    private static string FormatStatus(
        string status)
    {
        return status switch
        {
            "pending" => "Pending",
            "confirmed" => "Confirmed",
            "in-progress" => "In Progress",
            "completed" => "Completed",
            "cancelled" => "Cancelled",
            "no-show" => "No Show",
            _ => status
        };
    }
}