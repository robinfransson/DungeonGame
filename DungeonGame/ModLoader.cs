﻿using System.Reflection;
using System.Text.Json;
using DungeonGame.Mods;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace DungeonGame;

public class ModLoader
{
    private readonly byte[] _bom = [0xEF, 0xBB, 0xBF];
    private readonly ILogger<ModLoader> _logger;

    public ModLoader(ILogger<ModLoader> logger)
    {
        _logger = logger;
    }
        
    public void LoadMods(IServiceCollection serviceCollection)
    {
        // return;
        const string modsPath = "mods";
        // Load mods from /mods folder and initialize them
            
        var path = Path.Combine(Directory.GetCurrentDirectory(), modsPath);
        var directories = Directory.GetDirectories(path);
        List<Type> mods = new();

        foreach (var directory in directories)
        {
            var modJson = Path.Combine(directory, "mod.json");
            
            if(!File.Exists(modJson))
            {
                _logger.LogWarning($"Could not find mod.json in {directory}");
                continue;
            }
            
            ReadOnlySpan<byte> modSettings = File.ReadAllBytes(modJson);
            ReadOnlySpan<byte> utf8Bom = _bom;

            if (modSettings.StartsWith(utf8Bom))
            {
                modSettings = modSettings[utf8Bom.Length..];
            }
            var mod = JsonSerializer.Deserialize<ModSettings>(modSettings);
            if(mod == null)
            {
                continue;
            }
                
            var assembly = Assembly.LoadFrom(Path.Combine(directory, mod.Entry));
            var initializerType = assembly.GetTypes().FirstOrDefault(x => x.IsAssignableTo(typeof(IModInitializer)));
                
            if(initializerType == null)
            {
                Console.WriteLine($"Could not find mod initializer for mod {mod.Name}, it does not export a type that implements IModInitializer");
                continue;
            }
                
            RegisterServicesForMod(initializerType, serviceCollection);
            mods.Add(initializerType);
        }
            
        serviceCollection.TryAddEnumerable(mods.Select(mod => ServiceDescriptor.Singleton(typeof(IModInitializer), mod)));
        serviceCollection.TryAddEnumerable(mods.Select(mod => ServiceDescriptor.Singleton(typeof(IModInitializer), mod)));
        serviceCollection.AddTransient<IModInitializer[]>(x =>
        {
             var smods = x.GetServices(typeof(IModInitializer)).ToArray() as IModInitializer[] ?? [];
             return smods;
        });
    }


    // ReSharper disable once SuggestBaseTypeForParameter
    private void RegisterServicesForMod(Type type, IServiceCollection serviceCollection)
    {
        var method = typeof(ModLoader).GetMethod(nameof(RegisterServicesForModImpl), BindingFlags.Static | BindingFlags.NonPublic);
        var genericMethod = method!.MakeGenericMethod(type);
        genericMethod.Invoke(this, [serviceCollection]);
    }
        
        
    private static void RegisterServicesForModImpl<T>(IServiceCollection serviceCollection) where T : IModInitializer
    {
        T.Register(serviceCollection);
    }
}