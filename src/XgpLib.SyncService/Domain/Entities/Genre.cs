using System.ComponentModel.DataAnnotations.Schema;

namespace XgpLib.SyncService.Domain.Entities;

public class Genre : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;

    [Column(TypeName = "jsonb")]
    public string Data { get; set; } = string.Empty;
}
