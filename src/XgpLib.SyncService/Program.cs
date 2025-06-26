using XgpLib.SyncService.Infrastructure.HttpHandlers;

namespace XgpLib.SyncService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        builder.Services.AddScoped<SyncGenresUseCase>();
        builder.Services.AddScoped<AuthenticationHandler>();
        builder.Services.AddHttpClient<ITokenManagerService, TokenManagerService>();
        builder.Services.AddHttpClient<IIgdbService, IgdbService>().AddHttpMessageHandler<AuthenticationHandler>();

        builder.Services.AddHostedService<IgdbGenresSyncWorker>();
        var host = builder.Build();
        host.Run();
    }
}
