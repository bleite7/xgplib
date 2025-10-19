namespace XgpLib.SyncService.Application.Abstractions.Messaging;

/// <summary>
/// 
/// </summary>
public interface ICommand : IBaseCommand { }

/// <summary>
/// 
/// </summary>
/// <typeparam name="TResponse"></typeparam>
public interface ICommand<TResponse> : IBaseCommand { }

/// <summary>
/// 
/// </summary>
public interface IBaseCommand { }
