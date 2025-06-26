using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace XgpLib.SyncService.Infrastructure.Services;

public class IgdbService : IIgdbService
{
    private readonly HttpClient _httpClient;

    public IgdbService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(configuration["Igdb:BaseUrl"]);
    }

    public async Task<IEnumerable<IgdbGenre>> FetchGenresAsync(CancellationToken cancellationToken)
    {
        var requestBody = new StringContent("fields name,slug; limit 500;", Encoding.UTF8, "text/plain");
        var response = await _httpClient.PostAsync("genres", requestBody, cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<IEnumerable<IgdbGenre>>(cancellationToken);
    }
}
