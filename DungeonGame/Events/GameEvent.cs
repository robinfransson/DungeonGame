
using Microsoft.Xna.Framework;

namespace DungeonGame.Events;

public abstract class GameEvent : IGameEvent
{
    public bool Handled { get; set; }
    public abstract string Name { get; }
    public GameTime Time { get; set; } = null!;
    public TimeSpan PreviousUpdateTime { get; set; }
}