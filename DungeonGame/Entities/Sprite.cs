using DungeonGame.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonGame.Entities;

public abstract class Sprite : Entity, IDisposable
{

    private readonly Texture2D _emptyTexture;
    
    private Texture2D? _texture2D;
    protected virtual Texture2D Texture => _texture2D ?? _emptyTexture;
    public bool HasRegistered { get; protected set; }
    public int DrawOrder { get; set; }


    protected Sprite(Texture2D texture) : base(texture.Bounds.Location.ToVector2(), texture.Bounds, Color.White)
    {
        _emptyTexture = new Texture2D(texture.GraphicsDevice, 1, 1);
        _emptyTexture.SetData([Color.Transparent]);
        _texture2D = texture;
        DrawOrder = 0;
    }
    
    public virtual void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Texture, Position, Color);
    }

    protected virtual void OnDisposeSignal() {}
    
    public void Dispose()
    {
        _texture2D = null;
        OnDisposeSignal();
        GC.SuppressFinalize(this);
    }
}