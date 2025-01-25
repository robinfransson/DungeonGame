using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonGame.Entities;

public class Projectile : CollidableSprite, IOffScreenEvent
{
    public Point Target { get; set; } = Point.Zero;
    public Entity? Owner { get; set; }
    public Action? OnDispose { get; set; }
    public float Damage { get; set; } = 1.0f;

    public Projectile(Texture2D texture, Direction direction, Vector2 origin) : base(texture)
    {
        DrawOrder = int.MaxValue;
        Direction = direction;
        Position = origin;
        HasCollisions = true;
    }

    public bool HasCollisions { get; set; }

    public override bool ShouldCollideWith(Sprite other)
    {
        return other != Owner;
    }

    protected override void OnCollided(Sprite other, Direction direction)
    {
        if(other is DestroyableSprite destroyableSprite)
        {
            destroyableSprite.TakeDamage((int)Damage);
        }
        
        Dispose();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        Position += Direction switch
        {
            Direction.Up => new Vector2(0, -Speed),
            Direction.Down => new Vector2(0, Speed),
            Direction.Left => new Vector2(-Speed, 0),
            Direction.Right => new Vector2(Speed, 0),
            _ => Vector2.Zero
        };
        base.Draw(spriteBatch);
    }

    protected override void OnDisposeSignal()
    {
        OnDispose?.Invoke();
    }
    
    public void SetDirection(Direction direction)
    {
        Direction = direction;
    }

    public Rectangle Bounds => new Rectangle(Position.ToPoint(), Texture.Bounds.Size);

    public void OnOffScreen()
    {
        Dispose();
    }
}