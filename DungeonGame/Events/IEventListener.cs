namespace DungeonGame.Events;

public interface IEventListener
{
    public bool HasRegistered { get; }
    void RegisterEvents(IGameManager gameManager);
}