namespace XgpLib.SyncService.Application.DTOs;

/// <summary>
/// Represents a request to publish a message to a specific topic.
/// </summary>
/// <param name="Topic">The topic to which the message will be published.</param>
/// <param name="Message">The message content to be published.</param>
public record PublishMessageRequest(string Topic, string Message);
