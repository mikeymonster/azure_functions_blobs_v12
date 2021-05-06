using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace BlobStorageV12FunctionApp.Tests.Builders
{
    public static class FunctionContextBuilder
    {
        public static FunctionContext Build(ILogger logger = null)
        {
            logger ??= Substitute.For<ILogger>();
            var loggerFactory = Substitute.For<ILoggerFactory>();
            loggerFactory.CreateLogger(Arg.Any<string>())
                .Returns(logger);

            var functionContext = Substitute.For<FunctionContext>();
            functionContext.InstanceServices.GetService(Arg.Any<Type>())
                .Returns(loggerFactory);

            return functionContext;
        }
    }
}
