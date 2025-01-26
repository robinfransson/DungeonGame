using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonGame.Entities;

public abstract class DestroyableSprite : CollidableSprite
{
    private const int Width = 100;
    private Texture2D? _texture;
    private Color[]? _data = null;
    private SemaphoreSlim _semaphore = new(1, 1);
    
    
    protected Action? OnDispose = null;
    public int Health { get; protected set; }
    public int MaxHealth { get; protected set; }

    protected bool IsDisposed { get; private set; } = false;
    
    
    protected virtual Texture2D? HealthBarTexture
    {
        get
        {
            if(_semaphore.CurrentCount == 0)
            {
                return null;
            }
            if (Position.ToPoint() == PreviousPosition && _texture is not null)
            {
                return _texture;
            }
            
            _semaphore.Wait();
            
            var (scopedHealth, scopedMaxHealth)  = (Health, MaxHealth);
            _texture ??= new Texture2D(Texture.GraphicsDevice, Width, 10);
            _data ??= new Color[Width * 10];
            
            var currentHpPct = (float)scopedHealth / scopedMaxHealth;
            var currentHp = (int)(currentHpPct * Width);
            for (var i = 0; i < _data.Length; i++)
            {
                _data[i] = i % Width < currentHp ? Color.Green : Color.Red;
            }
            

            _texture.SetData(_data);
            _semaphore.Release();
            return _texture;
        }
    }

    protected DestroyableSprite(Texture2D texture) : base(texture)
    {
        Health = 100;
        MaxHealth = 100;
    }
    
    public void TakeDamage(int damage)
    {
        Health -= damage;
        if(Health <= 0)
        {
            OnDeath();
        }
    }
    
    public void TakeDamage(EquippableItem weapon)
    {
        var damage = weapon.Attack;
        Health -= damage;
        if(Health <= 0)
        {
            OnDeath();
        }
    }
    
    public void RestoreHealth(int health)
    {
        Health += health;
        if(Health > MaxHealth)
        {
            Health = MaxHealth;
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        
        base.Draw(spriteBatch);
        if(HealthBarTexture is null)
        {
            return;
        }

        var healthBarPositionWithOffset = Position - new Vector2(-(HealthBarTexture.Width / 4), 30);
        spriteBatch.Draw(HealthBarTexture, healthBarPositionWithOffset, Color.White);
    }

    protected abstract void OnDeath();
    protected abstract void OnDamageTaken(int damage);
    protected abstract void OnHealthRestored(int health);

    protected override void OnDisposeSignal()
    {
        if(IsDisposed)
        {
            return;
        }
        
        Health = 0;
        _texture = null;
        OnDispose?.Invoke();
        IsDisposed = true;
    }
}