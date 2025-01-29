using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;

namespace DungeonGame.Events;

public abstract class GameEventHandler<T> : IGameEventHandler<T> where T : GameEvent
{
    protected GameEventHandler(ILogger<T> logger)
    {
        Logger = logger;
    }

    protected ILogger<T> Logger { get; }
    protected TimeSpan LastTimeHandledEvent { get; set; } = TimeSpan.Zero;
    public Task Handle(T gameEvent) => HandleEvent(gameEvent);
    public abstract Task<bool> Predicate(T gameEvent);
    protected abstract Task HandleEvent(T gameEvent);

    public abstract TimeSpan Delay { get; }

    public Task Handle(IGameEvent gameEvent)
    {
        return gameEvent is not T typed ? Task.CompletedTask : HandleEvent(typed);
    }

    public Task<bool> Predicate(IGameEvent gameEvent)
    {
        if(gameEvent is not T typed)
        {
            return Task.FromResult(false);
        }
        
        if(LastTimeHandledEvent == TimeSpan.Zero)
        {
            LastTimeHandledEvent = gameEvent.Time.TotalGameTime;
        }
        var shouldTriggerBasedOnTime = gameEvent.Time.TotalGameTime - LastTimeHandledEvent > Delay;
        if(!shouldTriggerBasedOnTime)
        {
            return Task.FromResult(false);
        }
        
        return Predicate(typed).ContinueWith(result =>
        {
            if (result is {IsCompleted: true, Result: true})
            {
                LastTimeHandledEvent = gameEvent.Time.TotalGameTime;
                return true;
            }

            return false;
        });
    }
}

public abstract class GameEventHandler : GameEventHandler<GameEvent>
{
    protected abstract override Task HandleEvent(GameEvent gameEvent);
    public abstract override Task<bool> Predicate(GameEvent gameEvent);

    protected GameEventHandler(ILogger<GameEvent> logger) : base(logger)
    {
    }
}