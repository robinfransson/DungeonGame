using DungeonGame.Entities;

namespace DungeonGame.Events;

public class DeathEventHandler : GameEventHandler<OnDeathEvent>
{
    public override Task<bool> Predicate(OnDeathEvent gameEvent) => Task.FromResult(gameEvent.Entity is Player);

    protected override Task HandleEvent(OnDeathEvent gameEvent)
    {
        if(gameEvent.Entity is not Player player)
        {
            return Task.CompletedTask;
        }

        player.SetScale(0.1f);
        return Task.CompletedTask;
    }
}