namespace XgpLib.SyncService.Domain.Entities;

public class Genre : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    //public string Slug { get; set; } = string.Empty;
}
