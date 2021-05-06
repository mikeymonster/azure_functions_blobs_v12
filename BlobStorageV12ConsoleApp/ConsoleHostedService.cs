using System;
using System.Threading;
using System.Threading.Tasks;
using BlobStorageV12ConsoleApp.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BlobStorageV12ConsoleApp
{
    internal sealed class ConsoleHostedService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly IBlobUploadService _blobUploadService;
        private int? _exitCode;

        public ConsoleHostedService(
            ILogger<ConsoleHostedService> logger,
            IHostApplicationLifetime appLifetime,
            IBlobUploadService blobUploadService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _appLifetime = appLifetime ?? throw new ArgumentNullException(nameof(appLifetime));
            _blobUploadService = blobUploadService ?? throw new ArgumentNullException(nameof(blobUploadService));
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var args = Environment.GetCommandLineArgs();
            _logger.LogDebug($"Starting with arguments: {string.Join(" ", args)}");

            _appLifetime.ApplicationStarted.Register(() =>
            {
                Task.Run(async () =>
                {
                    try
                    {
                        _logger.LogInformation("Hello World 2!");

                        // Simulate real work is being done
                        //await Task.Delay(1000);
                        string filePath;
                        if (args.Length >= 2)
                        {
                            //Second arg is the file name, because first is the executable being run
                            filePath = args[1];
                        }
                        else
                        {
                            Console.WriteLine("Enter a blob file path: ");
                            filePath = Console.ReadLine();
                        }

                        const string defaultBlobContainerName = "to-be-queued";
                        const string contentTypeText = "text/plain";

                        if (!string.IsNullOrWhiteSpace(filePath))
                        {
                            await _blobUploadService.UploadFile(filePath, defaultBlobContainerName, contentTypeText);
                        }

                        _exitCode = 0;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Unhandled exception!");
                        _exitCode = 1;
                    }
                    finally
                    {
                        // Stop the application once the work is done
                        _appLifetime.StopApplication();
                    }
                });
            });

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug($"Exiting with return code: {_exitCode}");
            // Exit code may be null if the user cancelled via Ctrl+C/SIGTERM
            Environment.ExitCode = _exitCode.GetValueOrDefault(-1);
            return Task.CompletedTask;
        }
    }
}
