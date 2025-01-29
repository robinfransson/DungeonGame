using DungeonGame.Entities;
using DungeonGame.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;

namespace DungeonGame.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddGraphicsDeviceAccessor(this IServiceCollection services)
    {
        return services.AddSingleton<GraphicsDeviceManagerFactory>(sp => (game) =>
            {
                var manager = sp.GetRequiredService<GraphicsDeviceManager>();
                return manager;
            })
            .AddSingleton<IGraphicsDeviceAccessor, DefaultGraphicsDeviceAccessor>(sp =>
                new DefaultGraphicsDeviceAccessor()
                {
                    LazyGraphicsDevice = new Lazy<GraphicsDevice>(() =>
                    {
                        var manager = sp.GetRequiredService<GraphicsDeviceManager>();
                        return manager.GraphicsDevice;
                    })
                })
            .AddSingleton<GraphicsDeviceManager>(sp =>
            {
                var manager = new GraphicsDeviceManager(sp.GetRequiredService<GameWindow>());
                manager.ApplyChanges();
                return manager;
            });
    }
    
    public static IServiceCollection AddCamera(this IServiceCollection services)
    {
        return services.AddSingleton<CameraWrapper>(sp =>
        {
            var game = sp.GetRequiredService<GameWindow>();
            var graphicsDevice = sp.GetRequiredService<IGraphicsDeviceAccessor>().GraphicsDevice;
            var boxingAdapter = new BoxingViewportAdapter(game.Window, graphicsDevice, graphicsDevice.PresentationParameters.BackBufferWidth, graphicsDevice.PresentationParameters.BackBufferHeight);
            var camera = new OrthographicCamera(boxingAdapter)
            {
                MinimumZoom = //make it so that the camera can't zoom out too far
                    0.1f,
                MaximumZoom = //make it so that the camera can't zoom in too far
                    2f
            };
            return new CameraWrapper(camera);
        });
    }
    public static IServiceCollection AddModSupport(this IServiceCollection services)
    {
        using var provider = services.AddSingleton<ModLoader>()
            .BuildServiceProvider();
        
        var modLoader = provider.GetRequiredService<ModLoader>();
        modLoader.LoadMods(services);
        return services;
    }
    
    
    public static IServiceCollection AddEventHandlers(this IServiceCollection services)
    {
        return services.AddSingleton<EventHandlerProvider>()
            .AddEventHandler<DeathEventHandler, OnDeathEvent>()
            .AddEventHandler<MouseScrollEventHandler, OnMouseScrollEvent>()
            .Configure<GameEventOptions>(options =>
            {
                options.RegisterEventHandler<DeathEventHandler, OnDeathEvent>();
                options.RegisterEventFactory<OnDeathEvent>(manager => new OnDeathEvent(manager.GetPlayer()!));
                options.RegisterRaisingFactory<OnDeathEvent>(manager => manager.GetPlayer()?.Health <= 0);
                options.RegisterEventHandler<MouseScrollEventHandler, OnMouseScrollEvent>();
                options.RegisterEventFactory<OnMouseScrollEvent>(manager =>
                {
                    var mouseState = manager.GetMouseState(true);
                    var previousMouseState = manager.GetPreviousMouseState();
                    var direction = mouseState.DeltaScrollWheelValue > 0 ? Direction.Up : Direction.Down;
                    return new OnMouseScrollEvent(manager.GetPlayer()!, mouseState.DeltaScrollWheelValue, direction);
                });
                options.RegisterRaisingFactory<OnMouseScrollEvent>(manager =>
                {
                    var mouseState = manager.GetMouseState();
                    var previousMouseState = manager.GetPreviousMouseState();
                    var hasScrolledRecently = mouseState.ScrollWheelValue != previousMouseState.ScrollWheelValue;
                    return hasScrolledRecently;
                });
            });

    }

    public static IServiceCollection AddEventHandler<TEventHandler, T>(this IServiceCollection services)
        where TEventHandler : class, IGameEventHandler<T>
        where T : GameEvent
    {
        return services.AddEventHandlerCore<TEventHandler>()
            .AddSingleton<IGameEventHandler<T>, TEventHandler>();
    }


    public static IServiceCollection AddEventHandlerCore<T>(this IServiceCollection services) where T : class, IGameEventHandler
    {
        services.AddOptions<GameEventOptions>();
        return services.AddSingleton<T>()
            .AddSingleton<IGameEventHandler, T>();
    }
}