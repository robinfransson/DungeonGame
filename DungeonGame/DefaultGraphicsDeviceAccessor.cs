
using Microsoft.Xna.Framework.Graphics;

namespace DungeonGame;

internal class DefaultGraphicsDeviceAccessor : IGraphicsDeviceAccessor
{
    private GraphicsDevice? _graphicsDevice;

    public ref GraphicsDevice? GraphicsDevice
    {
        get
        {
            _graphicsDevice ??= LazyGraphicsDevice.Value;
            return ref _graphicsDevice;
        }
    }


    public Lazy<GraphicsDevice> LazyGraphicsDevice { get; internal set; } = null!;
}