using DungeonGame.Events;
using DungeonGame.Extensions;
using Microsoft.Xna.Framework;

namespace DungeonGame;

internal class EventRaiser : GameComponent
{
    private TimeSpan _previousUpdateTime;
    private const int IntervalInMilliseconds = 1000;
    
    internal required IGameManager GameManager { get; init; }
    internal required GameEventOptions GameEventOptions { get; init; }
    internal required EventHandlerProvider EventHandlerProvider { get; init; }
    
    public EventRaiser(Game game) : base(game)
    {
    }
    
    public override void Update(GameTime gameTime)
    {
        if((gameTime.TotalGameTime - _previousUpdateTime).TotalMilliseconds < IntervalInMilliseconds)
        {
            return;
        }
        
        
        if(GameEventOptions.ShouldRaiseEvent(GameManager) is (true, Type eventType))
        {
            var evt = GameEventOptions.CreateEvent(GameManager, eventType);
            EventHandlerProvider.TriggerEvent(evt);
        }
        
        base.Update(gameTime);
        _previousUpdateTime = gameTime.TotalGameTime;
    }
}