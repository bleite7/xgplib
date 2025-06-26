using System.Net.Http.Json;
using System.Text;

namespace XgpLib.SyncService.Infrastructure.Services;

public class IgdbService : IIgdbService
{
    private readonly HttpClient _httpClient;

    public IgdbService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;

        var baseUrl = configuration["Igdb:BaseUrl"];
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            throw new ArgumentException("The configuration value for 'Igdb:BaseUrl' cannot be null or empty.", nameof(configuration));
        }
        _httpClient.BaseAddress = new Uri(baseUrl);
    }

    public Task<IEnumerable<IgdbGenre>> FetchGenresAsync(CancellationToken cancellationToken) => FetchAllPagedAsync<IgdbGenre>("genres", "name,slug", null, cancellationToken);
    public Task<IEnumerable<IgdbGame>> FetchGamesByPlatformAsync(IEnumerable<int> platformIds, CancellationToken cancellationToken)
    {
        var platformsFilter = string.Join(",", platformIds);
        var whereClause = $"platforms = ({platformsFilter})";
        return FetchAllPagedAsync<IgdbGame>("games", "id,name,slug,platforms", whereClause, cancellationToken);
    }

    private async Task<IEnumerable<T>> FetchAllPagedAsync<T>(string endpoint, string fields, string? whereClause, CancellationToken cancellationToken)
    {
        const int limit = 500;
        int offset = 0;
        var allItems = new List<T>();

        while (true)
        {
            var query = $"fields {fields};{(string.IsNullOrWhiteSpace(whereClause) ? "" : $" where {whereClause};")} limit {limit}; offset {offset};";
            var requestBody = new StringContent(query, Encoding.UTF8, "text/plain");
            var response = await _httpClient.PostAsync(endpoint, requestBody, cancellationToken);

            response.EnsureSuccessStatusCode();

            var items = await response.Content.ReadFromJsonAsync<IEnumerable<T>>(cancellationToken);
            var itemsList = items?.ToList() ?? [];

            if (itemsList.Count == 0)
                break;

            allItems.AddRange(itemsList);
            offset += limit;
        }

        return allItems;
    }
}
