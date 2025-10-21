using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Context;
using System.Security.Claims;

namespace SharedLibrary.Services;

public class LoggingService(IHttpContextAccessor httpContextAccessor) : ILoggingService
{
    private readonly ILogger _logger = Log.ForContext<LoggingService>();

    public string CorrelationId => GetCorrelationId();

    private string GetCorrelationId()
    {
        // Try to get correlation ID from header
        var correlationId = httpContextAccessor?.HttpContext?.Request?.Headers["X-Correlation-ID"].FirstOrDefault();
        
        // If not found, try to get from trace identifier
        if (string.IsNullOrEmpty(correlationId))
        {
            correlationId = httpContextAccessor?.HttpContext?.TraceIdentifier;
        }
        
        // If still not found, generate a new one
        if (string.IsNullOrEmpty(correlationId))
        {
            correlationId = Guid.NewGuid().ToString();
        }
        
        return correlationId;
    }

    public void LogInformation(string message, params object[] args)
    {
        using (LogContext.PushProperty("CorrelationId", CorrelationId))
        {
            using (LogContext.PushProperty("UserId", GetUserId()))
            {
                _logger.Information(message, args);
            }
        }
    }

    public void LogWarning(string message, params object[] args)
    {
        using (LogContext.PushProperty("CorrelationId", CorrelationId))
        {
            using (LogContext.PushProperty("UserId", GetUserId()))
            {
                _logger.Warning(message, args);
            }
        }
    }

    public void LogError(Exception exception, string message, params object[] args)
    {
        using (LogContext.PushProperty("CorrelationId", CorrelationId))
        {
            using (LogContext.PushProperty("UserId", GetUserId()))
            {
                _logger.Error(exception, message, args);
            }
        }
    }

    public void LogDebug(string message, params object[] args)
    {
        using (LogContext.PushProperty("CorrelationId", CorrelationId))
        {
            using (LogContext.PushProperty("UserId", GetUserId()))
            {
                _logger.Debug(message, args);
            }
        }
    }

    public void LogTrace(string message, params object[] args)
    {
        using (LogContext.PushProperty("CorrelationId", CorrelationId))
        {
            using (LogContext.PushProperty("UserId", GetUserId()))
            {
                _logger.Verbose(message, args);
            }
        }
    }

    public void LogCritical(Exception exception, string message, params object[] args)
    {
        using (LogContext.PushProperty("CorrelationId", CorrelationId))
        {
            using (LogContext.PushProperty("UserId", GetUserId()))
            {
                _logger.Fatal(exception, message, args);
            }
        }
    }

    private string GetUserId()
    {
        var userId = httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return string.IsNullOrEmpty(userId) ? "anonymous" : userId;
    }
}
