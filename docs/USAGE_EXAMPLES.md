# Usage Examples

This document provides examples of how to use the enhanced features of the SharedLibrary in your microservices.

## 🔐 JWT Authentication with Refresh Tokens

```csharp
// In your service
public class AuthService
{
    private readonly IJWTTokenService _tokenService;

    public AuthService(IJWTTokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public async Task<JWTToken> LoginAsync(string username, string password)
    {
        // Validate credentials (simplified)
        if (ValidateCredentials(username, password))
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, "User")
            };

            return _tokenService.GenerateTokens(claims);
        }

        throw new UnauthorizedException("Invalid credentials");
    }

    public async Task<JWTToken> RefreshTokenAsync(string accessToken, string refreshToken)
    {
        // Validate refresh token
        if (!_tokenService.ValidateRefreshToken(refreshToken))
        {
            throw new UnauthorizedException("Invalid refresh token");
        }

        // Get principal from expired token
        var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
        
        // Generate new tokens
        return _tokenService.GenerateTokens(principal.Claims);
    }
}
```

## 📝 Logging with Correlation IDs

```csharp
// In your controller or service
public class UserService
{
    private readonly ILoggingService _loggingService;

    public UserService(ILoggingService loggingService)
    {
        _loggingService = loggingService;
    }

    public async Task<User> GetUserAsync(int id)
    {
        _loggingService.LogInformation("Getting user with ID {UserId}", id);
        
        try
        {
            var user = await _userRepository.FindByIdAsync(id);
            
            if (user == null)
            {
                _loggingService.LogWarning("User with ID {UserId} not found", id);
                throw new NotFoundException("User", id);
            }
            
            _loggingService.LogInformation("Successfully retrieved user {UserName}", user.Name);
            return user;
        }
        catch (Exception ex)
        {
            _loggingService.LogError(ex, "Error retrieving user with ID {UserId}", id);
            throw;
        }
    }
}
```

## 💾 Repository with Specification Pattern

```csharp
// Create a specification
public class ActiveUsersSpecification : Specification<User>
{
    public ActiveUsersSpecification()
    {
        ApplyCriteria(u => u.IsActive);
        ApplyOrderBy(u => u.CreatedDate);
        ApplyPaging(0, 10);
    }
}

// Use in service
public class UserService
{
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<User>> GetActiveUsersAsync()
    {
        var spec = new ActiveUsersSpecification();
        return await _unitOfWork.Repository<User>().ListAsync(spec);
    }

    public async Task<bool> IsEmailUniqueAsync(string email, int? excludeUserId = null)
    {
        var spec = new Specification<User>();
        spec.ApplyCriteria(u => u.Email == email && u.Id != excludeUserId);
        return !await _unitOfWork.Repository<User>().AnyAsync(spec);
    }
}
```

## 🛡️ Circuit Breaker Usage

```csharp
public class ExternalServiceClient
{
    private readonly ICircuitBreaker _circuitBreaker;
    private readonly HttpClient _httpClient;

    public ExternalServiceClient(ICircuitBreaker circuitBreaker, HttpClient httpClient)
    {
        _circuitBreaker = circuitBreaker;
        _httpClient = httpClient;
    }

    public async Task<string> GetDataAsync()
    {
        return await _circuitBreaker.ExecuteAsync(async () =>
        {
            var response = await _httpClient.GetAsync("https://api.example.com/data");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        });
    }
}
```

## 📡 Event Bus Messaging

