namespace XgpLib.SyncService.Application.DTOs;

/// <summary>
/// 
/// </summary>
public record IgdbGame
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
    [JsonPropertyName("storyline")]
    public string Storyline { get; set; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("summary")]
    public string Summary { get; set; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("genres")]
    public int[] Genres { get; set; } = [];
}
