using System.Net.Http.Headers;

namespace XgpLib.SyncService.Infrastructure.HttpHandlers;

/// <summary>
/// Twitch authentication HTTP handler
/// </summary>
/// <param name="tokenManager"></param>
/// <param name="configuration"></param>
public class TwitchAuthenticationHandler(
    ITokenManagerService tokenManager,
    IConfiguration configuration) : DelegatingHandler
{
    private readonly ITokenManagerService _tokenManager = tokenManager;
    private readonly IConfiguration _configuration = configuration;

    /// <summary>
    /// Sends the HTTP request with Twitch authentication headers
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _tokenManager.GetValidTokenAsync(cancellationToken);
        var clientId = _configuration["Igdb:ClientId"];

        request.Headers.Add("client-id", clientId);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await base.SendAsync(request, cancellationToken);
    }
}
