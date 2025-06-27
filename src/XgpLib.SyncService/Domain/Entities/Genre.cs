using System.ComponentModel.DataAnnotations.Schema;

namespace XgpLib.SyncService.Domain.Entities;

[Table("Genres")]
public class Genre : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    //public string Slug { get; set; } = string.Empty;
}
