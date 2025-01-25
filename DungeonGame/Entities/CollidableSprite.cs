
using Microsoft.Xna.Framework.Graphics;

namespace DungeonGame.Entities;

public abstract class CollidableSprite : Sprite
{
    public abstract bool ShouldCollideWith(Sprite other);

    protected CollidableSprite(Texture2D texture) : base(texture)
    {
    }


    public void OnCollision(Sprite other, Direction direction)
    {
        OnCollided(other, direction);
    }

    protected abstract void OnCollided(Sprite other, Direction direction);
}