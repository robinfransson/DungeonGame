using DungeonGame.Entities.States;
using DungeonGame.Events;
using DungeonGame.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DungeonGame.Entities;

public class Player : DestroyableSprite, IEventListener
{
    private readonly ILogger<Player> _logger;
    private Dictionary<Direction, Dictionary<MovementState, Func<Texture2D>>> _textures = new();
    private MovementState _movementState = MovementState.Idle;

    private List<EquippableItem> _inventory = new();
    

    public Player(Texture2D texture, ILogger<Player> logger) : base(texture)
    {
        _logger = logger;
        Position = Vector2.Zero;
        Direction = Direction.Down;
        Color = Color.White;
        Scale = 0.4f;
    }

    protected override Texture2D Texture => _textures[Direction][_movementState]();

    public override bool ShouldCollideWith(Sprite other)
    {
        return other is not Player;
    }

    protected override void OnCollided(Sprite other, Direction direction)
    {
        other.Push(direction);
    }

    public void RegisterEvents(IGameManager gameManager)
    {
        if(HasRegistered)
        {
            return;
        }
        
        _inventory.Add(
            new RangeWeapon()
            {
                Attack = 10,
                Defense = 0,
                Slot = EquipmentSlot.Weapon,
                Name = "Gun",
                Speed = 10.0F,
                OnUse = (weapon) =>
                {
                    var mouseTarget = gameManager.GetMousePosition();
                    gameManager.CreateEntity<Projectile>(CreatureSpawnMarker.Projectile, manager =>
                    {
                        var direction = GetDirectionTo(mouseTarget);
                       var texture = new Texture2D(manager.Game.GraphicsDevice, 5, 5);
                       var data = Enumerable.Repeat(Color.Black, 25).ToArray();
                       texture.SetData(data);

                       var projectile = new Projectile(texture, direction, Position)
                       {
                           Owner = this,
                           Damage = GetWeapon()?.Attack ?? 1F,
                           Target = mouseTarget
                       };
                       projectile.SetScale(5);
                       projectile.DrawOrder = 20;
                       projectile.OnDispose = () =>
                       {
                           gameManager.RemoveEntity(projectile);
                       };
                       weapon.Shots.Add(projectile);
                       return projectile;
                    });
                }
            });
        
        gameManager.KeyPressed += OnKeyPressed;
        gameManager.MouseClicked += OnMouseClicked;
        _logger.LogDebug("Registered events, loading textures...");

        InitializeTextures(gameManager);
        HasRegistered = true;
    }

    private void OnMouseClicked(object? sender, MouseEventArgs e)
    {
        _logger.LogDebug("Mouse clicked at {position}", new Vector2(e.X, e.Y));
        
        if(Rectangle.Contains(e.X, e.Y))
        {
            _logger.LogDebug("Player was clicked!");
        }

        if (e.Entity is not null || GetWeapon() is not RangeWeapon rangeWeapon) return;
        _logger.LogDebug("Dealing some damage, using {Weapon}", rangeWeapon.Name);
        rangeWeapon.Use();
    }

    private void InitializeTextures(IGameManager gameManager)
    {
        var directions = Enum.GetValues<Direction>();
        
        foreach (var direction in directions)
        {
            _textures[direction] = new Dictionary<MovementState, Func<Texture2D>>
            {
                [MovementState.Idle] = () => gameManager.LoadContent<Texture2D>($"character_{direction}")!,
                [MovementState.Walking] = () =>
                {
                    var currentState = Random.Shared.Next(0, 2);
                    return currentState == 0 ? gameManager.LoadContent<Texture2D>($"character_{direction}_walking1")! : gameManager.LoadContent<Texture2D>($"character_{direction}_walking2")!;
                }
            };
        }
    }

    private void OnKeyPressed(object? sender, KeyboardEventArgs e)
    {
        
        _movementState = MovementState.Walking;
        var previousDirection = Direction;
        switch (e.Key)
        {
            case Keys.W:
                Position += new Vector2(0, -Speed);
                Direction = Direction.Up;
                break;
            case Keys.S:
                Position += new Vector2(0, Speed);
                Direction = Direction.Down;
                break;
            case Keys.A:
                Position += new Vector2(-Speed, 0);
                Direction = Direction.Left;
                break;
            case Keys.D:
                Position += new Vector2(Speed, 0);
                Direction = Direction.Right;
                break;
            case Keys.Up: 
                Speed += 0.1f;
                break;
            case Keys.Down:
                Speed -= 0.1f;
                break;
            case Keys.None:
            default: 
                _movementState = MovementState.Idle;
                return;
        }
        
        if(previousDirection != Direction)
        {
            _logger.LogDebug("Changed direction to {Direction}", Direction);
            OnChangedDirection();
        }
        _logger.LogDebug("Moved to direction {Direction} at position {Position}", Direction, Position);
    }

    private void OnChangedDirection()
    {
        _logger.LogDebug("Changed direction to {Direction}", Direction);
        _logger.LogDebug("Texture is now {Texture}, using file {file}", Texture, Texture.Name);
    }

    protected override void OnDeath()
    {
        _logger.LogDebug("Player died, game over!");
        Health = 100;
    }

    protected override void OnDamageTaken(int damage)
    {
        _logger.LogDebug("Player took {damage} damage, current health is {health}", damage, Health);
    }

    protected override void OnHealthRestored(int health)
    {
        _logger.LogDebug("Player restored {health} health, current health is {health}", health, Health);
    }

    protected EquippableItem? GetWeapon()
    {
        return _inventory.FirstOrDefault(item => item.Slot == EquipmentSlot.Weapon);
    }
}