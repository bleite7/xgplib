using System.ComponentModel.DataAnnotations.Schema;

namespace XgpLib.SyncService.Domain.Entities;

public class Game : AuditableEntity
{
    public string Name { get; set; } = string.Empty;

    [Column(TypeName = "jsonb")]
    public string Data { get; set; } = string.Empty;
}
