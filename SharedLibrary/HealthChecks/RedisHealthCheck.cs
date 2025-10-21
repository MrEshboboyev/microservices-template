using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace SharedLibrary.HealthChecks;

public class RedisHealthCheck(
    IConnectionMultiplexer connectionMultiplexer
) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var database = connectionMultiplexer.GetDatabase();
            var result = await database.PingAsync();
            
            return HealthCheckResult.Healthy($"Redis is accessible. Ping time: {result.TotalMilliseconds}ms");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Redis health check failed", ex);
        }
    }
}
