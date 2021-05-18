using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BlobStorageV12.Core.Model;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace BlobStorageV12FunctionApp
{
    public class BlobFunctions
    {
        private const string TestBlobContainer = "blob-test";
        private const string BlobConnection = "BlobStorageConnectionString";

        //public BlobFunctions()
        //{
        //}
        /*
        [Function("ProcessBlob")]
        public async Task ProcessBlob(
            //[File]
            [BlobTrigger("blob-test/{name}",
                Connection = "BlobStorageConnectionString"),
             //FileAccess.ReadWrite
            ] 
            Stream stream,
            //BlobClient blobClient,
            string name,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("BlobFunction");
            logger.LogInformation($"Blob triggered ProcessBlob function called. Blob name = '{name}'");

            //    var response = req.CreateResponse(HttpStatusCode.OK);
            //    response.Headers.Add("Content-Type", "text/plain");

            //    await response.WriteStringAsync("All good");

            //    return response;
        }
        */

        /*
        //This works but gets text content as a string
        [Function("BlobFunction")]
        [BlobOutput("blob-test-output/{name}-output.txt")]
        public static string Run(
            [BlobTrigger("blob-test/{name}")] string myTriggerItem,
            //[BlobInput("test-samples-input/sample1.txt")] string myBlob,
            string name,
            FunctionContext context)
        {
            var logger = context.GetLogger("BlobFunction");
            logger.LogInformation($"Triggered Item = {myTriggerItem}");
            logger.LogInformation($"Name = {name}");
            //logger.LogInformation($"Input Item = {myBlob}");

            // Blob Output
            return "queue message";
        }
        */

        //this works - but I don't like it
        [Function("BlobFunctionWithOutput")]
        [BlobOutput("blob-test-output/{name}-output.txt")]
        public async Task<string> Run(
            [Microsoft.Azure.Functions.Worker.BlobTrigger(TestBlobContainer + "/{name}")] 
            //string myTriggerItem, //Works for blob
            //byte[] bytes, //Works for blob
            //BlockBlobClient blockBlobClient,
            BlobClient blobClientIn,
            //[BlobInput("test-samples-input/sample1.txt")] string myBlob,
            //https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-storage-blob-trigger?tabs=csharp#blob-name-patterns
            string name,
            Uri uri,
            BlobProperties properties,
            string metadata,
            FunctionContext context)
        {
            var logger = context.GetLogger("BlobFunction");
            //logger.LogInformation($"Triggered Item = {myTriggerItem}");
            logger.LogInformation($"Name = {name}");
            //logger.LogInformation($"Input Item = {myBlob}");

            var connectionString = Environment.GetEnvironmentVariable(BlobConnection);
            if (string.IsNullOrEmpty(connectionString))
            {
                //TODO: pass from startup
                connectionString = "UseDevelopmentStorage=true";
            }

            var blobServiceClient = new BlobServiceClient(connectionString);
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(TestBlobContainer);
            var blobClient = blobContainerClient.GetBlobClient(name);

            var blobProperties = await blobClient.GetPropertiesAsync();

            logger.LogInformation($"Blob was updated on: {blobProperties.Value.LastModified}");
            if (blobProperties.Value.Metadata != null && blobProperties.Value.Metadata.Any())
            {
                foreach (var (key, value) in blobProperties.Value.Metadata)
                {
                    logger.LogInformation($"    Metadata: {key} = {value}");
                }
            }

            logger.LogInformation("Blob was updated on: {datetime}", blobProperties.Value.LastModified);

            await using var stream = await blobClient.OpenReadAsync();
            using var streamReader = new StreamReader(stream);
            var content = await streamReader.ReadToEndAsync();
            logger.LogInformation("Blob content: ");
            logger.LogInformation(content);

            // Blob Output
            return "queue message";
        }
        /**/

        /*
        // https://github.com/Azure/azure-sdk-for-net/tree/master/sdk/storage/Microsoft.Azure.WebJobs.Extensions.Storage.Blobs#examples
        //Doesn't convert/bind
        //Try with Azure.WebJobs.Extensions.Storage.Blobs preview package
        //https://docs.microsoft.com/en-us/dotnet/api/overview/azure/microsoft.azure.webjobs.extensions.storage.blobs-readme-pre
        [Function("BlobFunction")]
        public static async Task Run(
            //[BlobTrigger("blob-test/{name}")] BlobClient blobClient1,
            [BlobTrigger("blob-test/{name}")] BlobClient blobClient1,
            //[BlobInput("sample-container/sample-blob-2")] BlobClient blobClient2,
            ILogger logger)
        {
            BlobProperties blobProperties1 = await blobClient1.GetPropertiesAsync();
            logger.LogInformation("Blob sample-container/sample-blob-1 has been updated on: {datetime}", blobProperties1.LastModified);
            //BlobProperties blobProperties2 = await blobClient2.GetPropertiesAsync();
            //logger.LogInformation("Blob sample-container/sample-blob-2 has been updated on: {datetime}", blobProperties2.LastModified);
        }         
        */
        /*
         /http://dontcodetired.com/blog/post/Automatic-Input-Blob-Binding-in-Azure-Functions-from-Queue-Trigger-Message-Data

        [Function("ConvertNameCase")]
        public static void Run(
            [QueueTrigger("capitalize-names")] string inputBlobPath,
            [Blob("names-in/{queueTrigger}", FileAccess.Read)] string originalName,
            [Blob("names-out/{queueTrigger}")] out string capitalizedName)
        {
            capitalizedName = originalName.ToUpperInvariant();
        }


        public static class ConvertNameCase
    {
        [FunctionName("ConvertNameCase")]
        public static void Run([QueueTrigger("capitalize-names")]string inputBlobPath)
        {
            string originalName = ReadInputName(inputBlobPath);
 
            var capitalizedName = originalName.ToUpperInvariant();
 
            WriteOutputName(inputBlobPath, capitalizedName);
        }
         
        private static string ReadInputName(string blobPath)
        {
            CloudStorageAccount account = CloudStorageAccount.DevelopmentStorageAccount;
            CloudBlobClient blobClient = account.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("names-in");
 
            var blobReference = container.GetBlockBlobReference(blobPath);
 
            string originalName = blobReference.DownloadText();
 
            return originalName;
        }
 
        private static void WriteOutputName(string blobPath, string capitalizedName)
        {
            CloudStorageAccount account = CloudStorageAccount.DevelopmentStorageAccount;
            CloudBlobClient blobClient = account.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("names-out");
 
            CloudBlockBlob cloudBlockBlob = container.GetBlockBlobReference(blobPath);
            cloudBlockBlob.UploadText(capitalizedName);            
        }
 
    }
        */

        [Function("BlobFunctionWithBytes")]
        public void RunWithBytes(
            [Microsoft.Azure.Functions.Worker.BlobTrigger("sample-container/{name}")] byte[] bytes,
            string name,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("BlobFunction");

            var stringFromByteArray = System.Text.Encoding.UTF8.GetString(bytes);
            var stringFromBitConverter = BitConverter.ToString(bytes);

            //await using var stream = await blobClient.OpenReadAsync();
            //using var streamReader = new StreamReader(stream);
            //var content = await streamReader.ReadToEndAsync();

            var content = stringFromBitConverter;

            //using var blobStreamReader = new StreamReader(bytes);

            logger.LogInformation($"Blob sample-container/{name} has been updated with content: {content}");
        }

        [Function("QueuedBlob")]
        public async Task QueuedBlob(
            [QueueTrigger("to-be-queued-blob-queue")] string queueMessage,
            //[BlobInput("to-be-queued/{queueTrigger}", FileAccess.Read)] byte[] blob,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("BlobFunction");
            
            logger.LogInformation($"Have queued message '{queueMessage}'");
            
            var message = System.Text.Json.JsonSerializer.Deserialize<BlobInfoMessage>(queueMessage);


            var connectionString = Environment.GetEnvironmentVariable(BlobConnection);
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = "UseDevelopmentStorage=true";
            }


            var blobServiceClient = new BlobServiceClient(connectionString);
            var blobContainerClient = blobServiceClient
                .GetBlobContainerClient(message.ContainerName);
            var blobClient = blobContainerClient
                .GetBlobClient(message.FileName);

            var blobProperties = await blobClient.GetPropertiesAsync();

            logger.LogInformation($"BlobClient has eTag {blobProperties.Value.ETag} vs queued {message.ETag}");

            logger.LogInformation($"Blob was updated on: {blobProperties.Value.LastModified}");
            if (blobProperties.Value.Metadata != null && blobProperties.Value.Metadata.Any())
            {
                foreach (var (key, value) in blobProperties.Value.Metadata)
                {
                    logger.LogInformation($"    Metadata: {key} = {value}");
                }
            }

            logger.LogInformation("Blob was updated on: {datetime}", blobProperties.Value.LastModified);

            await using var stream = await blobClient.OpenReadAsync();
            using var streamReader = new StreamReader(stream);
            var content = await streamReader.ReadToEndAsync();
            logger.LogInformation("Blob content: ");
            logger.LogInformation(content);
        }
    }
}
