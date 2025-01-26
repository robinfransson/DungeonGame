namespace DungeonGame;

public interface IRoutineScope
{
    void EndScope();
    void RunScope();
    void Post(SendOrPostCallback d, object? state);
}