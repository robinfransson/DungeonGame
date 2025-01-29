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
        var eventQueue = GameEventOptions.GetEventQueue(GameManager);
        
        while(eventQueue.TryDequeue(out var gameEvent))
        {
            SetTimeOnEvent(gameEvent, gameTime, _previousUpdateTime);
            ThreadPool.QueueUserWorkItem(_ => EventHandlerProvider.TriggerEvent(gameEvent), null);
        }
        
        _previousUpdateTime = gameTime.TotalGameTime;
    }
    
    


    private void SetTimeOnEvent(IGameEvent gameEvent, GameTime gameTime, TimeSpan previousUpdateTime)
    {
        gameEvent.Time = gameTime;
        gameEvent.PreviousUpdateTime = previousUpdateTime;
    }
}