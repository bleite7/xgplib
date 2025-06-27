using Microsoft.EntityFrameworkCore;
using Serilog;
using XgpLib.SyncService.Infrastructure.Data;
using XgpLib.SyncService.Infrastructure.Data.Repositories;

namespace XgpLib.SyncService;

public class Program
{
    public static void Main(string[] args)
    {
        // Configure Serilog for logging
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

        // Create the application builder
        // This is the entry point for the .NET application
        var builder = Host.CreateApplicationBuilder(args);

        // Create and configure database context
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<XgpLibDbContext>(options => options
            .UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention());

        // Register services
        builder.Services.AddScoped<SyncGenresUseCase>();
        builder.Services.AddScoped<SyncGamesUseCase>();
        builder.Services.AddScoped<AuthenticationHandler>();

        // Register HTTP clients
        builder.Services.AddScoped<IGenreRepository, GenreRepository>();
        builder.Services.AddHttpClient<ITokenManagerService, TokenManagerService>();
        builder.Services.AddHttpClient<IIgdbService, IgdbService>().AddHttpMessageHandler<AuthenticationHandler>();

        // Add Serilog for logging
        builder.Services.AddSerilog();

        // Register the synchronization workers
        builder.Services.AddHostedService<IgdbGenresSyncWorker>();
        builder.Services.AddHostedService<IgdbGamesSyncWorker>();

        // Run the application
        var host = builder.Build();
        host.Run();
    }
}
