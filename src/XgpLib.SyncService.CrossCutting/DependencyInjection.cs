using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using XgpLib.SyncService.Application.Interfaces.Services;
using XgpLib.SyncService.Application.UseCases;
using XgpLib.SyncService.Domain.Interfaces.Repositories;
using XgpLib.SyncService.Infrastructure.Configuration;
using XgpLib.SyncService.Infrastructure.Data;
using XgpLib.SyncService.Infrastructure.Data.Repositories;
using XgpLib.SyncService.Infrastructure.HttpHandlers;
using XgpLib.SyncService.Infrastructure.Interfaces.Services;
using XgpLib.SyncService.Infrastructure.Services;

namespace XgpLib.SyncService.CrossCutting;

public static class DependencyInjection
{
    public static IServiceCollection AddSyncServiceDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        // DbContext
        services.AddDbContext<XgpLibDbContext>(options => options.UseNpgsql(configuration
            .GetConnectionString("XgpLibPostgres"))
            .UseSnakeCaseNamingConvention());

        // Message Broker - RabbitMQ
        services.Configure<RabbitMqConfiguration>(configuration.GetSection("RabbitMq"));
        services.AddScoped<IMessageBrokerService, RabbitMqService>();
        services.AddScoped<PublishMessageUseCase>();

        // Application UseCases
        services.AddScoped<SyncGamesUseCase>();
        services.AddScoped<SyncGenresUseCase>();

        // Infrastructure
        services.AddScoped<TwitchAuthenticationHandler>();
        services.AddScoped<IGameRepository, GameRepository>();
        services.AddScoped<IGenreRepository, GenreRepository>();
        services.AddHttpClient<ITokenManagerService, TokenManagerService>("TokenManagerServiceApi");

        // IGDB Service
        var igdbServiceApiBaseUrl = configuration.GetValue<string>("Igdb:BaseUrl") ?? "";
        services.AddHttpClient<IIgdbService, IgdbService>("IgdbServiceApi", options =>
        {
            options.BaseAddress = new Uri(igdbServiceApiBaseUrl);
        }).AddHttpMessageHandler<TwitchAuthenticationHandler>();

        return services;
    }
}
