
namespace DungeonGame.Events;

public abstract class GameEvent : IGameEvent
{
    public bool Handled { get; set; }
    public abstract string Name { get; }
}