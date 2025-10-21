namespace SharedLibrary.Messaging;

public interface IEventBus
{
    /// <summary>
    /// Publish an event to the event bus
    /// </summary>
    /// <typeparam name="T">Type of event</typeparam>
    /// <param name="event">Event to publish</param>
    Task PublishAsync<T>(T @event) where T : IntegrationEvent;

    /// <summary>
    /// Subscribe to an event type with a handler
    /// </summary>
    /// <typeparam name="T">Type of event</typeparam>
    /// <typeparam name="TH">Type of handler</typeparam>
    void Subscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>;

    /// <summary>
    /// Unsubscribe from an event type
    /// </summary>
    /// <typeparam name="T">Type of event</typeparam>
    /// <typeparam name="TH">Type of handler</typeparam>
    void Unsubscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>;
}
