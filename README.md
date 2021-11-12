

## Upgrade functions:

The project was created as .NET Core 3.1, so it needs to be updated.

### Change project:

> (This is obsolete and probably not needed for .NET 6.0)

```
  <PropertyGroup>
    <TargetFramework>net5.0</TargetFramework>
    <AzureFunctionsVersion>v3</AzureFunctionsVersion>
    <OutputType>Exe</OutputType>
    <_FunctionsSkipCleanOutput>true</_FunctionsSkipCleanOutput>
  </PropertyGroup>
```

Remove nuget package:
```
Microsoft.NET.Sdk.Functions
```

Add nuget packages:
```
Microsoft.Azure.Functions.Worker
Microsoft.Azure.Functions.Worker.Sdk OutputItemType="Analyzer" 
Microsoft.Azure.WebJobs.Extensions.Storage
System.Net.NameResolution

optional?
Microsoft.Azure.WebJobs.Extensions
Microsoft.Azure.WebJobs.Extensions.Http

?
Microsoft.Azure.WebJobs.Script.ExtensionsMetadataGenerator

```

Add an `OutputItemType="Analyzer"` attribute to `Microsoft.Azure.Functions.Worker.Sdk`

Add Program.cs

Update local.settings.json runtime version:
```
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
```

## Running the functions

Until VS2019 is upgraded, functions have to be run from the console.

Navigate to the function project folder, e.g. 
```
cd C:\dev\repos\BlobStorageV12FunctionApp\BlobStorageV12FunctionApp\
```

If function tools are not installed, run
```
npm i -g azure-functions-core-tools@3 --unsafe-perm true
```

Then run the functions using
```
func host start --verbose
```


## Links

	- Announcement: https://techcommunity.microsoft.com/t5/apps-on-azure/net-on-azure-functions-roadmap/ba-p/2197916
	  - https://docs.microsoft.com/en-gb/azure/azure-functions/dotnet-isolated-process-developer-howtos?pivots=development-environment-vscode&tabs=browser
	  - https://docs.microsoft.com/en-gb/azure/azure-functions/dotnet-isolated-process-guide
	- Older references - these use pre-release code
	  - https://codetraveler.io/2021/02/12/creating-azure-functions-using-net-5/
	  - https://mattjameschampion.com/2020/12/23/so-you-want-to-run-azure-functions-using-net-5/


## Issues

https://github.com/Azure/Azure-Functions/issues/1881
https://github.com/Azure/azure-functions-dotnet-worker/issues/215

Basically, streams are not supported and we should use byte[] in .NET 5.0 functions, 
or perhaps a QueueTrigger or EventGridTrigger.

