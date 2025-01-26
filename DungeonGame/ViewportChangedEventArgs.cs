using Microsoft.Xna.Framework.Graphics;

namespace DungeonGame;

public struct ViewportChangedEventArgs
{
    public ViewportChangedEventArgs(ViewportContainer viewport)
    {
        Viewport = viewport;
    }

    public ViewportContainer Viewport;
        
    public struct ViewportContainer
    {
        public ViewportContainer(Viewport viewport)
        {
                
        }
         
        public Viewport Viewport { get; }   
    }
}