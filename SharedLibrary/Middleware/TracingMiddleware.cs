using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.Services;
using System.Diagnostics;

namespace SharedLibrary.Middleware;

public class TracingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var tracingService = context.RequestServices.GetService<ITracingService>();
        
        if (tracingService != null)
        {
            var tags = new[]
            {
                new KeyValuePair<string, object>("http.method", context.Request.Method),
                new KeyValuePair<string, object>("http.url", context.Request.Path),
                new KeyValuePair<string, object>("http.host", context.Request.Host.ToString())
            };

            using var activity = tracingService.StartActivity("HTTP Request", tags, ActivityKind.Server);
            
            if (activity != null)
            {
                // Add request headers to trace
                foreach (var header in context.Request.Headers)
                {
                    activity.SetTag($"http.request.header.{header.Key.ToLower()}", header.Value.ToString());
                }
                
                try
                {
                    await next(context);
                    
                    // Set response status
                    tracingService.SetStatus("OK");
                    activity.SetTag("http.status_code", context.Response.StatusCode);
                }
                catch (Exception ex)
                {
                    tracingService.SetStatus(ex.Message, ActivityStatusCode.Error);
                    activity.SetTag("error.type", ex.GetType().FullName);
                    activity.SetTag("error.message", ex.Message);
                    throw;
                }
            }
            else
            {
                await next(context);
            }
        }
        else
        {
            await next(context);
        }
    }
}
