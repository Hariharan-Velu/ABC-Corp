using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging;

namespace InfraLayer.Services
{
    public class BlobService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;
        private readonly ILogger<BlobService> _logger;
        public BlobService(BlobServiceClient blobServiceClient, string containerName,ILogger<BlobService> logger)
        {
            _blobServiceClient = blobServiceClient;
            _containerName = containerName;
            _logger = logger;
        }

        public async Task<string> UploadFileAsync(string blobName, Stream fileStream)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
                await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);
                var blobClient = containerClient.GetBlobClient(blobName);
                await blobClient.UploadAsync(fileStream, overwrite: true);
            }
            catch(Exception ex)
            {
                _logger.LogError("Error occurred on file upload "+ex.Message);
            }
            return blobName;
        }
        public async Task<Stream> DownloadFileAsync(string blobName)
        {
            
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
                var blobClient = containerClient.GetBlobClient(blobName);
                var response = await blobClient.DownloadAsync();
                return response.Value.Content;
            }
            catch(Exception ex)
            {
                _logger.LogError("Error occurred on file download "+ex.Message);
                return new MemoryStream();
            }
            
        }

    }
}
