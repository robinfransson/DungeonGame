using Microsoft.Extensions.DependencyInjection;

namespace DungeonGame.Mods;

public interface IModInitializer
{
    void Initialize(IGameManager gameManager);
    static abstract IServiceCollection Register(IServiceCollection services);
}