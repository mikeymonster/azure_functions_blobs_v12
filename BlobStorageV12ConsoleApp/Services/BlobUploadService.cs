using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Queues;
using BlobStorageV12ConsoleApp.Model;
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
            _logger.LogInformation($"BlobUploadService:: Blob Storage Connection = {_storageConfiguration.BlobStorageConnectionString}");
            _logger.LogInformation($"BlobUploadService:: Queue Storage Connection = {_storageConfiguration.QueueStorageConnectionString}");
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
            var queueName = $"{blobContainerName}-blob-queue";
            var queueMessage = new BlobInfoMessage
            {
                ContainerName = blobContainerName,
                FileName = fileName,
                ETag = blobContentInfo.Value?.ETag.ToString(),
                VersionId = blobContentInfo.Value?.VersionId,
            };
            var message = JsonSerializer.Serialize(queueMessage);

            var queueClient = await GetQueueClient(queueName);
            await queueClient.SendMessageAsync(message);

            _logger.LogInformation($"Added Message to Message Queue {queueName} => {message}");
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

        private async Task<QueueClient> GetQueueClient(string queueName)
        {
            var queueClient = new QueueClient(_storageConfiguration.QueueStorageConnectionString, queueName,
                new QueueClientOptions
                {
                    MessageEncoding = QueueMessageEncoding.Base64
                });

            await queueClient.CreateIfNotExistsAsync();

            return queueClient;
        }

    }
}
