using System.Runtime.CompilerServices;
using System.Threading.Channels;
using DungeonGame.Entities;
using DungeonGame.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonGame;

public class Scene : GameComponent, IEventListener
{
    private readonly Channel<Entity> _entityRemoveQueue;
    public Texture2D Background { get; protected set; }
    public List<Entity> Sprites { get; } = [];

    public List<GameObject> GameObjects { get; set; } = [];

    public Scene(Game game, Channel<Entity> entityRemoveQueue) : base(game)
    {
        _entityRemoveQueue = entityRemoveQueue;
        Background = CreateGrassyBackground();
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        var toRemove = GetEntitiesAsync();
        
        foreach (var entity in toRemove)
        {
            Sprites.Remove(entity);
            Game.Components.Remove(entity);
        }
        spriteBatch.Draw(Background, Vector2.Zero, Color.White);
        foreach (var sprite in Sprites.OfType<Sprite>().OrderBy(x => x.DrawOrder))
        {
            sprite.Draw(spriteBatch);
        }
    }
    
    private IEnumerable<Entity> GetEntitiesAsync()
    {
        
        var cts = new CancellationTokenSource();
        cts.CancelAfter(1000);
        try
        {
            while (_entityRemoveQueue.Reader.TryRead(out var entity))
            {
                yield return entity;
            }
        }
        finally
        {
            cts.Dispose();
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
                var red = (byte) random.Next(0, 50); // Low red component for greener shades
                var green = (byte) random.Next(100, 255); // Vibrant green range
                var blue = (byte) random.Next(0, 100); // Low blue for earthy greens
                const byte alpha = 255; // Opaque grass

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

    public bool RemoveEntity(Entity entity)
    {
        try
        {
            _entityRemoveQueue.Writer.WriteAsync(entity).ConfigureAwait(false).GetAwaiter().GetResult();
            return true;
        }
        catch 
        {
            return false;
        }
    }

    public bool HasRegistered { get; }
    public void RegisterEvents(IGameManager gameManager)
    {
        if (HasRegistered)
        {
            return;
        }
        
        gameManager.Game.ViewportChanged += OnViewportChange;
    }

    private void OnViewportChange(object? sender, ViewportChangedEventArgs e)
    {
        Background = CreateGrassyBackground();
    }
}