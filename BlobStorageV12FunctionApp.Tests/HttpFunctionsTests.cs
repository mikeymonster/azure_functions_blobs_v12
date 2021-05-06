using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BlobStorageV12FunctionApp.Tests.Builders;
using BlobStorageV12FunctionApp.Tests.Extensions;
using FluentAssertions;
using Xunit;

namespace BlobStorageV12FunctionApp.Tests
{
    public class HttpFunctionsTests
    {
        [Fact]
        public async Task Health_Check_Returns_Expected_Result()
        {
            const string expected = "All good";

            var functionContext = FunctionContextBuilder.Build();
            var request = FunctionHttpObjectBuilder.BuildHttpRequestData(HttpMethod.Get);

            var functions = FunctionBuilder.BuildHttpFunctions();

            var result = await functions.HealthCheck(request, functionContext);

            result.Headers.GetValues("Content-Type").Should().NotBeNull();
            result.Headers.GetValues("Content-Type").First().Should().Be("text/plain");

            var actual = await result.Body.ReadAsString();

            actual.Should().Be(expected);
        }
    }
}
