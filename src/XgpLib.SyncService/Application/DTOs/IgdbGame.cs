using System.Text.Json.Serialization;

namespace XgpLib.SyncService.Application.DTOs;

public class IgdbGame
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}
