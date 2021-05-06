
namespace BlobStorageV12FunctionApp.Tests.Builders
{
    public static class FunctionBuilder
    {
        public static BlobFunctions BuildBlobFunctions()
        {
            return new();
        }

        public static HttpFunctions BuildHttpFunctions()
        {
            return new();
        }
    }
}
