namespace XgpLib.SyncService.Infrastructure.Services;

/// <summary>
/// 
/// </summary>
public static class TokenCache
{
    private static string _token = string.Empty;
    private static DateTimeOffset _expiry;

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static bool IsTokenValid() => !string.IsNullOrEmpty(_token) && _expiry > DateTimeOffset.UtcNow;

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static string GetToken() => _token;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="token"></param>
    /// <param name="expiresIn"></param>
    public static void SetToken(string token, int expiresIn)
    {
        _token = token;
        _expiry = DateTimeOffset.UtcNow.AddSeconds(expiresIn - 300);
    }
}
