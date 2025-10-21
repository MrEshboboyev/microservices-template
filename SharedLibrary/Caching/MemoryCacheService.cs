using Microsoft.Extensions.Caching.Memory;

namespace SharedLibrary.Caching;

public class MemoryCacheService(IMemoryCache memoryCache) : ICacheService
{
    public T Get<T>(string key)
    {
        return memoryCache.Get<T>(key);
    }

    public async Task<T> GetAsync<T>(string key)
    {
        return await Task.FromResult(memoryCache.Get<T>(key));
    }

    public void Set<T>(string key, T value, TimeSpan expiration)
    {
        memoryCache.Set(key, value, expiration);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan expiration)
    {
        memoryCache.Set(key, value, expiration);
        await Task.CompletedTask;
    }

    public void Remove(string key)
    {
        memoryCache.Remove(key);
    }

    public async Task RemoveAsync(string key)
    {
        memoryCache.Remove(key);
        await Task.CompletedTask;
    }

    public bool Exists(string key)
    {
        return memoryCache.TryGetValue(key, out _);
    }

    public async Task<bool> ExistsAsync(string key)
    {
        return await Task.FromResult(memoryCache.TryGetValue(key, out _));
    }
}
