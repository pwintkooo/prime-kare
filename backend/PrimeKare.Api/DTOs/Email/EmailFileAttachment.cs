namespace PrimeKare.Api.DTOs.Email;

public class EmailFileAttachment
{
    public string FileName { get; set; } = string.Empty;
    public byte[] Content { get; set; } = [];
    public string ContentType { get; set; } =
        "application/octet-stream";
}