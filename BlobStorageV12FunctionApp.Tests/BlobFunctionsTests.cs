using System;
using System.Net.Http;
using System.Threading.Tasks;
using BlobStorageV12FunctionApp.Tests.Builders;
using Xunit;

namespace BlobStorageV12FunctionApp.Tests
{
    public class BlobFunctionsTests
    {
        [Fact]
        public async Task Process_Blob_Returns_Expected_Result()
        {
            const string blobName = "test";
            var functionContext = FunctionContextBuilder.Build();
            //var request = FunctionHttpObjectBuilder.BuildHttpRequestData(HttpMethod.Get);

            var functions = FunctionBuilder.BuildBlobFunctions();

            //await functions.ProcessBlob(clientBlob, blobName, functionContext);
        }
    }
}
