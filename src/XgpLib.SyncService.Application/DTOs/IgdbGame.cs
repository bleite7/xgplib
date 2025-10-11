using System.Text.Json.Serialization;

namespace XgpLib.SyncService.Application.DTOs;

public record IgdbGame
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("storyline")]
    public string Storyline { get; set; } = string.Empty;

    [JsonPropertyName("summary")]
    public string Summary { get; set; } = string.Empty;

    [JsonPropertyName("genres")]
    public int[] Genres { get; set; } = [];
}
