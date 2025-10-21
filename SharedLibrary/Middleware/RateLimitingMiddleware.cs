using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.Services;

namespace SharedLibrary.Middleware;

public class RateLimitingMiddleware(RequestDelegate next, IConfiguration config)
{
    private readonly int _limit = config.GetValue<int>("RateLimiting:Limit", 100);
    private readonly int _window = config.GetValue<int>("RateLimiting:Window", 60);

    public async Task InvokeAsync(HttpContext context)
    {
        var rateLimitingService = context.RequestServices.GetService<IRateLimitingService>();
        var loggingService = context.RequestServices.GetService<ILoggingService>();
        
        if (rateLimitingService != null)
        {
            // Use IP address as client identifier
            var clientId = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            
            var isRateLimited = await rateLimitingService.IsRateLimitedAsync(clientId, _limit, _window);
            
            if (isRateLimited)
            {
                loggingService?.LogWarning("Rate limit exceeded for client {ClientId}", clientId);
                
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsync("Rate limit exceeded. Please try again later.");
                return;
            }
        }
        
        await next(context);
    }
}
