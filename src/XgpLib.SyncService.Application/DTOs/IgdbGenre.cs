using System.Text.Json.Serialization;

namespace XgpLib.SyncService.Application.DTOs;

public class IgdbGenre
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("slug")]
    public string Slug { get; set; } = string.Empty;
}
