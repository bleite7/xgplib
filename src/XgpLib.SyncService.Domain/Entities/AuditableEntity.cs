namespace XgpLib.SyncService.Domain.Entities;

public class AuditableEntity : BaseEntity
{
    public DateTimeOffset CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset ModifiedAt { get; set; }
    public string? LastModifiedBy { get; set; }
}
