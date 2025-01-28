using DungeonGame.Entities;
using Microsoft.Xna.Framework;

namespace DungeonGame;

public class EntityWrapper<T> :  EntityWrapper where T : Entity
{
    private readonly IGameManager _gameManager;
    protected override T Entity { get; }
    public EntityWrapper(IGameManager gameManager, T entity) : base(gameManager.Game, entity)
    {
        Entity = entity;
        _gameManager = gameManager;
    }

    public override void Initialize()
    {
        Entity.Initialize();
    }

    public override void Update(GameTime gameTime)
    {
        Entity.Update(_gameManager);
        base.Update(gameTime);
    }
        
    public override T GetEntity() => Entity;

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (Entity is IDisposable disposable)
        {
            disposable.Dispose();   
        }
    }

    public static implicit operator T(EntityWrapper<T> wrapper) => wrapper.Entity;
}

public abstract class EntityWrapper : GameComponent
{
    protected virtual Entity Entity { get; }

    protected EntityWrapper(Game game, Entity entity) : base(game)
    {
        Entity = entity;
    }

    public abstract Entity GetEntity();
}