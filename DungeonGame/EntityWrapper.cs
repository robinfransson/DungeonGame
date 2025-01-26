using DungeonGame.Entities;
using Microsoft.Xna.Framework;

namespace DungeonGame;

public abstract class EntityWrapper : GameComponent
{
    protected virtual Entity Entity { get; }

    protected EntityWrapper(Game game, Entity entity) : base(game)
    {
        Entity = entity;
    }
        
    public abstract Entity GetEntity();
        
}