using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace SharedLibrary.HealthChecks;

public class DatabaseHealthCheck<TContext>(
    TContext context
) : IHealthCheck where TContext : DbContext
{
    private readonly TContext _context = context;

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check if database is accessible
            var canConnect = await _context.Database.CanConnectAsync(cancellationToken);
            
            if (canConnect)
            {
                return HealthCheckResult.Healthy("Database is accessible");
            }
            
            return HealthCheckResult.Unhealthy("Database is not accessible");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Database health check failed", ex);
        }
    }
}
