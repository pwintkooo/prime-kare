using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;

namespace PrimeKare.Api.Services;

public class FirebaseStorageService : IImageStorageService
{
    private readonly StorageClient _storageClient;
    private readonly string _bucketName;

    public FirebaseStorageService(IConfiguration configuration)
    {
        var credentialPath = configuration["Firebase:CredentialPath"]
            ?? throw new InvalidOperationException(
                "Firebase credential path is not configured.");

        _bucketName = configuration["Firebase:BucketName"]
            ?? throw new InvalidOperationException(
                "Firebase bucket name is not configured.");

        var credential = CredentialFactory
            .FromFile<ServiceAccountCredential>(credentialPath)
            .ToGoogleCredential();

        _storageClient = StorageClient.Create(credential);
    }

    public async Task<string> UploadImageAsync(
        Stream fileStream,
        string fileName,
        string contentType)
    {
        var objectName =
            $"services/{Guid.NewGuid()}-{fileName}";

        await _storageClient.UploadObjectAsync(
            _bucketName,
            objectName,
            contentType,
            fileStream);

        return
            $"https://storage.googleapis.com/{_bucketName}/{objectName}";
    }

    public async Task DeleteImageAsync(string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            return;
        }

        var prefix =
            $"https://storage.googleapis.com/{_bucketName}/";

        if (!imageUrl.StartsWith(prefix))
        {
            return;
        }

        var objectName =
            imageUrl.Substring(prefix.Length);

        await _storageClient.DeleteObjectAsync(
            _bucketName,
            objectName);
    }
}