using System.Collections.Concurrent;
using DungeonGame.Entities;
using DungeonGame.Entities.States;
using DungeonGame.Events;
using DungeonGame.Extensions;
using DungeonGame.Mods;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DungeonGame;

public class GameManager : IGameManager
{
    private readonly ContentManager _contentManager; 
    private SpriteBatch? _previousSpriteBatch = null!;
    private const int UpdateInterval = 16 * 4;
    private TimeSpan _previousUpdateTime = TimeSpan.Zero;
    private Keys[] _previousKeys = [];
    private readonly ILogger<GameManager> _logger;
    private readonly GameWindow _game;
    private readonly ILoggerFactory _loggerFactory;
    private readonly IServiceProvider _serviceProvider;
    private Player? _player;
    private NonPlayerCharacter? _npc;
    public event EventHandler<KeyboardEventArgs>? KeyPressed;
    public event EventHandler<LogMessageEventArgs>? LogMessage;
    public event EventHandler<MouseEventArgs>? MouseClicked;
    private Scene? _currentScene;
    private MouseState _previousMouseState;
    private ConcurrentDictionary<object, object> _entityProvider = new();


    public GameManager(
        ILogger<GameManager> logger, 
        GameWindow game, 
        ILoggerFactory loggerFactory,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _game = game;
        _loggerFactory = loggerFactory;
        _serviceProvider = serviceProvider;
        _contentManager = _game.Content;
        game.OnUpdate += (_, e) => Update(e.GameTime);
        game.OnDraw += (_, e) => Draw(e.GameTime);
        
        var mods = serviceProvider.GetServices(typeof(IModInitializer));
            
        foreach (var mod in mods)
        {
            if(mod is IModInitializer initializer)
            {
                initializer.Initialize(this);
            }
        }
    }

    private void Draw(GameTime gameTime)
    {
        using var spriteBatch = new SpriteBatch(_game.GraphicsDevice);
        spriteBatch.Begin();
        DoDraw(spriteBatch);
        spriteBatch.End();
    }

    private void DoDraw(SpriteBatch spriteBatch)
    {
        _player ??= CreatePlayer();
        _npc ??= new NonPlayerCharacter(_contentManager.Load<Texture2D>("character_down"));
        _npc.RegisterEvents(this);
        _currentScene ??= new Scene(_game)
        {
            
            Sprites = {_player, _npc}
        };
        _currentScene.Draw(spriteBatch);
    }

        

    private Player CreatePlayer()
    {
        var pixel = LoadPlayerTexture();
        var data = Enumerable.Repeat(Color.White, pixel.Height * pixel.Width).ToArray();
        pixel.SetData(data);
        var player = new Player(pixel, _loggerFactory.CreateLogger<Player>());
        player.RegisterEvents(this);
        return player;
    }

    private Texture2D LoadPlayerTexture()
    {
        var texture = _contentManager.Load<Texture2D>("character_down");
        var newBounds = new Rectangle(0, 0, texture.Width / 2, texture.Height / 2);
        var pixel = new Texture2D(_game.GraphicsDevice, newBounds.Width, newBounds.Height);
        var data = new Color[newBounds.Width * newBounds.Height];
        texture.GetData(0, newBounds, data, 0, data.Length);
        pixel.SetData(data);
        return pixel;
    }

    public void OnKeyboardChange(KeyboardEventArgs e) => KeyPressed?.Invoke(this, e);

    public Game Game => _game;

