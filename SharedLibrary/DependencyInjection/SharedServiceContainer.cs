using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Serilog;
using SharedLibrary.Caching;
using SharedLibrary.HealthChecks;
using SharedLibrary.Messaging;
using SharedLibrary.Middleware;
using SharedLibrary.Repositories;
using SharedLibrary.Services;

namespace SharedLibrary.DependencyInjection;

public static class SharedServiceContainer
{
    public static IServiceCollection AddSharedService<TContext>
        (this IServiceCollection services, IConfiguration config, string fileName) where TContext : DbContext
    {
        // Add Generic Database context
        services.AddDbContext<TContext>(option =>
        {
            option.UseNpgsql(config.GetConnectionString("PostgresConnection"));
        });
        
        // Add HTTP context accessor for correlation ID and logging
        services.AddHttpContextAccessor();
        
        // Add memory cache
        services.AddMemoryCache();
        
        // Add distributed tracing
        services.AddOpenTelemetry()
            .WithTracing(builder =>
            {
                builder
                    .AddSource("SharedLibrary")
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();
            })
            .WithMetrics(builder =>
            {
                builder
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();
            });
        
        // configure serilog logging
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Debug()
            .WriteTo.Console()
            .WriteTo.File(
                path: $"{fileName}-.text",
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} CorrelationId: {CorrelationId} UserId: {UserId} {NewLine}{Exception}",
                rollingInterval: RollingInterval.Day)
            .CreateLogger();
            
        // Add JWT Authentication Scheme 
        services.AddJWTAuthenticationScheme(config);
        
        // Register JWT Token Service
        services.AddScoped<IJWTTokenService, JWTTokenService>();
        
        // Register Logging Service
        services.AddScoped<ILoggingService, LoggingService>();
        
        // Register Cache Service (default to memory cache)
        services.AddScoped<ICacheService, MemoryCacheService>();
        
        // Register Rate Limiting Service
        services.AddScoped<IRateLimitingService, RateLimitingService>();
        
        // Register Circuit Breaker
        services.AddSingleton<ICircuitBreaker, CircuitBreaker>();
        
        // Register Tracing Service
        services.AddSingleton<ITracingService, TracingService>();
        
        // Register In-Memory Event Bus
        services.AddSingleton<IEventBus, InMemoryEventBus>();
        
        // Register Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        // Add health checks
        services.AddHealthChecks()
            .AddCheck<DatabaseHealthCheck<TContext>>("database");
            
        return services;
    }
    
    public static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration config)
    {
        var redisConnectionString = config.GetConnectionString("RedisConnection");
        if (!string.IsNullOrEmpty(redisConnectionString))
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
            });
            
            // Override cache service with Redis implementation
            services.AddScoped<ICacheService, RedisCacheService>();
        }
        
        return services;
    }
    
    public static IServiceCollection AddRedisHealthCheck(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck<RedisHealthCheck>("redis");
            
        return services;
    }
    
    public static IApplicationBuilder UseSharedPolicies(this IApplicationBuilder app)
    {
        // Use Correlation ID middleware
        app.UseMiddleware<CorrelationIdMiddleware>();
        
        // Use Tracing middleware
        app.UseMiddleware<TracingMiddleware>();
        
        // Use Rate Limiting middleware
        app.UseMiddleware<RateLimitingMiddleware>();
        
        // Use Global Exception
        app.UseMiddleware<GlobalException>();
        // Register Middleware to block all outsiders API calls
        app.UseMiddleware<ListenToOnlyApiGateway>();
        return app;
    }
}
