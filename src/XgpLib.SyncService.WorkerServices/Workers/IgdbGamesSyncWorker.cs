using XgpLib.SyncService.Application.Abstractions.Messaging;
using XgpLib.SyncService.Application.DTOs;
using XgpLib.SyncService.Application.Games.SyncGames;

namespace XgpLib.SyncService.WorkerServices.Workers;

public class IgdbGamesSyncWorker(
    ILogger<IgdbGamesSyncWorker> logger,
    IServiceProvider serviceProvider) : BackgroundService
{
    private static readonly TimeSpan PollingInterval = TimeSpan.FromSeconds(5);
    private const string QueueName = "sync";
    private const int MaxMessagesToReceive = 1;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessSyncCycleAsync(stoppingToken);
            await Task.Delay(PollingInterval, stoppingToken);
        }
    }

    #region Private Methods

    private async Task ProcessSyncCycleAsync(CancellationToken stoppingToken)
    {
        var startTime = DateTimeOffset.UtcNow;
        logger.LogInformation("Starting games synchronization at {Time}", startTime);

        await using var scope = serviceProvider.CreateAsyncScope();

        var receiveMessagesUseCase = scope.ServiceProvider.GetRequiredService<ReceiveMessagesUseCase>();
        var syncGamesCommandHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<SyncGamesCommand>>();
        try
        {
            if (!(await HasMessageToProcessAsync(receiveMessagesUseCase, stoppingToken)).HasMessage)
            {
                LogSkippedSync(startTime);
                return;
            }

            logger.LogInformation("{QueueName} message received. Starting synchronization", QueueName);
            var syncGamesCommand = new SyncGamesCommand([3]);
            await syncGamesCommandHandler.HandleAsync(syncGamesCommand, stoppingToken);
            logger.LogInformation("Games synchronization finished successfully at {Time}", DateTimeOffset.UtcNow);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while synchronizing games: {Message}", ex.Message);
        }
        finally
        {
            LogCompletedSync(startTime);
        }
    }

    private async static Task<(bool HasMessage, List<string> Messages)> HasMessageToProcessAsync(
        ReceiveMessagesUseCase receiveMessagesUseCase,
        CancellationToken stoppingToken)
    {
        var receiveResponse = await receiveMessagesUseCase.ExecuteAsync(
            new ReceiveMessagesRequest(QueueName, MaxMessagesToReceive),
            stoppingToken);

        // Return tuple without named elements to match the target type
        return (receiveResponse.Messages.Count > 0, receiveResponse.Messages);
    }

    private void LogSkippedSync(DateTimeOffset startTime)
    {
        logger.LogInformation("No {QueueName} message found. Skipping synchronization", QueueName);
        var elapsed = DateTimeOffset.UtcNow - startTime;
        logger.LogInformation("Games synchronization skipped at {Time} (Elapsed: {Elapsed}", DateTimeOffset.UtcNow, elapsed);
    }

    private void LogCompletedSync(DateTimeOffset startTime)
    {
        var elapsed = DateTimeOffset.UtcNow - startTime;
        logger.LogInformation("Games synchronization completed at {Time} (Elapsed: {Elapsed})", DateTimeOffset.UtcNow, elapsed);
    }

    #endregion Private Methods
}
