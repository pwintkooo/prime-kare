namespace PrimeKare.Api.Services;

public interface IFirebaseStorageService
{
    Task<string> UploadImageAsync(
        Stream fileStream,
        string fileName,
        string contentType);

    Task DeleteImageAsync(string fileName);
}