using XgpLib.SyncService.Application.Abstractions.Messaging;

namespace XgpLib.SyncService.Application.Games.SyncGames;

/// <summary>
/// 
/// </summary>
/// <param name="PlatformsIds"></param>
public sealed record SyncGamesCommand(int[] PlatformsIds) : ICommand;
