namespace XgpLib.SyncService.Domain.Entities;

/// <summary>
/// Base class for entities that require auditing fields
/// </summary>
public class AuditableEntity : BaseEntity
{
    /// <summary>
    /// Gets or sets the date and time when the entity was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user or system that created the entity.
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the entity was last modified.
    /// </summary>
    public DateTimeOffset ModifiedAt { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user or system that last modified the entity.
    /// </summary>
    public string? LastModifiedBy { get; set; }
}
