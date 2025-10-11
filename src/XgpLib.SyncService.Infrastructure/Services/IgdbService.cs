using System.Net.Http.Json;
using System.Text;
using XgpLib.SyncService.Application.DTOs;
using XgpLib.SyncService.Application.Interfaces.Services;

namespace XgpLib.SyncService.Infrastructure.Services;

public class IgdbService(HttpClient httpClient) : IIgdbService
{
    private readonly HttpClient _httpClient = httpClient;

    public Task<IEnumerable<IgdbGenre>> FetchGenresAsync(CancellationToken cancellationToken = default) => FetchAllPagedAsync<IgdbGenre>("genres", "name,slug", null, cancellationToken);
    public Task<IEnumerable<IgdbGame>> FetchGamesByPlatformAsync(IEnumerable<int> platformIds, CancellationToken cancellationToken = default)
    {
        var platformsFilter = string.Join(",", platformIds);
        var whereClause = $"platforms = ({platformsFilter})";
        var fields = "name,slug,summary,storyline,platforms,genres";

        return FetchAllPagedAsync<IgdbGame>("games", fields, whereClause, cancellationToken);
    }

    private async Task<IEnumerable<T>> FetchAllPagedAsync<T>(string endpoint, string fields, string? whereClause, CancellationToken cancellationToken = default)
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
