namespace XgpLib.SyncService.Application.DTOs;

/// <summary>
/// Represents a request to reject a message and send it to the Dead Letter Queue.
/// </summary>
/// <param name="Queue">The original queue name.</param>
/// <param name="Message">The message content to be rejected.</param>
/// <param name="Reason">The reason why the message is being rejected.</param>
public record RejectMessageToDlqRequest(string Queue, string Message, string Reason);
