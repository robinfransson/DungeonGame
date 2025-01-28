using System.ComponentModel;

namespace DungeonGame.Events;

// Marker interface for events
public interface IGameEvent
{
    string Name { get; }
}