using DungeonGame.Entities.States;
using DungeonGame.Events;
using Microsoft.Xna.Framework.Input;

namespace DungeonGame.Extensions;

public static class ButtonStateExtensions
{
    public static KeyboardButtonState GetState(this Keys key, KeyboardState state, IEnumerable<Keys> previousKeys)
    {
        return previousKeys.Contains(key) && state.IsKeyUp(key) ? KeyboardButtonState.Released : KeyboardButtonState.Pressed;
    }
    
    
    public static MouseButtonState GetMouseButtonState(this MouseState mouseState)
    {
        return new MouseButtonState(mouseState.RightButton, mouseState.LeftButton, mouseState.MiddleButton);
    }
}