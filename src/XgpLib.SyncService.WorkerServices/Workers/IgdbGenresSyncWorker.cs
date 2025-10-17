using XgpLib.SyncService.Application.DTOs;

namespace XgpLib.SyncService.WorkerServices.Workers;

public class IgdbGenresSyncWorker(
    ILogger<IgdbGenresSyncWorker> logger,
    IServiceProvider serviceProvider) : BackgroundService
{
    private static readonly TimeSpan PollingInterval = TimeSpan.FromSeconds(60);
    private const string QueueName = "sync_genres";
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
        logger.LogInformation("Starting genres synchronization at {Time}", startTime);

        await using var scope = serviceProvider.CreateAsyncScope();

        var syncGenresUseCase = scope.ServiceProvider.GetRequiredService<SyncGenresUseCase>();
        var receiveMessagesUseCase = scope.ServiceProvider.GetRequiredService<ReceiveMessagesUseCase>();
        try
        {
            if (!await HasMessageToProcessAsync(receiveMessagesUseCase, stoppingToken))
            {
                LogSkippedSync(startTime);
                return;
            }

            logger.LogInformation("{QueueName} message received. Starting synchronization", QueueName);
            await syncGenresUseCase.ExecuteAsync(stoppingToken);
            logger.LogInformation("Genres synchronization finished successfully at {Time}", DateTimeOffset.UtcNow);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while synchronizing genres: {Message}", ex.Message);
        }
        finally
        {
            LogCompletedSync(startTime);
        }
    }

    private async static Task<bool> HasMessageToProcessAsync(
        ReceiveMessagesUseCase receiveMessagesUseCase,
        CancellationToken stoppingToken)
    {
        var receiveResponse = await receiveMessagesUseCase.ExecuteAsync(
            new ReceiveMessagesRequest(QueueName, MaxMessagesToReceive),
            stoppingToken);

        return receiveResponse.Messages.Count > 0;
    }

    private void LogSkippedSync(DateTimeOffset startTime)
    {
        logger.LogInformation("No {QueueName} message found. Skipping synchronization", QueueName);
        var elapsed = DateTimeOffset.UtcNow - startTime;
        logger.LogInformation("Genres synchronization skipped at {Time} (Elapsed: {Elapsed}", DateTimeOffset.UtcNow, elapsed);
    }

    private void LogCompletedSync(DateTimeOffset startTime)
    {
        var elapsed = DateTimeOffset.UtcNow - startTime;
        logger.LogInformation("Genres synchronization completed at {Time} (Elapsed: {Elapsed})", DateTimeOffset.UtcNow, elapsed);
    }

    #endregion Private Methods
}
