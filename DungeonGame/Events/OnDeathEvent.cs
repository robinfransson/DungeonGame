using DungeonGame.Entities;

namespace DungeonGame.Events;

public class OnDeathEvent : GameEvent
{
    public override string Name => "OnDeath";

    public Entity Entity { get; }
    public OnDeathEvent(Entity entity)
    {
        Entity = entity;
    }
}