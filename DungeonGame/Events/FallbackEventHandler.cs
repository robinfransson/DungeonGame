using Microsoft.Extensions.Logging;

namespace DungeonGame.Events;

public class FallbackEventHandler : GameEventHandler
{
    private readonly ILogger<FallbackEventHandler> _logger;

    public FallbackEventHandler(ILogger<FallbackEventHandler> logger)
    {
        _logger = logger;
    }
    
    protected override Task HandleEvent(GameEvent gameEvent)
    {
        _logger.LogDebug("No event handler found for event {Event}", gameEvent.Name);
        
        return Task.CompletedTask;
    }

    public override Task<bool> Predicate(GameEvent gameEvent)
    {
        return Task.FromResult(true);
    }
}