namespace PrimeKare.Api.Services;

public interface IAzureBlobStorageService
{
    Task<string> UploadImageAsync(
        Stream stream,
        string fileName,
        string contentType);

    Task DeleteImageAsync(string imageUrl);
}