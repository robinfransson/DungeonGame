using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DungeonGame.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddGraphicsDeviceAccessor(this IServiceCollection services)
    {
        return services.AddSingleton<GraphicsDeviceManagerFactory>(_ => (game) => new GraphicsDeviceManager(game));
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