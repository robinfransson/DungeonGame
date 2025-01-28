using System.Collections.Concurrent;
using DungeonGame.Events;

namespace DungeonGame.Extensions;

public class GameEventOptions
{
    public ConcurrentDictionary<Type, List<Type>> EventHandlers { get; } = new();
    public ConcurrentDictionary<Type, Func<IGameManager, GameEvent>> EventFactory { get; } = new();
    public ConcurrentDictionary<Type, Func<IGameManager, bool>> RaisingFactory { get; } = new();
    
    
    public T CreateEvent<T>(IGameManager manager) where T : GameEvent
    {
        return (T)EventFactory[typeof(T)](manager);
    }

    public IGameEvent CreateEvent(IGameManager manager, Type type)
    {
        return EventFactory[type](manager);
    }
    
    public void RegisterEventHandler<THandler, TEvent>() where THandler : IGameEventHandler<TEvent> where TEvent : GameEvent
    {
        EventHandlers.AddOrUpdate(typeof(TEvent), [typeof(THandler)], (_, list) =>
        {
            list.Add(typeof(THandler));
            return list;
        });
    }
    
    public void RegisterEventFactory<T>(Func<IGameManager, GameEvent> factory) where T : GameEvent
    {
        if(EventFactory.ContainsKey(typeof(T)))
        {
            throw new InvalidOperationException($"Event factory for {typeof(T).Name} already exists");
        }
        
        
        EventFactory[typeof(T)] = factory;
    }

    public void RegisterRaisingFactory<T>(Func<IGameManager, bool> factory) where T : GameEvent
    {
        if(RaisingFactory.ContainsKey(typeof(T)))
        {
            throw new InvalidOperationException($"Raising factory for {typeof(T).Name} already exists");
        }
        
        RaisingFactory[typeof(T)] = factory;
    }
    
    public (bool shouldTrigger, Type? eventType) ShouldRaiseEvent(IGameManager manager)
    {
        foreach(var (type, factory) in RaisingFactory)
        {
            if(factory(manager))
            {
                return (true, type);
            }
        }
        
        return (false, null);
    }
}