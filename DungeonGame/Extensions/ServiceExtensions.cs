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
            var camera = new OrthographicCamera(boxingAdapter);
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
            .Configure<GameEventOptions>(options =>
            {
                options.RegisterEventHandler<DeathEventHandler, OnDeathEvent>();
                options.RegisterEventFactory<OnDeathEvent>(manager => new OnDeathEvent(manager.GetPlayer()!));
                options.RegisterRaisingFactory<OnDeathEvent>(manager => manager.GetPlayer()?.Health <= 0);
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