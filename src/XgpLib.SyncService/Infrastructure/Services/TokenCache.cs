namespace XgpLib.SyncService.Infrastructure.Services;

public static class TokenCache
{
    private static string _token = string.Empty;
    private static DateTimeOffset _expiry;

    public static bool IsTokenValid() => !string.IsNullOrEmpty(_token) && _expiry > DateTimeOffset.UtcNow;
    public static string GetToken() => _token;
    public static void SetToken(string token, int expiresIn)
    {
        _token = token;
        _expiry = DateTimeOffset.UtcNow.AddSeconds(expiresIn - 300);
    }
}
