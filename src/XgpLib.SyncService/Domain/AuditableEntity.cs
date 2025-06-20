namespace XgpLib.SyncService.Domain;

public class AuditableEntity
{
    public DateTimeOffset CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset ModifiedAt { get; set; }
    public string? LastModifiedBy { get; set; }
}
