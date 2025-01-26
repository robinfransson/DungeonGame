using DungeonGame.Entities;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonGame;

public class GameObject : CollidableSprite
{
    public bool HasCollision { get; set; }
    
    public GameObject(Texture2D texture) : base(texture)
    {
    }

    public override bool ShouldCollideWith(Sprite other) => other is CollidableSprite && HasCollision;

    protected override void OnCollided(Sprite other, Direction direction)
    {
        throw new NotImplementedException();
    }
}