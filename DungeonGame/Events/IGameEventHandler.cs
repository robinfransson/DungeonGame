namespace DungeonGame.Events;

public interface IGameEventHandler
{
    Task Handle(IGameEvent gameEvent);
    Task<bool> Predicate(IGameEvent gameEvent);
}

public interface IGameEventHandler<in T> : IGameEventHandler where T : IGameEvent
{
    Task Handle(T gameEvent);
    Task<bool> Predicate(T gameEvent);
}