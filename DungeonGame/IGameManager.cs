using DungeonGame.Entities;
using DungeonGame.Events;
using Microsoft.Xna.Framework;

namespace DungeonGame;

public interface IGameManager
{
    event EventHandler<KeyboardEventArgs> KeyPressed;
    event EventHandler<LogMessageEventArgs> LogMessage;
    event EventHandler<MouseEventArgs> MouseClicked;
    
    Game Game { get; }
    
    void Update(GameTime gameTime);
    Task RunAsync();
    T GetService<T>() where T : notnull;
    T? LoadContent<T>(string name);
    ref Player? GetPlayer();
    void RemoveEntity(Entity entity);
    T CreateEntity<T>(object key) where T : Entity;
    T CreateEntity<T>(object key, Func<IGameManager, T> acquire) where T : Entity;
}