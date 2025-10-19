namespace XgpLib.SyncService.Application.DTOs;

/// <summary>
/// 
/// </summary>
public record IgdbGenre
{
    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("id")]
    public long Id { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("slug")]
    public string Slug { get; set; } = string.Empty;
}
