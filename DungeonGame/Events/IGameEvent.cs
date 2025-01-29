using System.ComponentModel;
using Microsoft.Xna.Framework;

namespace DungeonGame.Events;

// Marker interface for events
public interface IGameEvent
{
    string Name { get; }
    GameTime Time { get; internal set; }
    TimeSpan PreviousUpdateTime { get; internal set; }
}