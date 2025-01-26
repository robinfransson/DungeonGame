using DungeonGame.Entities;
using Microsoft.Xna.Framework;

namespace DungeonGame;

public abstract class EntityWrapper : GameComponent
{
    protected abstract Entity Entity { get; }
        
    protected EntityWrapper(Game game, Entity entity) : base(game)
    {
    }
        
    public abstract Entity GetEntity();
        
}