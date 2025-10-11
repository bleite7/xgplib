using Serilog;
using XgpLib.SyncService.CrossCutting;

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
builder.Services.AddHostedService<IgdbGenresSyncWorker>();
builder.Services.AddHostedService<IgdbGamesSyncWorker>();

// Run the application
var host = builder.Build();
await host.RunAsync();
