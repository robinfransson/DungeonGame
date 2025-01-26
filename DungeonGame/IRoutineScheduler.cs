namespace DungeonGame;

public interface IRoutineScheduler
{
    void Update();
    void Post(SendOrPostCallback d, object? state);
}