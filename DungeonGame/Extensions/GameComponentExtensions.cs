using Microsoft.Xna.Framework;

namespace DungeonGame.Extensions;

public static class GameComponentExtensions
{
    public static bool IsOffScreen(this Rectangle rect, Rectangle bounds)
    {
        return rect.Left < bounds.Left || rect.Right > bounds.Right || rect.Top < bounds.Top || rect.Bottom > bounds.Bottom;
    }
}