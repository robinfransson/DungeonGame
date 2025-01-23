using DungeonGame.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonGame.Entities;

public abstract class Sprite : Entity, IEventListener, IDisposable
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

    protected virtual void OnDisposeSignal() {}
    
    public void Dispose()
    {
        Texture.Dispose();
        OnDisposeSignal();
        GC.SuppressFinalize(this);
    }
}