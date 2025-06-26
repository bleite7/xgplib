using XgpLib.SyncService.Infrastructure.HttpHandlers;

namespace XgpLib.SyncService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        // Configura os serviços do aplicativo
        builder.Services.AddScoped<SyncGenresUseCase>();

        // Registra o Handler como transitório
        builder.Services.AddScoped<AuthenticationHandler>();

        // Registra o serviço de gerenciamento de token
        builder.Services.AddHttpClient<ITokenManagerService, TokenManagerService>();

        // Configura o HttpClient para o IGDBService
        builder.Services.AddHttpClient<IIgdbService, IgdbService>().AddHttpMessageHandler<AuthenticationHandler>();

        // Registra o serviço de sincronização de gêneros
        builder.Services.AddHostedService<IgdbGenresSyncWorker>();

        var host = builder.Build();
        host.Run();
    }
}
