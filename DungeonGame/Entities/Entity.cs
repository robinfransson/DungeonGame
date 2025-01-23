using Microsoft.Xna.Framework;

namespace DungeonGame.Entities;

public abstract class Entity
{
    protected Point PreviousPosition = Point.Zero;
    protected Vector2 Position { get; set; }
    protected Color Color { get; set; } 
    protected float Speed {get; set;} = 10.0f;
    protected Direction Direction { get; set; } = Direction.Down;
    protected Rectangle Rectangle { get; set; }
    public string Name { get; protected set; } = string.Empty;

    protected Entity(Vector2 position, Rectangle bounds, Color color)
    {
        Position = position;
        Color = color;
        Rectangle = bounds;
    }


    public Rectangle GetPosition()
    {
        return new Rectangle(Position.ToPoint(), Rectangle.Size);
    }
    
    public Direction GetDirectionTo(Entity other) => other.PreviousPosition switch
    {
        { X: var x, Y: var y } when x < Position.X => Direction.Left,
        { X: var x, Y: var y } when x > Position.X => Direction.Right,
        { Y: var y } when y < Position.Y => Direction.Up,
        { Y: var y } when y > Position.Y => Direction.Down,
        _ => Direction
    };
    
    public Direction GetDirectionTo(Point point) => point switch
    {
        { X: var x, Y: var y } when x < Position.X => Direction.Left,
        { X: var x, Y: var y } when x > Position.X => Direction.Right,
        { Y: var y } when y < Position.Y => Direction.Up,
        { Y: var y } when y > Position.Y => Direction.Down,
        _ => Direction
    };
}