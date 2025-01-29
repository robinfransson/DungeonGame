using DungeonGame.Entities;
using DungeonGame.Extensions;
using Microsoft.Extensions.Logging;

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

    public override TimeSpan Delay => TimeSpan.FromSeconds(1);

    public DeathEventHandler(ILogger<OnDeathEvent> logger) : base(logger)
    {
    }
}

public class MouseScrollEventHandler : GameEventHandler<OnMouseScrollEvent>
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly CameraWrapper _cameraWrapper;

    public MouseScrollEventHandler(CameraWrapper cameraWrapper, ILogger<OnMouseScrollEvent> logger) : base(logger)
    {
        _cameraWrapper = cameraWrapper;
    }
    
    public override Task<bool> Predicate(OnMouseScrollEvent gameEvent) => Task.FromResult(gameEvent.ScrollDelta > 0);

    protected override Task HandleEvent(OnMouseScrollEvent gameEvent)
    {
        try
        {
            if(!_semaphore.Wait(0))
            {
                return Task.CompletedTask;
            }
            
            Logger.LogInformation("Scrolling in");
            if (gameEvent.Direction is Direction.Down)
            {
                _cameraWrapper.ZoomOut(0.1f);
            }
            else
            {
                _cameraWrapper.ZoomIn(0.1f);
            }
            return Task.CompletedTask;
        }
        finally
        {
            _semaphore.Release();
        }
        
    }

    public override TimeSpan Delay => TimeSpan.FromMilliseconds(100);
}

public class OnMouseScrollEvent : GameEvent
{
    public override string Name => "OnMouseScroll";
    public Entity Entity { get; }
    public float ScrollDelta { get; }
    public float PreviousScrollDelta { get; set; }
    public Direction Direction { get; }

    public OnMouseScrollEvent(Entity entity, float scrollDelta, Direction direction) : base()
    {
        Entity = entity;
        ScrollDelta = scrollDelta;
        Direction = direction;
    }
}