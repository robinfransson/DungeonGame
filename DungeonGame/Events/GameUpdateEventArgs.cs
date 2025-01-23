using Microsoft.Xna.Framework;

namespace DungeonGame.Events;

public struct GameUpdateEventArgs
{
    public GameTime GameTime { get; }
    
    public GameUpdateEventArgs(GameTime gameTime)
    {
        GameTime = gameTime;
    }
}