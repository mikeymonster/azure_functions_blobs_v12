using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BlobStorageV12ConsoleApp.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BlobStorageV12ConsoleApp.Services
{
    public interface IBlobUploadService
    {
        Task UploadFile(string path, string blobContainerName, string contentType);
    }

    public class BlobUploadService : IBlobUploadService
    {
        private readonly ILogger _logger;
        private readonly StorageConfiguration _storageConfiguration;

        public BlobUploadService(
            ILogger<BlobUploadService> logger,
            IOptions<StorageConfiguration> storageConfiguration)
        {
            _storageConfiguration = storageConfiguration?.Value ?? throw new ArgumentNullException(nameof(storageConfiguration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _logger.LogInformation($"BlobUploadService:: Azure Web Jobs Connection = {_storageConfiguration.AzureWebJobsStorage}");
            _logger.LogInformation($"BlobUploadService:: Blog Storage Connection = {_storageConfiguration.BlobStorageConnectionString}");
        }

        public async Task UploadFile(string path, string blobContainerName, string contentType)
        {
            _logger.LogInformation($"Uploading file from path {path}");

            var fileName = Path.GetFileName(path);

            await using var fileStream = new FileStream(path, FileMode.Open);
            //using var streamReader = new StreamReader(fileStream);

            var blobClient = await GetBlobClient(
                blobContainerName.ToLowerInvariant(),
                fileName);
            
            var metadata = GetMetadata();
            //Upload blob
            var blobContentInfo = await blobClient.UploadAsync(fileStream,
                metadata: metadata,
                httpHeaders: new BlobHttpHeaders { ContentType = contentType });

            _logger.LogInformation($"Uploaded blob {blobContainerName}/{fileName} with etag {blobContentInfo.Value.ETag}");

            //Send queue message

        }

        private async Task<BlobClient> GetBlobClient(
            string containerName,
            string fileName)
        {
            var blobContainerClient = await GetBlobContainer(containerName);

            var blobClient = blobContainerClient.GetBlobClient(fileName);
            return blobClient;
        }

        private async Task<BlobContainerClient> GetBlobContainer(string containerName)
        {
            var blobServiceClient = new BlobServiceClient(_storageConfiguration.BlobStorageConnectionString);

            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();

            return containerClient;
        }

        private IDictionary<string, string> GetMetadata()
        {
            return new Dictionary<string, string>
            {
                {"created_by", Environment.UserName}
            };
        }
    }
}
