using System.Diagnostics.CodeAnalysis;
using DungeonGame.Entities;
using DungeonGame.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;

namespace DungeonGame;

public interface IGameManager
{
    event EventHandler<KeyboardEventArgs> KeyPressed;
    event EventHandler<LogMessageEventArgs> LogMessage;
    event EventHandler<MouseEventArgs> MouseClicked;
    event EventHandler<ViewportChangedEventArgs> ViewportChanged;
    GameWindow Game { get; }

    void Update(GameTime gameTime);
    Task RunAsync();
    T GetService<T>() where T : notnull;
    T? LoadContent<T>(string name);
    ref Player? GetPlayer();
    MouseStateExtended GetMouseState(bool skipUpdate = false);
    MouseStateExtended GetPreviousMouseState();
    void RemoveEntity(Entity entity);
    T CreateEntity<T>(object key) where T : Entity;
    T CreateEntity<T>(object key, Func<IGameManager, T> acquire) where T : Entity;
    Point GetMousePosition();
    bool TryGetKeyedAsset<T>(object key, [MaybeNullWhen(false)]out T asset);

    TValue AddKeyedAsset<TArg, TKey, TValue>(TKey key, Func<TKey, TArg, TValue> addValueFactory,
        Func<TKey, TValue, TArg, TValue> updateValueFactory, TArg factoryArgument)
        where TKey : notnull
        where TArg : notnull
        where TValue : notnull;
}