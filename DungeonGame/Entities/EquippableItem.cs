namespace DungeonGame.Entities;

public abstract class EquippableItem : Item
{
    public int Attack { get; set; }
    public int Defense { get; set; }
    public float Speed { get; set; }
    public EquipmentSlot Slot { get; set; }
}

public class EquippableItem<TSelf> : EquippableItem, IUsableItem<TSelf> where TSelf : EquippableItem<TSelf>
{
    public Action<TSelf>? OnUse { get; set; }

    public override void Use()
    {
        OnUse?.Invoke((TSelf) this);
    }
}