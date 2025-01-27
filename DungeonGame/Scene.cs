using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Channels;
using DungeonGame.Entities;
using DungeonGame.Events;
using DungeonGame.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;

namespace DungeonGame;

public class Scene : GameComponent, IEventListener
{
    private readonly Channel<Entity> _entityRemoveQueue;
    private readonly CameraWrapper _cameraWrapper;
    private readonly GameObjectWrapper Inspector = null!;
    public Texture2D Background { get; protected set; }
    public List<Entity> Sprites { get; } = [];

    public List<GameObject> GameObjects { get; set; } = [];

    public Scene(Game game, Channel<Entity> entityRemoveQueue, CameraWrapper cameraWrapper) : base(game)
    {
        _entityRemoveQueue = entityRemoveQueue;
        _cameraWrapper = cameraWrapper;
        Background = CreateGrassyBackground();
        Inspector = SpawnInspector();
    }

    private GameObjectWrapper SpawnInspector()
    {
        var tileSet = Game.Content.Load<TiledMapTileset>("Tileset/punyworld-overworld-tiles");
        var walls = new List<GameObject>();

        foreach (var tile in tileSet.Tiles.OfType<TiledMapTilesetAnimatedTile>())
        {
            var tileId = tile.LocalTileIdentifier;
            
            var animationCoordinates = tile.AnimationFrames.SelectMany(x => x.GetTextureCoordinates(TiledMapTileFlipFlags.None));
            var rectangle = tileSet.GetTileRegion(tileId);
            var tileTexture = tileSet.Texture;
            var textures = animationCoordinates.Select(x =>
            {
                var texture = new Texture2D(Game.GraphicsDevice, rectangle.Width, rectangle.Height);
                var colors = new Color[rectangle.Width * rectangle.Height];
                tileTexture.GetData(0, rectangle, colors, 0, colors.Length);
                texture.SetData(colors);
                return texture;
            }).ToList();
            
            
            
            var wall = new GameObject(Game, rectangle)
            {
                HasCollision = true,
                Animations = textures
            };
            walls.Add(wall);
        }
        GameObjectWrapper.GameObjects = walls;
        return new GameObjectWrapper(Game, new Rectangle(0, 0, 0, 0));
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
        
        foreach (var gameObject in GameObjects)
        {
            gameObject.Draw(spriteBatch);
        }
        Inspector.Draw(spriteBatch);
        
        
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
        Inspector.RegisterEvents(gameManager);
    }

    private void OnViewportChange(object? sender, ViewportChangedEventArgs e)
    {
        Background = CreateGrassyBackground();
    }
}