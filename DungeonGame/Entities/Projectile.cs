using DungeonGame.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonGame.Entities;

public class Projectile : CollidableSprite, IOffScreenEvent, IEventListener
{
    private static object _bloodSplatterPointer = new();
    private Stack<BloodEffect> _activeBloodEffects = new();
    private Func<BloodEffect?>? GetRandomBloodEffect;
    public Point Target { get; set; } = Point.Zero;
    public Entity? Owner { get; set; }
    public Action? OnDispose { get; set; }
    public float Damage { get; set; } = 1.0f;
    public Projectile(Texture2D texture, Direction direction, Vector2 origin) : base(texture)
    {
        DrawOrder = -1;
        Direction = direction;
        Position = origin;
        HasCollisions = true;
        Scale = 10f;
    }

    public bool HasCollisions { get; set; }

    public override bool ShouldCollideWith(Sprite other)
    {
        return other != Owner && other is NonPlayerCharacter;
    }

    protected override void OnCollided(Sprite other, Direction direction)
    {
        if(other is DestroyableSprite destroyableSprite && ShouldCollideWith(other))
        {
            destroyableSprite.TakeDamage((int)Damage);
            var bloodEffect = GetRandomBloodEffect?.Invoke();
            
            if(bloodEffect is null)
            {
                return;
            }
            
            var point = other.GetPosition().Location;
            bloodEffect.SetPosition(point.ToVector2());
            _activeBloodEffects.Push(bloodEffect);   
        }
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
        
        foreach (var bloodEffect in _activeBloodEffects)
        {
            bloodEffect.Draw(spriteBatch);
        }
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

    public void RegisterEvents(IGameManager gameManager)
    {
        if(HasRegistered)
        {
            return;
        }

        
        if(!gameManager.TryGetKeyedAsset<BasicTextureCollection>(_bloodSplatterPointer, out _))
        {
            var asset = new BasicTextureCollection();
            asset.Add(BloodEffect.GetRandomTexture2D(gameManager.Game));
            asset.Add(BloodEffect.GetRandomTexture2D(gameManager.Game));
            asset.Add(BloodEffect.GetRandomTexture2D(gameManager.Game));
            asset.Add(BloodEffect.GetRandomTexture2D(gameManager.Game));
            asset.Add(BloodEffect.GetRandomTexture2D(gameManager.Game));
            asset.Add(BloodEffect.GetRandomTexture2D(gameManager.Game));
            asset.Add(BloodEffect.GetRandomTexture2D(gameManager.Game));
            asset.Add(BloodEffect.GetRandomTexture2D(gameManager.Game));
            asset.Add(BloodEffect.GetRandomTexture2D(gameManager.Game));
            asset.Add(BloodEffect.GetRandomTexture2D(gameManager.Game));
            gameManager.AddKeyedAsset<BasicTextureCollection, object, BasicTextureCollection>(
                _bloodSplatterPointer,
                (_, __) => asset,
                (_, collection, __) => collection,
                asset);
        }
        
        GetRandomBloodEffect = () =>
        {
            if(!gameManager.TryGetKeyedAsset<BasicTextureCollection>(_bloodSplatterPointer, out var asset))
            {
                return null;
            }
            var index = new Random().Next(0, asset.Count);
            return new BloodEffect(asset[index]);
        };
    }
}