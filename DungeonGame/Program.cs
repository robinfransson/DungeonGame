using System;
using DungeonGame.Extensions;
//dep inject
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Xna.Framework;

namespace DungeonGame
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddGraphicsDeviceAccessor()
                .AddSingleton<GameWindow>()
                .AddSingleton<IGameManager, GameManager>()
                .AddSingleton<IRoutineScheduler, DefaultRoutineScheduler>()
                .AddLogging(builder =>
                {
                    builder.AddConsole(options =>
                    {
                        options.QueueFullMode = ConsoleLoggerQueueFullMode.DropWrite;
                    });
                })
                .AddModSupport()
                .BuildServiceProvider();
            var gameManager = serviceProvider.GetRequiredService<IGameManager>();
            await gameManager.RunAsync();
        }
    }
}

    