using DungeonGame.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonGame.Entities;

public abstract class Sprite : Entity, IEventListener
{
    protected virtual Texture2D Texture { get; }
    public bool HasRegistered { get; protected set; }
    public bool HasCollisions { get; protected set; }
    public int DrawOrder { get; set; }


    protected Sprite(Texture2D texture) : base(texture.Bounds.Location.ToVector2(), texture.Bounds, Color.White)
    {
        Texture = texture;
        DrawOrder = 0;
    }
    
    public virtual void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Texture, Position, Color);
    }
    
    public void OnCollision(Sprite other, Direction direction)
    {
        if(HasCollisions)
        {
            OnCollided(other, direction);
        }
    }
    
    protected abstract void OnCollided(Sprite other, Direction direction);
    public abstract void RegisterEvents(IGameManager gameManager);
}


public class Projectile : Sprite, IDisposable
{
    public Entity? Owner { get; set; }
    public float Speed { get; set; } = 10.0f;
    public Action? OnDispose { get; set; }
    public float Damage { get; set; } = 1.0f;

    public Projectile(Texture2D texture) : base(texture)
    {
    }

    protected override void OnCollided(Sprite other, Direction direction)
    {
        if(other is DestroyableSprite destroyableSprite)
        {
            destroyableSprite.TakeDamage((int)Damage);
        }
        
        Dispose();
    }
    
    public override void RegisterEvents(IGameManager gameManager)
    {
        
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

    public void Dispose()
    {
        OnDispose?.Invoke();
    }
    
    public void SetDirection(Direction direction)
    {
        Direction = direction;
    }
}