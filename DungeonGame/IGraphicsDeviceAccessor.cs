
using Microsoft.Xna.Framework.Graphics;

namespace DungeonGame;

public interface IGraphicsDeviceAccessor
{
    ref GraphicsDevice? GraphicsDevice { get; }
}