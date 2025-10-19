using System.ComponentModel.DataAnnotations.Schema;

namespace XgpLib.SyncService.Domain.Entities;

/// <summary>
/// 
/// </summary>
public class Game : AuditableEntity
{
    /// <summary>
    /// 
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    public int[] Genres { get; set; } = [];

    /// <summary>
    /// 
    /// </summary>
    [Column(TypeName = "jsonb")]
    public string Data { get; set; } = string.Empty;
}
