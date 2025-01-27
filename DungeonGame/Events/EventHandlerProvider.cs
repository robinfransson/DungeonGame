namespace DungeonGame.Events;

public class EventHandlerProvider
{
    private readonly IGameEventHandler[] _handlers;

    public EventHandlerProvider(IGameEventHandler[] eventHandlers)
    {
        _handlers = eventHandlers;
    }

    public void TriggerEvent(IGameEvent gameEvent)
    {
        var handlers = _handlers.Where(handler => handler.Predicate(gameEvent).GetAwaiter().GetResult());
            
        foreach (var handler in handlers)
        {
            handler.Handle(gameEvent);
        }
    }
}