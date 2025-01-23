using DungeonGame.Entities;
using DungeonGame.Entities.States;
using Microsoft.Xna.Framework.Input;

namespace DungeonGame.Events;

public struct MouseEventArgs
{
    public int X { get; }
    public int Y { get; }
    public MouseState State { get; }
    public MouseButtonState Buttons { get; }
    public Entity? Entity { get; }

    public MouseEventArgs(int x, int y, MouseState state, MouseButtonState buttons, Entity? entity)
    {
        X = x;
        Y = y;
        State = state;
        Buttons = buttons;
        Entity = entity;
    }
}