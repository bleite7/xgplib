namespace XgpLib.SyncService.Infrastructure.Configuration;

/// <summary>
/// 
/// </summary>
public record IgdbConfiguration
{
    /// <summary>
    /// 
    /// </summary>
    public string AuthUrl { get; set; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    public string ClientSecret { get; set; } = string.Empty;
}
