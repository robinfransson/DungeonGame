using Microsoft.Xna.Framework;

namespace DungeonGame.Extensions;

public static class GameComponentExtensions
{
    public static bool Intersects(this Rectangle rect, Rectangle other)
    {
        return rect.Left < other.Right && rect.Right > other.Left && rect.Top < other.Bottom && rect.Bottom > other.Top;
    }
    
    public static bool IsOffScreen(this Rectangle rect, Rectangle bounds)
    {
        return rect.Left < bounds.Left || rect.Right > bounds.Right || rect.Top < bounds.Top || rect.Bottom > bounds.Bottom;
    }
}