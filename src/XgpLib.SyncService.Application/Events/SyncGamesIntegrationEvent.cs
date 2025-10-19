namespace XgpLib.SyncService.Application.Events;

/// <summary>
/// 
/// </summary>
/// <param name="PlatformsIds"></param>
public record SyncGamesIntegrationEvent(int[] PlatformsIds);
