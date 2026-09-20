using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace PrimeKare.Api.Services;

public class AzureBlobStorageService
    : IImageStorageService
{
    private readonly BlobContainerClient _containerClient;
    private readonly string _accountName;

    public AzureBlobStorageService(
        IConfiguration configuration)
    {
        _accountName = configuration[
            "AzureStorage:AccountName"]
            ?? throw new InvalidOperationException(
                "Azure Storage account name is not configured.");

        var containerName = configuration[
            "AzureStorage:ContainerName"]
            ?? throw new InvalidOperationException(
                "Azure Storage container name is not configured.");

        var serviceUri = new Uri(
            $"https://{_accountName}.blob.core.windows.net");

        var credential = new DefaultAzureCredential();

        var blobServiceClient = new BlobServiceClient(
            serviceUri,
            credential);

        _containerClient =
            blobServiceClient.GetBlobContainerClient(
                containerName);
    }

    public async Task<string> UploadImageAsync(
        Stream stream,
        string fileName,
        string contentType)
    {
        var extension = Path.GetExtension(fileName);

        var blobName =
            $"services/{Guid.NewGuid()}{extension}";

        var blobClient =
            _containerClient.GetBlobClient(blobName);

        await blobClient.UploadAsync(
            stream,
            new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = contentType
                }
            });

        return blobClient.Uri.ToString();
    }

    public async Task DeleteImageAsync(
        string imageUrl)
    {
        var uri = new Uri(imageUrl);

        var blobName = uri.AbsolutePath
            .TrimStart('/')
            .Substring(
                _containerClient.Name.Length + 1);

        var blobClient =
            _containerClient.GetBlobClient(blobName);

        await blobClient.DeleteIfExistsAsync();
    }
}