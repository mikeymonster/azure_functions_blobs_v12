using System.IO;
using System.Net.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using NSubstitute;

namespace BlobStorageV12FunctionApp.Tests.Builders
{
    public static class FunctionHttpObjectBuilder
    {
        public static HttpRequestData BuildHttpRequestData(
            HttpMethod method,
            FunctionContext functionContext = null)
        {
            functionContext ??= FunctionContextBuilder.Build();

            var request = Substitute.For<HttpRequestData>(functionContext);
            request.Method.Returns(method.ToString());

            var responseData = BuildHttpResponseData(functionContext);
            request.CreateResponse().Returns(responseData);

            return request;
        }

        public static HttpResponseData BuildHttpResponseData(
            FunctionContext functionContext = null)
        {
            functionContext ??= FunctionContextBuilder.Build();

            var responseData = Substitute.For<HttpResponseData>(functionContext);

            var responseHeaders = new HttpHeadersCollection();
            responseData.Headers.Returns(responseHeaders);

            var responseBody = new MemoryStream();
            responseData.Body.Returns(responseBody);

            return responseData;
        }
    }
}
