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
            var boxingAdapter = new BoxingViewportAdapter(game.Window, graphicsDevice, 800, 480, 800, 480);
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
}

public class CameraWrapper
{
    public static CameraWrapper Empty { get; } = new (null!);
    private readonly OrthographicCamera _camera;

    public CameraWrapper(OrthographicCamera camera)
    {
        _camera = camera;
    }

    public Matrix GetViewMatrix()
    {
        return _camera.GetViewMatrix();
    }

    public void Move(Vector2 direction)
    {
        _camera.Move(direction);
    }

    public void ZoomIn(float delta)
    {
        _camera.ZoomIn(delta);
    }
    
    public void ZoomOut(float delta)
    {
        _camera.ZoomOut(delta);
    }
    
    public void SetZoom(float delta)
    {
        _camera.Zoom = delta;
    }

    public void Rotate(float delta)
    {
        _camera.Rotate(delta);
    }

    public void LookAt(Vector2 position)
    {
        _camera.LookAt(position);
    }
}