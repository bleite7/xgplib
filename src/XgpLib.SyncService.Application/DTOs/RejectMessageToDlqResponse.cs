namespace XgpLib.SyncService.Application.DTOs;

/// <summary>
/// Represents the response after attempting to reject a message to DLQ.
/// </summary>
/// <param name="Success">Indicates whether the message was successfully rejected to DLQ.</param>
/// <param name="DlqName">The name of the Dead Letter Queue where the message was sent.</param>
/// <param name="ErrorMessage">Error message if the operation failed.</param>
public record RejectMessageToDlqResponse(bool Success, string DlqName, string? ErrorMessage = null);
