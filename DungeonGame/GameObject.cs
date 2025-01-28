using DungeonGame.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonGame;

public class GameObject : CollidableSprite
{
    private Texture2D _currentTexture = null!;
    public Guid Id { get; } = Guid.NewGuid();
    public bool HasCollision { get; set; }
    protected override Texture2D Texture => _currentTexture;
    protected GraphicsDevice GraphicsDevice => _currentTexture.GraphicsDevice;

    public GameObject(Game gameWindow, Rectangle rectangle) : base(gameWindow, rectangle)
    {
    }
    
    public GameObject(Texture2D texture) : base(texture)
    {
    }

    public override bool ShouldCollideWith(Sprite other) => other is CollidableSprite && HasCollision;

    protected override void OnCollided(Sprite other, Direction direction)
    {
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (Animations.Count < 1)
            return;
        
        var randomTexture = Random.Shared.Next(0, Animations.Count);
        _currentTexture = Animations[randomTexture];
        base.Draw(spriteBatch);
    }
}