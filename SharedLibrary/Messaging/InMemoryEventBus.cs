using Microsoft.Extensions.DependencyInjection;

namespace SharedLibrary.Messaging;

public class InMemoryEventBus(IServiceProvider serviceProvider) : IEventBus
{
    private readonly Dictionary<Type, List<Type>> _handlers = [];
    private readonly List<Type> _eventTypes = [];

    public async Task PublishAsync<T>(T @event) where T : IntegrationEvent
    {
        var eventType = @event.GetType();
        
        if (_handlers.TryGetValue(eventType, out List<Type> handlers))
        {
            foreach (var handlerType in handlers)
            {
                using var scope = serviceProvider.CreateScope();
                var handler = scope.ServiceProvider.GetService(handlerType);
                
                if (handler != null)
                {
                    var handlerTypeDefinition = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                    var method = handlerTypeDefinition.GetMethod("Handle");
                    await (Task)method?.Invoke(handler, [@event]);
                }
            }
        }
    }

    public void Subscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        var eventType = typeof(T);
        var handlerType = typeof(TH);

        if (!_eventTypes.Contains(eventType))
        {
            _eventTypes.Add(eventType);
        }

        if (!_handlers.TryGetValue(eventType, out List<Type> value))
        {
            value = [];
            _handlers[eventType] = value;
        }

        if (value.Any(s => s == handlerType))
        {
            throw new ArgumentException(
                $"Handler Type {handlerType.Name} already registered for '{eventType.Name}'");
        }

        value.Add(handlerType);
    }

    public void Unsubscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        var eventType = typeof(T);
        var handlerType = typeof(TH);

        if (_handlers.TryGetValue(eventType, out List<Type> handlers))
        {
            var handlerToRemove = handlers.FirstOrDefault(s => s == handlerType);
            
            if (handlerToRemove != null)
            {
                handlers.Remove(handlerToRemove);
            }
        }
    }
}
