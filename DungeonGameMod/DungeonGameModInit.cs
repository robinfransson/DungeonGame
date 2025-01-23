using DungeonGame;
using DungeonGame.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DungeonGameMod;

using DungeonGame.Mods;

public class DungeonGameModInit : IModInitializer
{
    private ILogger<DungeonGameModInit>? _logger;
    
    public void Initialize(IGameManager gameManager)
    {
        _logger = gameManager.GetService<ILogger<DungeonGameModInit>>();
        gameManager.KeyPressed += OnKeyPressed;
    }

    public static IServiceCollection Register(IServiceCollection services)
    {
        return services;
    }

    private void OnKeyPressed(object? sender, KeyboardEventArgs e)
    {
        _logger?.LogDebug("Key Pressed: {Key}, reported from {From}", e.Key, GetType().Name);
    }
}