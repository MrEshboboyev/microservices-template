using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;

namespace SharedLibrary.Middleware;

public class CorrelationIdMiddleware(RequestDelegate next)
{
    private const string CORRELATION_ID_HEADER = "X-Correlation-ID";

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = GetCorrelationId(context);
        context.Request.Headers[CORRELATION_ID_HEADER] = correlationId;
        context.Response.Headers[CORRELATION_ID_HEADER] = correlationId;

        await next(context);
    }

    private static string GetCorrelationId(HttpContext context)
    {
        // Check if correlation ID already exists in request headers
        if (context.Request.Headers.TryGetValue(CORRELATION_ID_HEADER, out var correlationId))
        {
            return correlationId.ToString();
        }

        // Generate a new correlation ID
        return GenerateCorrelationId();
    }

    private static string GenerateCorrelationId()
    {
        // Generate a cryptographically secure random correlation ID
        var bytes = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(bytes);
        }
        return Convert.ToBase64String(bytes);
    }
}
