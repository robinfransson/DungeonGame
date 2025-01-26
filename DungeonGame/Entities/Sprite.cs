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
    public virtual List<Texture2D> Animations { get; set; } = [];
    protected float Scale { get; set; } = 1.0f;

    protected Sprite(
        Game gameWindow,
        Rectangle rectangle) : base(rectangle.Location.ToVector2(), rectangle, Color.White)
    {
        _emptyTexture = new Texture2D(gameWindow.GraphicsDevice, 1, 1);
        _emptyTexture.SetData([Color.Transparent]);
        DrawOrder = 0;
    }

    protected Sprite(Texture2D texture) : base(texture.Bounds.Location.ToVector2(), texture.Bounds, Color.White)
    {
        _emptyTexture = new Texture2D(texture.GraphicsDevice, 1, 1);
        _emptyTexture.SetData([Color.Transparent]);
        _texture2D = texture;
        DrawOrder = 0;
    }
    
    public virtual void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Texture, Position, null, Color, 0, Vector2.Zero, Scale, SpriteEffects.None, DrawOrder);
    }

    
    public void SetScale(float scale)
    {
        Scale = scale;
    }
    protected virtual void OnDisposeSignal() {}
    
    public void Dispose()
    {
        _texture2D = null;
        OnDisposeSignal();
        GC.SuppressFinalize(this);
    }
}