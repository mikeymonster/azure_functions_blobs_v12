
namespace BlobStorageV12.Core.Model
{
    public class StorageConfiguration
    {
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        public string AzureWebJobsStorage { get; init; }
        public string BlobStorageConnectionString { get; init; }
        public string QueueStorageConnectionString { get; init; }
        // ReSharper restore UnusedAutoPropertyAccessor.Global
    }
}
