using DungeonGame.Entities.States;
using DungeonGame.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonGame.Entities;

public class NonPlayerCharacter : DestroyableSprite
{
    private readonly Timer _timer;
    public NonPlayerCharacter(Texture2D texture) : base(texture)
    {
        DrawOrder = 1;
        _timer = new Timer(OnTimerElapsed, null, 0, 50);
        Name = "Enemy";
    }

    protected override void OnCollided(Sprite other, Direction direction)
    {
    }

    public override void RegisterEvents(IGameManager gameManager)
    {
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
        var position = Position;
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
}