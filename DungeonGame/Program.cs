﻿using System;
using System.Threading.Channels;
using DungeonGame.Entities;
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
            AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
            {
                var exception = (Exception)eventArgs.ExceptionObject;
                var stackTrace = new System.Diagnostics.StackTrace(exception);
                Console.WriteLine(exception.ToString());
            };
            var serviceProvider = new ServiceCollection()
                .AddGraphicsDeviceAccessor()
                .AddSingleton<GameWindow>()
                .AddSingleton<Channel<Entity>>(_ => Channel.CreateUnbounded<Entity>(new UnboundedChannelOptions()
                {
                    AllowSynchronousContinuations = true,
                    SingleReader = true,
                    SingleWriter = false
                }))
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

    