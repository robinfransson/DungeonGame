namespace DungeonGame;

public interface IRoutineScheduler
{
    void Update();
    void Post(SendOrPostCallback d, object? state);
    void Post(IRoutineScope scope);
    IRoutineScope CreateScope();
}