namespace DungeonGame.Entities;

public interface IUsableItem<T>
{
    Action<T>? OnUse { get; set; }
    public void Use();
}