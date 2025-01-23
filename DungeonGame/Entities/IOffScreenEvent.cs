using Microsoft.Xna.Framework;

namespace DungeonGame.Entities;

public interface IOffScreenEvent
{
    Rectangle Bounds { get; }
    void OnOffScreen();
}