using System.IO;
using System.Reflection;
using BlobStorageV12ConsoleApp.Configuration;
using BlobStorageV12ConsoleApp.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BlobStorageV12ConsoleApp;

//https://dfederm.com/building-a-console-app-with-.net-generic-host/
await Host.CreateDefaultBuilder(args)
    .UseContentRoot(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
    .ConfigureLogging(logging =>
    {
        // Add any 3rd party loggers
    })
    .ConfigureServices((hostContext, services) =>
    {
        services
            .AddHostedService<ConsoleHostedService>()
            .AddSingleton<IBlobUploadService, BlobUploadService>();

        services.AddOptions<StorageConfiguration>().Bind(hostContext.Configuration.GetSection("StorageConfiguration"));
    })
    .RunConsoleAsync();
