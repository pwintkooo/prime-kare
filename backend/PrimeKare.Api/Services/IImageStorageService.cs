namespace PrimeKare.Api.Services;

public interface IImageStorageService
{
    Task<string> UploadImageAsync(
        Stream fileStream,
        string fileName,
        string contentType);

    Task DeleteImageAsync(string imageUrl);
}