using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using XgpLib.SyncService.Infrastructure.Configuration;

namespace XgpLib.SyncService.Infrastructure.Services;

public class TokenManagerService(
    HttpClient httpClient,
    IOptions<IgdbConfiguration> configuration) : ITokenManagerService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly IgdbConfiguration _configuration = configuration.Value;
    private static readonly SemaphoreSlim Semaphore = new(1, 1);

    public async Task<string> GetValidTokenAsync(CancellationToken cancellationToken)
    {
        if (TokenCache.IsTokenValid())
        {
            return TokenCache.GetToken();
        }

        await Semaphore.WaitAsync(cancellationToken);
        try
        {
            if (TokenCache.IsTokenValid())
            {
                return TokenCache.GetToken();
            }

            var clientId = _configuration.ClientId;
            var clientSecret = _configuration.ClientSecret;
            var authUrl = _configuration.AuthUrl;

            var requestUrl = $"{authUrl}?client_id={clientId}&client_secret={clientSecret}&grant_type=client_credentials";
            var response = await _httpClient.PostAsync(requestUrl, null, cancellationToken);

            response.EnsureSuccessStatusCode();

            var tokenResponse = await response.Content.ReadFromJsonAsync<TwitchTokenResponse>(cancellationToken);
            if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.AccessToken))
            {
                throw new InvalidOperationException("Failed to retrieve a valid access token from Twitch");
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
