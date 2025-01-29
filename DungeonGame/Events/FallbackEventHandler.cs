using Microsoft.Extensions.Logging;

namespace DungeonGame.Events;

public class FallbackEventHandler : GameEventHandler
{
    public FallbackEventHandler(ILogger<GameEvent> logger) : base(logger)
    {
    }
    
    protected override Task HandleEvent(GameEvent gameEvent)
    {
        Logger.LogDebug("No event handler found for event {Event}", gameEvent.Name);
        
        return Task.CompletedTask;
    }

    public override TimeSpan Delay => TimeSpan.Zero;

    public override Task<bool> Predicate(GameEvent gameEvent)
    {
        return Task.FromResult(true);
    }
}