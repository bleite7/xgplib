using XgpLib.SyncService.Application.Abstractions.Messaging;
using XgpLib.SyncService.Application.DTOs;
using XgpLib.SyncService.Application.Events;
using XgpLib.SyncService.Application.Genres.Commands.SyncGenres;
using XgpLib.SyncService.Domain.Entities;

namespace XgpLib.SyncService.WorkerServices.Workers;

public class IgdbGenresSyncWorker(
    ILogger<IgdbGenresSyncWorker> logger,
    IServiceProvider serviceProvider) : BackgroundService
{
    private static readonly TimeSpan PollingInterval = TimeSpan.FromSeconds(5);
    private const int MaxMessagesToReceive = 1;
    private static readonly string Queue = Queues.SyncGenres;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await ProcessSyncCycleAsync(cancellationToken);
            await Task.Delay(PollingInterval, cancellationToken);
        }
    }

    #region Private Methods

    private async Task ProcessSyncCycleAsync(CancellationToken cancellationToken)
    {
        var startTime = DateTimeOffset.UtcNow;
        logger.LogInformation("Starting genres synchronization at {Time}", startTime);

        await using var scope = serviceProvider.CreateAsyncScope();

        var receiveMessagesUseCase = scope.ServiceProvider.GetRequiredService<ReceiveMessagesUseCase>();
        var processMessageWithDlqUseCase = scope.ServiceProvider.GetRequiredService<ProcessMessageWithDlqUseCase<SyncGenresIntegrationEvent>>();
        var syncGenresCommandHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<SyncGenresCommand>>();
        try
        {
            var (HasMessage, Messages) = await HasMessageToProcessAsync(receiveMessagesUseCase, cancellationToken);
            if (!HasMessage)
            {
                LogSkippedSync(startTime);
                return;
            }

            logger.LogInformation("{QueueName} message received. Starting synchronization", Queue);

            var success = await processMessageWithDlqUseCase.ExecuteAsync(
                Queue,
                Messages[0],
                async (gamesSyncEvent, cancellationToken) =>
                {
                    await syncGenresCommandHandler.HandleAsync(
                        new SyncGenresCommand(),
                        cancellationToken);
                },
                cancellationToken);

            if (success)
            {
                logger.LogInformation("Genres synchronization finished successfully at {Time}", DateTimeOffset.UtcNow);
            }
            else
            {
                logger.LogWarning("Genres synchronization failed and message was sent to DLQ");
            }
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

    private async static Task<(bool HasMessage, List<string> Messages)> HasMessageToProcessAsync(
        ReceiveMessagesUseCase receiveMessagesUseCase,
        CancellationToken stoppingToken)
    {
        var receiveResponse = await receiveMessagesUseCase.ExecuteAsync(
            new ReceiveMessagesRequest(Queue, MaxMessagesToReceive),
            stoppingToken);

        // Return tuple without named elements to match the target type
        return (receiveResponse.Messages.Count > 0, receiveResponse.Messages);
    }

    private void LogSkippedSync(DateTimeOffset startTime)
    {
        logger.LogInformation("No {QueueName} message found. Skipping synchronization", Queue);
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
