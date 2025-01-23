using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.Xna.Framework;

namespace DungeonGame.Entities;

public class RangeWeapon : EquippableItem<RangeWeapon>
{
    public RangeWeapon()
    {
        Shots.CollectionChanged += (sender, args) =>
        {
            if (args.Action != NotifyCollectionChangedAction.Remove) return;
            foreach (var shot in args.OldItems?.OfType<Projectile>() ?? [])
            {
                shot.Dispose();
            }
        };
    }
    
    public ObservableCollection<Projectile> Shots { get; set; } = [];

    public void ShootTowards(Vector2 position)
    {
        foreach (var shot in Shots)
        {
            shot.SetDirection(shot.GetDirectionTo(position.ToPoint()));
        }
    }
}