    public void Update(GameTime gameTime)
    {
        if((gameTime.TotalGameTime - _previousUpdateTime).TotalMilliseconds < UpdateInterval)
        {
            return;
        }
            
        _previousUpdateTime = gameTime.TotalGameTime;
        var mouseState = Mouse.GetState();
        ReadOnlySpan<ButtonState> mouseButtons = [mouseState.LeftButton, mouseState.RightButton, mouseState.MiddleButton];
        ReadOnlySpan<ButtonState> previousMouseButtons = [_previousMouseState.LeftButton, _previousMouseState.RightButton, _previousMouseState.MiddleButton];
        
        if(!mouseButtons.SequenceEqual(previousMouseButtons))
        {
            var entityAtLocation = _currentScene?.GetEntityAtLocation(mouseState.X, mouseState.Y);
            MouseClicked?.Invoke(this, new MouseEventArgs(mouseState.X, mouseState.Y, mouseState, mouseState.GetMouseButtonState(), entityAtLocation));
        }
            
        var state = Keyboard.GetState();
        var currentKeys = state.GetPressedKeys();
            
        if(currentKeys.Length == 0)
        {
            OnKeyboardChange(new KeyboardEventArgs(Keys.None, KeyboardButtonState.None));
            return;
        }
            
        foreach (var key in currentKeys)
        {
            var keyState = key.GetState(state, _previousKeys);
            OnKeyboardChange(new KeyboardEventArgs(key, keyState));
            _logger.LogDebug("{Key} was {State}", key, keyState);
        }
        
        CheckCollision();
            
        _previousKeys = currentKeys;
        _previousMouseState = mouseState;
    }

    private void CheckCollision()
    {
        var entities = _currentScene?.Sprites.OfType<Sprite>().ToList();
        if(entities is null)
        {
            return;
        }

        foreach (var entity in entities)
        {
            var withoutCurrentAndPlayer = entities.Where(x => x != entity && x != _player);
            foreach (var other in withoutCurrentAndPlayer)
            {
                if(entity.GetPosition().Intersects(other.GetPosition()))
                {
                    _logger.LogDebug("{Entity} collided with {Other}", entity, other);
                    var collisionDirection = entity.GetDirectionTo(other);
                    entity.OnCollision(other, collisionDirection);
                    other.OnCollision(entity, collisionDirection.Inverse());
                }
            }
        }
    }

    public async Task RunAsync()
    {
        while (!_game.IsReady())
        {
            _logger.LogDebug("Waiting for graphics device to be initialized");
            await Task.Delay(TimeSpan.FromSeconds(3));
        }
            
        _logger.LogDebug("Graphics device initialized");
        
        _currentScene ??= new Scene(_game);
        _game.Run();
    }

    public T GetService<T>() where T : notnull
    {
        return _serviceProvider.GetRequiredService<T>();
    }

    public T? LoadContent<T>(string name)
    {
        return _contentManager.Load<T>(name);
    }

    public ref Player? GetPlayer()
    {
        _player ??= CreatePlayer();
        return ref _player;
    }

    public void RemoveEntity(Entity entity)
    {
        var success = (_currentScene?.Sprites.Remove(entity)).GetValueOrDefault();
        
        _logger.LogDebug("Entity {Entity} was removed: {Success}", entity, success);
    }
    
    /// <summary>
    /// Creates an entity of type <typeparamref name="T"/> with the specified key
    /// </summary>
    /// <param name="key">The key to use to generate the entity</param>
    /// <param name="acquire">The function to use to generate the entity, if no registered factory method exists.</param>
    /// <typeparam name="T">The entity type to generate</typeparam>
    /// <returns></returns>
    public T CreateEntity<T>(object key, Func<IGameManager, T> acquire) where T : Entity
    {
        if (!_entityProvider.TryGetValue(typeof(T), out var provider))
        {
            _entityProvider.AddOrUpdate(key, acquire, (_, _) => acquire);
        }
        return CreateEntity<T>(key);
    }
    
    public T CreateEntity<T>(object key) where T : Entity
    {
        if (!_entityProvider.TryGetValue(key, out var provider))
        {
            throw new InvalidOperationException($"No provider for entity type {typeof(T)}");
        }
        var entity = provider is Func<IGameManager, T> factory ? factory(this) : (T) provider;
        if(entity is IEventListener listener)
        {
            listener.RegisterEvents(this);
        }
        
        _currentScene?.Sprites.Add(entity);
        return entity;
    }
}