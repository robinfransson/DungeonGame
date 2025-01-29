using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
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

    public EventQueue GetEventQueue(IGameManager gameManager)
    {
        var queue = new EventQueue();
        foreach(var (type, factory) in RaisingFactory)
        {
            if(factory(gameManager))
            {
                queue.Enqueue(CreateEvent(gameManager, type));
            }
        }

        return queue;
    }
}


//create an ienumerable that can only contain one type of each gameEvent in the queue
public class EventQueue : IEnumerable<IGameEvent>
{
    private readonly ConcurrentQueue<IGameEvent> _queue = new();
    private readonly ConcurrentDictionary<Type, IGameEvent> _eventLookup = new();

    public void Enqueue(IGameEvent gameEvent)
    {
        if (_eventLookup.ContainsKey(gameEvent.GetType()))
        {
            return;
        }

        _queue.Enqueue(gameEvent);
        _eventLookup[gameEvent.GetType()] = gameEvent;
    }

    public bool TryDequeue([NotNullWhen(true)] out IGameEvent? gameEvent)
    {
        if(_queue.TryDequeue(out gameEvent))
        {
            _eventLookup.Remove(gameEvent.GetType(), out _);
            return true;
        }

        return false;
    }

    public IEnumerator<IGameEvent> GetEnumerator()
    {
        return _queue.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}