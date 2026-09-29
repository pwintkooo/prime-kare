using PdfSharp.Fonts;

namespace PrimeKare.Api.Services.Pdf;

public class PrimeKareFontResolver : IFontResolver
{
    private readonly string _fontsPath;

    public PrimeKareFontResolver(string fontsPath)
    {
        _fontsPath = fontsPath;
    }

    public byte[] GetFont(string faceName)
    {
        var fileName = faceName switch
        {
            "NotoSans-Regular" => "NotoSans-Regular.ttf",
            "NotoSans-Bold" => "NotoSans-Bold.ttf",

            _ => throw new InvalidOperationException(
                $"Font '{faceName}' is not supported.")
        };

        return File.ReadAllBytes(
            Path.Combine(_fontsPath, fileName));
    }

    public FontResolverInfo ResolveTypeface(
        string familyName,
        bool isBold,
        bool isItalic)
    {
        return new FontResolverInfo(
            isBold
                ? "NotoSans-Bold"
                : "NotoSans-Regular");
    }
}