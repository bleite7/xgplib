namespace XgpLib.SyncService.Infrastructure.Configuration;

public record IgdbConfiguration
{
    public string AuthUrl { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
}
