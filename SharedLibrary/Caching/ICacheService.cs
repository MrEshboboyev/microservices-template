namespace SharedLibrary.Caching;

public interface ICacheService
{
    /// <summary>
    /// Get item from cache
    /// </summary>
    /// <typeparam name="T">Type of item</typeparam>
    /// <param name="key">Cache key</param>
    /// <returns>Item from cache or default</returns>
    T Get<T>(string key);

    /// <summary>
    /// Get item from cache asynchronously
    /// </summary>
    /// <typeparam name="T">Type of item</typeparam>
    /// <param name="key">Cache key</param>
    /// <returns>Item from cache or default</returns>
    Task<T> GetAsync<T>(string key);

    /// <summary>
    /// Set item in cache
    /// </summary>
    /// <typeparam name="T">Type of item</typeparam>
    /// <param name="key">Cache key</param>
    /// <param name="value">Item to cache</param>
    /// <param name="expiration">Expiration time</param>
    void Set<T>(string key, T value, TimeSpan expiration);

    /// <summary>
    /// Set item in cache asynchronously
    /// </summary>
    /// <typeparam name="T">Type of item</typeparam>
    /// <param name="key">Cache key</param>
    /// <param name="value">Item to cache</param>
    /// <param name="expiration">Expiration time</param>
    Task SetAsync<T>(string key, T value, TimeSpan expiration);

    /// <summary>
    /// Remove item from cache
    /// </summary>
    /// <param name="key">Cache key</param>
    void Remove(string key);

    /// <summary>
    /// Remove item from cache asynchronously
    /// </summary>
    /// <param name="key">Cache key</param>
    Task RemoveAsync(string key);

    /// <summary>
    /// Check if item exists in cache
    /// </summary>
    /// <param name="key">Cache key</param>
    /// <returns>True if item exists in cache</returns>
    bool Exists(string key);

    /// <summary>
    /// Check if item exists in cache asynchronously
    /// </summary>
    /// <param name="key">Cache key</param>
    /// <returns>True if item exists in cache</returns>
    Task<bool> ExistsAsync(string key);
}
