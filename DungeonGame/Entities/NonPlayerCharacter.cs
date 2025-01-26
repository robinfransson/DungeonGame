using DungeonGame.Entities.States;
using DungeonGame.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonGame.Entities;

public class NonPlayerCharacter : DestroyableSprite, IEventListener,IOffScreenEvent
{
    private readonly Timer _timer;
    private Func<IGameManager>? _gameManagerGetter = null!;
    private Viewport? Viewport => _gameManagerGetter?.Invoke().Game.GraphicsDevice.Viewport;
    public NonPlayerCharacter(Texture2D texture) : base(texture)
    {
        DrawOrder = 1;
        _timer = new Timer(OnTimerElapsed, null, 0, 50);
        Name = "Enemy";
    }

    public override bool ShouldCollideWith(Sprite other)
    {
        return other is CollidableSprite;
    }

    protected override void OnCollided(Sprite other, Direction direction)
    {
        if(other is Projectile)
            return;
        //do a switch expression on direction
        other.Push(direction);
    }

    public void RegisterEvents(IGameManager gameManager)
    {
        _gameManagerGetter = () => gameManager;
        gameManager.LogMessage += LogMessage;
        OnDispose = () =>
        {
            gameManager.LogMessage -= LogMessage;
            gameManager.RemoveEntity(this);
            gameManager.CreateEntity<NonPlayerCharacter>(CreatureSpawnMarker.TestMarker, manager =>
            {
                var texture = manager.LoadContent<Texture2D>("character_down")!;
                return new NonPlayerCharacter(texture)
                {
                    Position = new Vector2(Random.Shared.Next(0, 800), Random.Shared.Next(0, 480))
                };
            });
        };
    }

    private void LogMessage(object? sender, LogMessageEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void OnTimerElapsed(object? state)
    {
        var directions = Enum.GetValues<Direction>();
        var direction = directions.ElementAt(Random.Shared.Next(0, directions.Length));
        
        if(!Viewport.HasValue)
            return;
        
        var viewport = Viewport.Value;
        
        var position = Position;
        
        var canMoveToDirection = direction switch
        {
            Direction.Up => position.Y - Speed > 0,
            Direction.Down => position.Y + Speed < viewport.Height - Texture.Height,
            Direction.Left => position.X - Speed > 0,
            Direction.Right => position.X + Speed < viewport.Width - Texture.Width,
            _ => false
        };
        
        if (!canMoveToDirection)
            return;
        
        switch (direction)
        {
            case Direction.Up:
                position += new Vector2(0, -Speed);
                break;
            case Direction.Down:
                position += new Vector2(0, Speed);
                break;
            case Direction.Left:
                position += new Vector2(-Speed, 0);
                break;
            case Direction.Right:
                position += new Vector2(Speed, 0);
                break;
        }

        Position = position;
        Direction = direction;
    }

    protected override void OnDeath()
    {
        this.Dispose();
    }

    protected override void OnDamageTaken(int damage)
    {
        throw new NotImplementedException();
    }

    protected override void OnHealthRestored(int health)
    {
        throw new NotImplementedException();
    }

    public Rectangle Bounds => new Rectangle(Position.ToPoint(), Texture.Bounds.Size);
    public void OnOffScreen()
    {
        this.KeepOnScreen();
    }

    private void KeepOnScreen()
    {
        if (_gameManagerGetter is not { } factory || factory() is not { } gameManager)
            return;
        var viewport = gameManager.Game.GraphicsDevice.Viewport;
        var position = Position;
        if (position.X < 0)
        {
            position.X = 0;
        }

        if (position.Y < 0)
        {
            position.Y = 0;
        }

        if (position.X + Texture.Width > viewport.Width)
        {
            position.X = viewport.Width - Texture.Width;
        }

        if (position.Y + Texture.Height > viewport.Height)
        {
            position.Y = viewport.Height - Texture.Height;
        }

        Position = position;
    }
}