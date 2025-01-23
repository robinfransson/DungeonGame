using DungeonGame.Events;

namespace DungeonGame.Entities;

public abstract class Item : IEventListener
{
    public string Name { get; set; }
    
    
    public bool HasRegistered { get; }
    public void RegisterEvents(IGameManager gameManager)
    {
        if(HasRegistered)
        {
            return;
        }
    }

    public abstract void Use();
}