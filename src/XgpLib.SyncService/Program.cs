using Serilog;
using XgpLib.SyncService.CrossCutting;

namespace XgpLib.SyncService;

public class Program
{
    public static async Task Main(string[] args)
    {
        // Configure Serilog for logging
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

        // Create the application builder
        // This is the entry point for the .NET application
        var builder = Host.CreateApplicationBuilder(args);

        // Register services
        builder.Services.AddSyncServiceDependencies(builder.Configuration);

        // Add Serilog for logging
        builder.Services.AddSerilog();

        // Register the synchronization workers
        builder.Services.AddHostedService<IgdbSyncWorker>();

        // Run the application
        var host = builder.Build();
        await host.RunAsync();
    }
}
