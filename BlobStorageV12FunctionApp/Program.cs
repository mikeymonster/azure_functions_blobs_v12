using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

#if DEBUG
Debugger.Launch();
#endif

var host = new HostBuilder()
    //.ConfigureFunctionsWorkerDefaults()
    .ConfigureFunctionsWorkerDefaults(workerApplication =>
    {
        // Register our custom middleware with the worker
        //workerApplication.UseMiddleware<MyCustomMiddleware>();
    })
    // Action<HostBuilderContext, IFunctionsWorkerApplicationBuilder> configure, Action<WorkerOptions> configureOptions
    //.ConfigureFunctionsWorker((hostBuilderContext, options) =>
    //{
    //    //hostBuilderContext.UseFunctionExecutionMiddleware();
    //})
    //.ConfigureAppConfiguration(c =>
    //    {
    //        c.AddCommandLine(args);
    //        c.AddJsonFile("local.settings.json", optional: true, reloadOnChange: false);
    //        c.AddJsonFile("local.settings.development.json", optional: true, reloadOnChange: false);
    //        c.AddEnvironmentVariables();
    //    }
    //)
    //.ConfigureFunctionsWorker((c, b) =>
    //{
    //    b.UseFunctionExecutionMiddleware();
    //})
    .ConfigureServices((hostContext, services) =>
    {
        var config = hostContext.Configuration;
    })
    .Build();

await host.RunAsync();
