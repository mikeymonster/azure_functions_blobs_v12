namespace BlobStorageV12.Core.Model
{
    public class BlobInfoMessage
    {
        public string ContainerName { get; set; }
        public string FileName { get; set; }
        public string ETag { get; set; }
        public string VersionId { get; set; }
    }
}
