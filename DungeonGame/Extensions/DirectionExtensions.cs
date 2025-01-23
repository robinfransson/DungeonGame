using DungeonGame.Entities;

namespace DungeonGame.Extensions;

public static class DirectionExtensions
{
    public static Direction Inverse(this Direction direction) => direction switch
    {
        Direction.None => Direction.None,
        Direction.UpLeft => Direction.DownRight,
        Direction.Up => Direction.Down,
        Direction.UpRight => Direction.DownLeft,
        Direction.Right => Direction.Left,
        Direction.DownRight => Direction.UpLeft,
        Direction.Down =>  Direction.Up,
        Direction.DownLeft => Direction.UpRight,
        Direction.Left => Direction.Right,
        _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
    };
}