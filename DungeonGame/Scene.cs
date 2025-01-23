using DungeonGame.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonGame;

public class Scene : GameComponent
{
    public Texture2D Background { get; }
    public List<Entity> Sprites { get; } = new();
        
    public Scene(Game game) : base(game)
    {
        Background = CreateGrassyBackground();
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Background, Vector2.Zero, Color.White);
        foreach (var sprite in Sprites.OfType<Sprite>().OrderBy(x => x.DrawOrder))
        {
            sprite.Draw(spriteBatch);
        }
    }

    private Texture2D CreateGrassyBackground()
    {
        var windowHeight = this.Game.GraphicsDevice.Viewport.Height;
        var windowWidth = this.Game.GraphicsDevice.Viewport.Width;
        var texture = new Texture2D(this.Game.GraphicsDevice, windowWidth, windowHeight);
        var colors = new Color[windowWidth * windowHeight];
        var random = Random.Shared;
            
            
        for (var i = 0; i < colors.Length; i++)
        {
            int chance = random.Next(0, 100);

            if (chance < 5)
            {
                // Rare darker green
                colors[i] = Color.DarkGreen;
            }
            else if (chance < 15)
            {
                // Slightly more common light green
                colors[i] = Color.LightGreen;
            }
            else
            {
                // Generate more natural random greens for grass
                var red = (byte)random.Next(0, 50);      // Low red component for greener shades
                var green = (byte)random.Next(100, 255); // Vibrant green range
                var blue = (byte)random.Next(0, 100);    // Low blue for earthy greens
                const byte alpha = 255;                         // Opaque grass

                colors[i] = new Color(red, green, blue, alpha);
            }
        }
            
        texture.SetData(colors);
        return texture;
    }

    public Entity? GetEntityAtLocation(int mouseStateX, int mouseStateY)
    {
        var mousePosition = new Point(mouseStateX, mouseStateY);
        return Sprites.FirstOrDefault(x => x.GetPosition().Contains(mousePosition));
    }
}