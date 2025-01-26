namespace DungeonGame;

public class DefaultRoutineScheduler : SynchronizationContext, IRoutineScheduler
{
    private readonly List<KeyValuePair<SendOrPostCallback, object>> _continuations = [];
    
    public override void Post(SendOrPostCallback d, object? state)
    {
        state ??= new object();

        _continuations.Add(new KeyValuePair<SendOrPostCallback, object>(d, state));
    }

    public void Post(IRoutineScope scope)
    {
        _continuations.Add(new KeyValuePair<SendOrPostCallback, object>(state =>
        {
            scope.RunScope();
        }, scope));
    }

    public IRoutineScope CreateScope()
    {
        return new RoutineScope(this);
    }


    public void Update()
    {
        var queue = _continuations.ToArray();
        _continuations.Clear();
        
        foreach (var (callback, state) in queue)
        {
            var context = Current;
            try
            {
                SetSynchronizationContext(this);
                callback(state);   
            }
            finally
            {
                SetSynchronizationContext(context);
            }
        }
        
    }
}