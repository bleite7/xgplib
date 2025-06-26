using XgpLib.SyncService.Infrastructure.HttpHandlers;

namespace XgpLib.SyncService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        // Configura os servi�os do aplicativo
        builder.Services.AddScoped<SyncGenresUseCase>();

        // Registra o Handler como transit�rio
        builder.Services.AddScoped<AuthenticationHandler>();

        // Registra o servi�o de gerenciamento de token
        builder.Services.AddHttpClient<ITokenManagerService, TokenManagerService>();

        // Configura o HttpClient para o IGDBService
        builder.Services.AddHttpClient<IIgdbService, IgdbService>().AddHttpMessageHandler<AuthenticationHandler>();

        // Registra o servi�o de sincroniza��o de g�neros
        builder.Services.AddHostedService<IgdbGenresSyncWorker>();

        var host = builder.Build();
        host.Run();
    }
}
