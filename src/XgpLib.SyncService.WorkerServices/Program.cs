using Serilog;
using XgpLib.SyncService.CrossCutting;

// Create the application builder
// This is the entry point for the .NET application
var builder = Host.CreateApplicationBuilder(args);

// Configure Serilog for logging
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .WriteTo.Console(outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] {Properties:j} " +
        "{Message:lj}{NewLine}{Exception}")
    .CreateLogger();

// Add Serilog for logging
builder.Services.AddSerilog();

// Register services
builder.Services.AddSyncServiceDependencies(builder.Configuration);

// Register the synchronization workers
builder.Services.AddHostedService<IgdbGenresSyncWorker>();
builder.Services.AddHostedService<IgdbGamesSyncWorker>();

// Run the application
var host = builder.Build();
await host.RunAsync();
