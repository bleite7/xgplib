namespace XgpLib.SyncService.Infrastructure.Configuration;

/// <summary>
/// 
/// </summary>
public record RabbitMqConfiguration
{
    /// <summary>
    /// 
    /// </summary>
    public string HostName { get; set; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    public string Password { get; set; } = string.Empty;
}
