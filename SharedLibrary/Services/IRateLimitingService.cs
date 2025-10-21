namespace SharedLibrary.Services;

public interface IRateLimitingService
{
    /// <summary>
    /// Check if a client has exceeded the rate limit
    /// </summary>
    /// <param name="clientId">Client identifier (IP address, user ID, etc.)</param>
    /// <param name="limit">Maximum number of requests allowed</param>
    /// <param name="window">Time window in seconds</param>
    /// <returns>True if rate limit is exceeded, false otherwise</returns>
    bool IsRateLimited(string clientId, int limit, int window);

    /// <summary>
    /// Check if a client has exceeded the rate limit asynchronously
    /// </summary>
    /// <param name="clientId">Client identifier (IP address, user ID, etc.)</param>
    /// <param name="limit">Maximum number of requests allowed</param>
    /// <param name="window">Time window in seconds</param>
    /// <returns>True if rate limit is exceeded, false otherwise</returns>
    Task<bool> IsRateLimitedAsync(string clientId, int limit, int window);
}
