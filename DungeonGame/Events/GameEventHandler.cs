namespace DungeonGame.Events;

public abstract class GameEventHandler<T> : IGameEventHandler<T> where T : GameEvent
{
    public Task Handle(T gameEvent) => HandleEvent(gameEvent);
    public abstract Task<bool> Predicate(T gameEvent);
    protected abstract Task HandleEvent(T gameEvent);

    public Task Handle(IGameEvent gameEvent)
    {
        return gameEvent is not T typed ? Task.CompletedTask : HandleEvent(typed);
    }

    public Task<bool> Predicate(IGameEvent gameEvent)
    {
        return gameEvent is not T typed ? Task.FromResult(false) : Predicate(typed);
    }
}

public abstract class GameEventHandler : GameEventHandler<GameEvent>
{
    protected abstract override Task HandleEvent(GameEvent gameEvent);
    public abstract override Task<bool> Predicate(GameEvent gameEvent);
}