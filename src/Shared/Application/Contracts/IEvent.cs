namespace NatrixServices.Shared.Application;

public interface IEvent { }

public interface IEventHandler<TEvent> 
    where TEvent : IEvent
{
    public Task HandleAsync(TEvent e);
}