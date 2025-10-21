namespace SharedLibrary.Services;

public interface ICircuitBreaker
{
    /// <summary>
    /// Execute a function with circuit breaker protection
    /// </summary>
    /// <typeparam name="T">Return type</typeparam>
    /// <param name="operation">Function to execute</param>
    /// <returns>Result of the function</returns>
    Task<T> ExecuteAsync<T>(Func<Task<T>> operation);

    /// <summary>
    /// Execute an action with circuit breaker protection
    /// </summary>
    /// <param name="operation">Action to execute</param>
    Task ExecuteAsync(Func<Task> operation);

    /// <summary>
    /// Get the current state of the circuit breaker
    /// </summary>
    CircuitBreakerState State { get; }
}

public enum CircuitBreakerState
{
    Closed,    // Normal operation
    Open,      // Circuit breaker is open, requests are failing fast
    HalfOpen   // Circuit breaker is half-open, testing if service is available
}
