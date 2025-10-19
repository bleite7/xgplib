namespace XgpLib.SyncService.Application.DTOs;

/// <summary>
/// 
/// </summary>
public record TwitchTokenResponse
{
    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = string.Empty;
}
