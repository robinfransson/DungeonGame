namespace DungeonGame;

public class RoutineScope : IRoutineScope
{
    private readonly List<KeyValuePair<SendOrPostCallback, object>> _continuations = [];
    private readonly IRoutineScheduler _parent;
    

    public RoutineScope(IRoutineScheduler parent)
    {
        _parent = parent;
    }
    
    public void EndScope()
    {
        _parent.Post(this);
    }

    public void RunScope()
    {
        var queue = _continuations.ToArray();
        _continuations.Clear();
        
        foreach (var (callback, state) in queue)
        {
            callback(state);
        }
    }

    public void Post(SendOrPostCallback d, object? state)
    {
        state ??= new object();

        _continuations.Add(new KeyValuePair<SendOrPostCallback, object>(d, state));
    }
}