using System.Net.Http.Json;
using XgpLib.SyncService.Infrastructure.Interfaces.Services;

namespace XgpLib.SyncService.Infrastructure.Services;

public class TokenManagerService(
    HttpClient httpClient,
    IConfiguration configuration) : ITokenManagerService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly IConfiguration _configuration = configuration;
    private static readonly SemaphoreSlim Semaphore = new(1, 1);

    public async Task<string> GetValidTokenAsync(CancellationToken cancellationToken)
    {
        if (TokenCache.IsTokenValid())
        {
            return TokenCache.GetToken();
        }

        // Use semaphore to ensure only one thread can fetch the token at a time
        await Semaphore.WaitAsync(cancellationToken);
        try
        {
            // Double-check if the token is still valid after acquiring the semaphore
            if (TokenCache.IsTokenValid())
            {
                return TokenCache.GetToken();
            }

            var clientId = _configuration["Igdb:ClientId"];
            var clientSecret = _configuration["Igdb:ClientSecret"];
            var authUrl = _configuration["Igdb:AuthUrl"];

            var requestUrl = $"{authUrl}?client_id={clientId}&client_secret={clientSecret}&grant_type=client_credentials";
            var response = await _httpClient.PostAsync(requestUrl, null, cancellationToken);

            response.EnsureSuccessStatusCode();

            var tokenResponse = await response.Content.ReadFromJsonAsync<TwitchTokenResponse>(cancellationToken);
            if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.AccessToken))
            {
                throw new InvalidOperationException("Failed to retrieve a valid token from the response.");
            }

            TokenCache.SetToken(tokenResponse.AccessToken, tokenResponse.ExpiresIn);
            return tokenResponse.AccessToken;
        }
        finally
        {
            Semaphore.Release();
        }
    }
}
