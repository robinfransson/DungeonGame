using System.Collections;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonGame.Entities;

public class BasicTextureCollection : IList<Texture2D>
{
    private readonly List<Texture2D> _collection = [];
    
    public IEnumerator<Texture2D> GetEnumerator()
    {
        return _collection.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(Texture2D item)
    {
        _collection.Add(item);
    }

    public void Clear()
    {
        _collection.Clear();
    }

    public bool Contains(Texture2D item)
    {
        return _collection.Contains(item);
    }

    public void CopyTo(Texture2D[] array, int arrayIndex)
    {
        _collection.CopyTo(array, arrayIndex);
    }

    public bool Remove(Texture2D item)
    {
        return _collection.Remove(item);
    }

    public int Count => _collection.Count;
    public bool IsReadOnly => false;
    public int IndexOf(Texture2D item)
    {
        return _collection.IndexOf(item);
    }

    public void Insert(int index, Texture2D item)
    {
        _collection.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
        _collection.RemoveAt(index);
    }

    public Texture2D this[int index] { get => _collection[index]; set => _collection[index] = value; }
}