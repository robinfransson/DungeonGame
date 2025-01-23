using Microsoft.Xna.Framework;

namespace DungeonGame.Extensions;

public static class GameComponentExtensions
{
    public static bool Intersects(this Rectangle rect, Rectangle other)
    {
        return rect.Left < other.Right && rect.Right > other.Left && rect.Top < other.Bottom && rect.Bottom > other.Top;
    }
}