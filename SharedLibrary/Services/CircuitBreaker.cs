using System.Collections.Concurrent;

namespace SharedLibrary.Services;

public class CircuitBreaker(
    int failureThreshold = 5,
    TimeSpan? openTimeout = null
) : ICircuitBreaker
{
    private readonly TimeSpan _openTimeout = openTimeout ?? TimeSpan.FromMinutes(1);
    
    private readonly ConcurrentDictionary<string, CircuitBreakerStateInfo> _circuitBreakers = new ConcurrentDictionary<string, CircuitBreakerStateInfo>();

    public CircuitBreakerState State => GetCircuitBreakerState("default");

    public async Task<T> ExecuteAsync<T>(Func<Task<T>> operation)
    {
        return await ExecuteAsync("default", operation);
    }

    public async Task ExecuteAsync(Func<Task> operation)
    {
        await ExecuteAsync("default", async () =>
        {
            await operation();
            return (object)null;
        });
    }

    private async Task<T> ExecuteAsync<T>(string key, Func<Task<T>> operation)
    {
        var circuitBreaker = GetOrCreateCircuitBreaker(key);

        // Check if circuit breaker is open
        if (circuitBreaker.State == CircuitBreakerState.Open)
        {
            // Check if we should move to half-open state
            if (DateTime.UtcNow >= circuitBreaker.NextAttemptTime)
            {
                circuitBreaker.State = CircuitBreakerState.HalfOpen;
            }
            else
            {
                throw new CircuitBreakerOpenException("Circuit breaker is open");
            }
        }

        try
        {
            var result = await operation();
            
            // On success, reset failure count and close circuit
            circuitBreaker.FailureCount = 0;
            circuitBreaker.State = CircuitBreakerState.Closed;
            
            return result;
        }
        catch (Exception)
        {
            // On failure, increment failure count
            circuitBreaker.FailureCount++;
            
            // If failure threshold is reached, open circuit
            if (circuitBreaker.FailureCount >= failureThreshold)
            {
                circuitBreaker.State = CircuitBreakerState.Open;
                circuitBreaker.NextAttemptTime = DateTime.UtcNow.Add(_openTimeout);
            }
            
            throw;
        }
    }

    private CircuitBreakerStateInfo GetOrCreateCircuitBreaker(string key)
    {
        return _circuitBreakers.GetOrAdd(key, _ => new CircuitBreakerStateInfo
        {
            State = CircuitBreakerState.Closed,
            FailureCount = 0,
            NextAttemptTime = DateTime.UtcNow
        });
    }

    private CircuitBreakerState GetCircuitBreakerState(string key)
    {
        if (_circuitBreakers.TryGetValue(key, out var circuitBreaker))
        {
            return circuitBreaker.State;
        }
        
        return CircuitBreakerState.Closed;
    }
}

public class CircuitBreakerStateInfo
{
    public CircuitBreakerState State { get; set; }
    public int FailureCount { get; set; }
    public DateTime NextAttemptTime { get; set; }
}

public class CircuitBreakerOpenException(string message) : Exception(message)
{
}
