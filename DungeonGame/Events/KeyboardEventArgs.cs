using DungeonGame.Entities.States;
using Microsoft.Xna.Framework.Input;

namespace DungeonGame.Events;

public struct KeyboardEventArgs
{
    public Keys Key { get; }
    public KeyboardButtonState State { get; }
    
    public KeyboardEventArgs(Keys key, KeyboardButtonState state)
    {
        Key = key;
        State = state;
    }
}