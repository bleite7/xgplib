using System.ComponentModel.DataAnnotations.Schema;

namespace XgpLib.Domain;

public class Game : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Storyline { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public ICollection<Genre> Genres { get; set; } = [];

    [Column(TypeName = "jsonb")]
    public string Data { get; set; } = string.Empty;
}
