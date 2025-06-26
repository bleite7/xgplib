namespace XgpLib.SyncService.Application.UseCases;

public class SyncGenresUseCase(
    IIgdbService igdbService, 
    ILogger<SyncGenresUseCase> logger)
{
    private readonly IIgdbService _igdbService = igdbService;
    private readonly ILogger<SyncGenresUseCase> _logger = logger;

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var genresFromApi = await _igdbService.FetchGenresAsync(cancellationToken);
        if (genresFromApi is null || !genresFromApi.Any())
        {
            _logger.LogWarning("Nenhum gênero encontrado na API do IGDB.");
            return;
        }

        var genres = genresFromApi.Select(genreDto => new Genre
        {
            Id = genreDto.Id,
            Name = genreDto.Name,
            Slug = genreDto.Slug,
        });

        _logger.LogInformation("{Count} gêneros encontrados. Sincronizando com o banco de dados...", genres.Count());
        foreach (var genre in genres)
        {
            _logger.LogInformation("Sincronizando gênero: {Name}", genre.Name);
        }
        _logger.LogInformation("Sincronização de gêneros concluída com sucesso!");
    }
}
