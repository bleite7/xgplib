using Serilog;

namespace XgpLib.SyncService;

public class Program
{
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

        var builder = Host.CreateApplicationBuilder(args);

        builder.Services.AddScoped<SyncGenresUseCase>();
        builder.Services.AddScoped<AuthenticationHandler>();
        builder.Services.AddHttpClient<ITokenManagerService, TokenManagerService>();
        builder.Services.AddHttpClient<IIgdbService, IgdbService>().AddHttpMessageHandler<AuthenticationHandler>();
        builder.Services.AddSerilog();

        builder.Services.AddHostedService<IgdbGenresSyncWorker>();
        var host = builder.Build();
        host.Run();
    }
}
