namespace XgpLib.SyncService.Application.DTOs;

/// <summary>
/// Represents the response after attempting to receive messages.
/// </summary>
/// <param name="Success">Indicates whether messages were successfully received.</param>
/// <param name="Messages">The list of received messages.</param>
/// <param name="ErrorMessage">The error message if receiving failed; null if successful.</param>
public record ReceiveMessagesResponse(bool Success, List<string> Messages, string? ErrorMessage = null);