```csharp
// Define an event
public class UserRegisteredEvent : IntegrationEvent
{
    public int UserId { get; set; }
    public string Email { get; set; }
    public DateTime RegistrationDate { get; set; }
}

// Create an event handler
public class UserRegisteredEventHandler : IIntegrationEventHandler<UserRegisteredEvent>
{
    private readonly IEmailService _emailService;

    public UserRegisteredEventHandler(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task Handle(UserRegisteredEvent @event)
    {
        await _emailService.SendWelcomeEmail(@event.Email);
    }
}

// Publish an event
public class UserService
{
    private readonly IEventBus _eventBus;

    public UserService(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task RegisterUserAsync(UserRegistrationDto dto)
    {
        // Create user logic here
        var user = new User { /* ... */ };
        
        // Publish event
        var userRegisteredEvent = new UserRegisteredEvent
        {
            UserId = user.Id,
            Email = user.Email,
            RegistrationDate = DateTime.UtcNow
        };
        
        await _eventBus.PublishAsync(userRegisteredEvent);
    }
}

// Subscribe to events (typically in Startup/Program.cs)
public void ConfigureServices(IServiceCollection services)
{
    // ... other service registrations
    
    services.AddSingleton<IEventBus, InMemoryEventBus>();
    
    // Subscribe handlers
    var serviceProvider = services.BuildServiceProvider();
    var eventBus = serviceProvider.GetService<IEventBus>();
    eventBus.Subscribe<UserRegisteredEvent, UserRegisteredEventHandler>();
}
```

## 📈 Distributed Tracing

```csharp
public class OrderService
{
    private readonly ITracingService _tracingService;
    private readonly IOrderRepository _orderRepository;

    public OrderService(ITracingService tracingService, IOrderRepository orderRepository)
    {
        _tracingService = tracingService;
        _orderRepository = orderRepository;
    }

    public async Task<Order> ProcessOrderAsync(OrderDto orderDto)
    {
        using var activity = _tracingService.StartActivity("ProcessOrder");
        
        try
        {
            _tracingService.AddTag("order.id", orderDto.Id.ToString());
            _tracingService.AddTag("order.amount", orderDto.Amount.ToString());
            
            // Process order logic
            var order = new Order { /* ... */ };
            
            _tracingService.AddEvent("OrderValidated");
            
            await _orderRepository.CreateAsync(order);
            
            _tracingService.AddEvent("OrderSaved");
            _tracingService.SetStatus("Order processed successfully");
            
            return order;
        }
        catch (Exception ex)
        {
            _tracingService.SetStatus($"Order processing failed: {ex.Message}", ActivityStatusCode.Error);
            throw;
        }
    }
}
```

## 💾 Caching

```csharp
public class ProductService
{
    private readonly ICacheService _cacheService;
    private readonly IProductRepository _productRepository;

    public ProductService(ICacheService cacheService, IProductRepository productRepository)
    {
        _cacheService = cacheService;
        _productRepository = productRepository;
    }

    public async Task<Product> GetProductAsync(int id)
    {
        var cacheKey = $"product_{id}";
        
        // Try to get from cache first
        var cachedProduct = await _cacheService.GetAsync<Product>(cacheKey);
        if (cachedProduct != null)
        {
            return cachedProduct;
        }
        
        // If not in cache, get from database
        var product = await _productRepository.FindByIdAsync(id);
        
        // Cache the result for 1 hour
        if (product != null)
        {
            await _cacheService.SetAsync(cacheKey, product, TimeSpan.FromHours(1));
        }
        
        return product;
    }
    
    public async Task InvalidateProductCacheAsync(int id)
    {
        var cacheKey = $"product_{id}";
        await _cacheService.RemoveAsync(cacheKey);
    }
}
```

## 🛡️ Rate Limiting

Rate limiting is automatically applied through middleware. Configure in `appsettings.json`:

```json
{
  "RateLimiting": {
    "Limit": 100,
    "Window": 60
  }
}
```

This configuration allows 100 requests per 60 seconds per client IP.

## 🏥 Health Checks

Health checks are automatically registered. Access them at `/health` endpoint:

```csharp
// In Program.cs or Startup.cs
app.UseHealthChecks("/health");
```

You can also create custom health checks:

```csharp
public class ExternalServiceHealthCheck : IHealthCheck
{
    private readonly IExternalServiceClient _client;

    public ExternalServiceHealthCheck(IExternalServiceClient client)
    {
        _client = client;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var isHealthy = await _client.IsHealthyAsync();
            
            if (isHealthy)
            {
                return HealthCheckResult.Healthy("External service is healthy");
            }
            
            return HealthCheckResult.Unhealthy("External service is unhealthy");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Health check failed", ex);
        }
    }
}

// Register in DI container
services.AddHealthChecks()
    .AddCheck<ExternalServiceHealthCheck>("external-service");
```

These examples demonstrate how to leverage the enhanced SharedLibrary features to build robust, production-ready microservices.