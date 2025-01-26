using DungeonGame.Entities.States;
using DungeonGame.Events;
using Microsoft.Xna.Framework.Input;

namespace DungeonGame.Extensions;

public static class ButtonStateExtensions
{
    public static KeyboardButtonState GetState(ref readonly Keys value, KeyboardState state, ref readonly Keys[] previousKeys)
    {
        return previousKeys.Contains(value) && state.IsKeyUp(value) ? KeyboardButtonState.Released : KeyboardButtonState.Pressed;
    }
    
    
    public static MouseButtonState GetMouseButtonState(this MouseState mouseState)
    {
        return new MouseButtonState(mouseState.RightButton, mouseState.LeftButton, mouseState.MiddleButton);
    }
}