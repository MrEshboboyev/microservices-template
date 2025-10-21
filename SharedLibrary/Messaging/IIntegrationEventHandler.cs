namespace SharedLibrary.Messaging;

public interface IIntegrationEventHandler<T> where T : IntegrationEvent
{
    Task Handle(T @event);
}
