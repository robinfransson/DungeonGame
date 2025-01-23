using Microsoft.Xna.Framework;

namespace DungeonGame.Events;

public struct GameDrawEventArgs
{
    public GameTime GameTime { get; }

    public GameDrawEventArgs(GameTime gameTime)
    {
        GameTime = gameTime;
    }
}