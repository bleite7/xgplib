namespace XgpLib.SyncService.Application.DTOs;

/// <summary>
/// Represents the response after attempting to publish a message.
/// </summary>
/// <param name="Success">Indicates whether the message was successfully published.</param>
/// <param name="ErrorMessage">The error message if the publishing failed; null if successful.</param>
public record PublishMessageResponse(bool Success, string? ErrorMessage = null);
