namespace XgpLib.SyncService.Application.DTOs;

/// <summary>
/// Represents a request to receive messages from a specific topic.
/// </summary>
/// <param name="Queue">The queue from which messages will be received.</param>
/// <param name="MaxMessages">The maximum number of messages to receive (default: 10).</param>
public record ReceiveMessagesRequest(string Queue, short MaxMessages = 10);
