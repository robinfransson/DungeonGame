using DungeonGame.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DungeonGame;

public class GameObjectWrapper : GameObject, IEventListener
{
    private readonly Game _gameWindow;
    private static int CurrentIndex { get; set; }
    private static int MaxIndex => GameObjects.Count - 1;
    public static List<GameObject> GameObjects = [];
    private bool Enabled { get; set; } = true;
    public void RegisterEvents(IGameManager gameManager)
    {
        if (HasRegistered)
            return;
        
        gameManager.KeyPressed += ((sender, args) =>
        {
            if (args.Key == Keys.Right)
            {
                if (CurrentIndex < MaxIndex)
                    CurrentIndex++;
            }
            else if (args.Key == Keys.Left)
            {
                if (CurrentIndex > 0)
                    CurrentIndex--;
            }
            else if (args.Key == Keys.Enter)
                Enabled = !Enabled;
        });
    }

    public GameObjectWrapper(Game gameWindow, Rectangle rectangle) : base(gameWindow, rectangle)
    {
        _gameWindow = gameWindow;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (!Enabled || GameObjects.Count < 1)
            return;
        
        var gameObject = GameObjects[CurrentIndex];
        //set in the middle of the screen
        var (roundedX, roundedY) = (_gameWindow.GraphicsDevice.Viewport.Width / 2, _gameWindow.GraphicsDevice.Viewport.Height / 2);
        
        gameObject.SetPosition(new Vector2(roundedX, roundedY));
        gameObject.SetScale(5);
        gameObject.Draw(spriteBatch);
        // base.Draw(spriteBatch);
    }
}