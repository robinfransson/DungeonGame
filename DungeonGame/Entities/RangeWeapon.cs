using Microsoft.Xna.Framework;

namespace DungeonGame.Entities;

public class RangeWeapon : EquippableItem<RangeWeapon>
{
    public List<Projectile> Shots { get; set; } = [];
    
    public void ShootTowards(Vector2 position)
    {
        foreach (var shot in Shots)
        {
            shot.SetDirection(shot.GetDirectionTo(position.ToPoint()));
        }
    }
}