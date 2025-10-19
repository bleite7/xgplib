using System.ComponentModel.DataAnnotations.Schema;

namespace XgpLib.SyncService.Domain.Entities;

/// <summary>
/// 
/// </summary>
public class Genre : AuditableEntity
{
    /// <summary>
    /// 
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    [Column(TypeName = "jsonb")]
    public string Data { get; set; } = string.Empty;
}
