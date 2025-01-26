
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonGame.Entities;

public abstract class CollidableSprite : Sprite
{
    public abstract bool ShouldCollideWith(Sprite other);

    protected CollidableSprite(Texture2D texture) : base(texture)
    {
    }
    
    protected CollidableSprite(Game gameWindow, Rectangle rectangle) : base(gameWindow, rectangle)
    {
    }


    public void OnCollision(Sprite other, Direction direction)
    {
        OnCollided(other, direction);
    }

    protected abstract void OnCollided(Sprite other, Direction direction);
}