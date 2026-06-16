namespace NatrixServices.Shared.Application;

public interface IEventManager
{
    public Task PublishEventAsync<TEvent>(TEvent e)
        where TEvent : IEvent;
}

public class EventManager(IServiceProvider serviceProvider, ILogger<EventManager> logger) : IEventManager
{
    public Task PublishEventAsync<TEvent>(TEvent e)
        where TEvent : IEvent
    {
        if (e == null) throw new ArgumentException("Event is null", nameof(e));

        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider.GetServices<IEventHandler<TEvent>>();

        var tasks = services.Select(async service =>
        {
            try
            {
                await service.HandleAsync(e);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, $"Error occured while executing event handler {service.GetType().Name} for event {e}");
            }
        });

        return Task.WhenAll(tasks);
    }
}