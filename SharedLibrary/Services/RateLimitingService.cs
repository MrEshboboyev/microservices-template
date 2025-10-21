using SharedLibrary.Caching;

namespace SharedLibrary.Services;

public class RateLimitingService(ICacheService cacheService) : IRateLimitingService
{
    public bool IsRateLimited(string clientId, int limit, int window)
    {
        var key = $"rate_limit:{clientId}";
        var requests = cacheService.Get<int[]>(key) ?? Array.Empty<int>();
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        // Remove expired requests
        var validRequests = requests.Where(t => now - t < window).ToArray();

        // Check if limit is exceeded
        if (validRequests.Length >= limit)
        {
            return true;
        }

        // Add current request
        var newRequests = validRequests.Append((int)now).ToArray();
        cacheService.Set(key, newRequests, TimeSpan.FromSeconds(window));

        return false;
    }

    public async Task<bool> IsRateLimitedAsync(string clientId, int limit, int window)
    {
        var key = $"rate_limit:{clientId}";
        var requests = await cacheService.GetAsync<int[]>(key) ?? Array.Empty<int>();
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        // Remove expired requests
        var validRequests = requests.Where(t => now - t < window).ToArray();

        // Check if limit is exceeded
        if (validRequests.Length >= limit)
        {
            return true;
        }

        // Add current request
        var newRequests = validRequests.Append((int)now).ToArray();
        await cacheService.SetAsync(key, newRequests, TimeSpan.FromSeconds(window));

        return false;
    }
}
