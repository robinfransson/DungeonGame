using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Channels;
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
    private readonly IRoutineScheduler _scheduler;
    private readonly Channel<Entity> _entityRemoveQueue;
    private readonly ContentManager _contentManager; 
    private SpriteBatch? _previousSpriteBatch = null!;
    private const int UpdateInterval = 0;
    private TimeSpan _previousUpdateTime = TimeSpan.Zero;
    private Keys[] _previousKeys = [];
    private readonly ILogger<GameManager> _logger;
    private readonly ILoggerFactory _loggerFactory;
    private readonly IServiceProvider _serviceProvider;
    private Player? _player;
    private NonPlayerCharacter? _npc;
    public event EventHandler<KeyboardEventArgs>? KeyPressed;
    public event EventHandler<LogMessageEventArgs>? LogMessage;
    public event EventHandler<MouseEventArgs>? MouseClicked;
    public event EventHandler<ViewportChangedEventArgs>? ViewportChanged
    {
        add => Game.ViewportChanged += value;
        remove => Game.ViewportChanged -= value;
    }

    private Scene? _currentScene;
    private MouseState _previousMouseState;
    private ConcurrentDictionary<object, object> _entityProvider = new();
    private ConcurrentDictionary<object, object> _assetProvider = new();


    public GameManager(
        ILogger<GameManager> logger, 
        GameWindow game, 
        ILoggerFactory loggerFactory,
        IServiceProvider serviceProvider,
        IRoutineScheduler scheduler,
        Channel<Entity> entityRemoveQueue)
    {
        _logger = logger;
        Game = game;
        _loggerFactory = loggerFactory;
        _serviceProvider = serviceProvider;
        _scheduler = scheduler;
        _entityRemoveQueue = entityRemoveQueue;
        _contentManager = Game.Content;
        game.OnUpdate += (_, e) => _scheduler.Post(_ => Update(e.GameTime), e.GameTime);
        game.OnDraw += (_, e) => Draw(e.GameTime);
        
        var mods = serviceProvider.GetServices(typeof(IModInitializer)) as IModInitializer[] ?? [];
            
        foreach (var mod in mods)
        {
            mod.Initialize(this);
        }
    }

    private void Draw(GameTime gameTime)
    {
        if(Game.GraphicsDevice is null)
            return;

        _scheduler.Post(_ =>
        {
            var spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);
            DoDraw(spriteBatch);
            spriteBatch.End();
        }, null);
        
        _scheduler.Update();
    }

    private void DoDraw(SpriteBatch spriteBatch)
    {
        _player ??= CreatePlayer();
        _npc ??= new NonPlayerCharacter(_contentManager.Load<Texture2D>("character_down"));
        _npc.RegisterEvents(this);
        _currentScene ??= new Scene(Game, _entityRemoveQueue)
        {
            
            Sprites = {_player, _npc},
        };

        var offScreenEntities = _currentScene.Sprites
            .Where(entity => entity is IOffScreenEvent offScreenEvent &&
                             offScreenEvent.Bounds.IsOffScreen(Game.GraphicsDevice.Viewport.Bounds))
            .Cast<IOffScreenEvent>();
        foreach (var entity in offScreenEntities)
        {
            entity.OnOffScreen();
        }
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
        var pixel = new Texture2D(Game.GraphicsDevice, newBounds.Width, newBounds.Height);
        var data = new Color[newBounds.Width * newBounds.Height];
        texture.GetData(0, newBounds, data, 0, data.Length);
        pixel.SetData(data);
        return pixel;
    }

    public void OnKeyboardChange(KeyboardEventArgs e) => KeyPressed?.Invoke(this, e);

    public GameWindow Game { get; }

    public void Update(GameTime gameTime)
    {
        _previousUpdateTime = gameTime.TotalGameTime;
        var mouseState = Mouse.GetState();
        ReadOnlySpan<ButtonState> mouseButtons = [mouseState.LeftButton, mouseState.RightButton, mouseState.MiddleButton];
        ReadOnlySpan<ButtonState> previousMouseButtons = [_previousMouseState.LeftButton, _previousMouseState.RightButton, _previousMouseState.MiddleButton];
        
        if(!mouseButtons.SequenceEqual(previousMouseButtons))
        {
            var entityAtLocation = _currentScene?.GetEntityAtLocation(mouseState.X, mouseState.Y);
            OnMouseClick(mouseState, entityAtLocation);
        }

        var state = Keyboard.GetState();
        var pressedKeys = state.GetPressedKeys();
        HandleKeyboardChange(state, pressedKeys);
        _scheduler.Post(_ => CheckCollision(), null);
            
        _previousKeys = pressedKeys;
        _previousMouseState = mouseState;
    }

    private void HandleKeyboardChange(KeyboardState keyboardState, ReadOnlySpan<Keys> keys)
    {
        if(keys.Length == 0)
        {
            _scheduler.Post(_ => OnKeyboardChange(new KeyboardEventArgs(Keys.None, KeyboardButtonState.None)), null);
            OnKeyboardChange(new KeyboardEventArgs(Keys.None, KeyboardButtonState.None));
            return;
        }

        foreach (ref readonly var key in keys)
        {
            var currentKeyState = ButtonStateExtensions.GetState(in key, keyboardState, ref _previousKeys);
            
            var unpinnedKey = key;
            _scheduler.Post(_ => OnKeyboardChange(new KeyboardEventArgs(unpinnedKey, currentKeyState)), null);
        }
    }

    private void OnMouseClick(MouseState mouseState, Entity? entityAtLocation)
    {
        MouseClicked?.Invoke(this, new MouseEventArgs(mouseState.X, mouseState.Y, mouseState, mouseState.GetMouseButtonState(), entityAtLocation));
    }

    private void CheckCollision()
    {
        var entities = _currentScene?.Sprites.OfType<CollidableSprite>().ToList();
        if(entities is null)
        {
            return;
        }

        foreach (var entity in entities)
        {
            var withoutCurrentAndPlayer = entities.Where(x => x != entity && x != _player);
            foreach (var other in withoutCurrentAndPlayer.Where(entity.ShouldCollideWith))
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
        while (!Game.IsReady())
        {
            _logger.LogDebug("Waiting for graphics device to be initialized");
            await Task.Delay(TimeSpan.FromSeconds(3));
        }
            
        _logger.LogDebug("Graphics device initialized");
        
        _currentScene ??= new Scene(Game, _entityRemoveQueue);
        _currentScene.RegisterEvents(this);
        Game.InitializeGame();
        Game.Run();
    }

    public T GetService<T>() where T : notnull
    {
        return _serviceProvider.GetRequiredService<T>();
    }
    
    public bool TryGetKeyedAsset<T>(object key, out T? asset)
    {
        ArgumentNullException.ThrowIfNull(key);
        
        if(_assetProvider.TryGetValue(key, out var serviceObj))
        {
            asset = (T) serviceObj;
            return true;
        }
        
        asset = default;
        return false;
    }
    
    public TValue AddKeyedAsset<TArg, TKey, TValue>(TKey key, Func<TKey, TArg, TValue> addValueFactory,  Func<TKey, TValue, TArg, TValue> updateValueFactory, TArg factoryArgument) 
        where TKey : notnull 
        where TArg : notnull
        where TValue : notnull
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(addValueFactory);
        ArgumentNullException.ThrowIfNull(updateValueFactory);

        return (TValue)_assetProvider.AddOrUpdate(key, (Func<object, TArg, object>) AddValueAsObjArgValue, (Func<object, object, TArg, object>)UpdateValueAsObjectObjectTArgObject, factoryArgument);

        object AddValueAsObjArgValue(object k, TArg v)
        {
            return addValueFactory((TKey) k, v);
        }

        object UpdateValueAsObjectObjectTArgObject(object k, object v, TArg a)
        {
            return updateValueFactory((TKey) k, (TValue) v, a);
        }
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
        var success = (_currentScene?.RemoveEntity(entity)).GetValueOrDefault();
        
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

    public Point GetMousePosition()
    {
        var state = Mouse.GetState();
        return new Point(state.X, state.Y);
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
        
        
        var wrapper = new EntityWrapper<T>(this, entity);
        Game.InitializeComponent(wrapper);
        
        _currentScene?.Sprites.Add(wrapper);
        return entity;
    }
}