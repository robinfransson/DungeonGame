using Microsoft.Xna.Framework.Input;

namespace DungeonGame.Entities.States;

public struct MouseButtonState
{
    public ButtonState RightButton { get; }
    public ButtonState LeftButton { get; }
    public ButtonState MiddleButton { get; }

    public MouseButtonState(ButtonState rightButton, ButtonState leftButton, ButtonState middleButton)
    {
        RightButton = rightButton;
        LeftButton = leftButton;
        MiddleButton = middleButton;
    }
}