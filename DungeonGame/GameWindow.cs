using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DungeonGame.Entities;
using DungeonGame.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonGame
{
    public class GameWindow : Game
    {
        private readonly GraphicsDeviceManagerFactory _factory;
        internal event EventHandler<GameUpdateEventArgs>? OnUpdate; 
        internal event EventHandler<GameDrawEventArgs>? OnDraw;
        internal event EventHandler<ViewportChangedEventArgs>? ViewportChanged;
        private bool IsInitialized => this.GraphicsDevice != null;

        public GameWindow(GraphicsDeviceManagerFactory factory) :base()
        {
            _factory = factory;
            Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
            
            //set fullscreen windowed
            this.Window.IsBorderless = true;
            this.Window.Position = Point.Zero;
            this.Window.AllowUserResizing = true;
            this.Window.IsBorderless = true;
            this.Window.AllowAltF4 = true;
        }

        protected override void Draw(GameTime gameTime)
        {
            if (IsInitialized)
            {
                OnDraw?.Invoke(this, new GameDrawEventArgs(gameTime));
            }
            
            base.Draw(gameTime);
        }

        protected override void Update(GameTime gameTime)
        {
            if (IsInitialized)
            {
                OnUpdate?.Invoke(this, new GameUpdateEventArgs(gameTime));

            }
            base.Update(gameTime);
        }

        public bool IsReady()
        {
            try
            {
                SetGraphicsDevice();
                this.RunOneFrame();
                return this.GraphicsDevice != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void SetGraphicsDevice()
        {
            try
            {
                if(this.GraphicsDevice is null) return;

                _factory(this);
            }
            catch
            {
                _factory(this);
            }
        }

        public void InitializeGame()
        {
            //get the max resolution of the monitor
            var (width, height) = (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
            this.GraphicsDevice.Viewport = new Viewport(0, 0, width, height);
            this.GraphicsDevice.PresentationParameters.BackBufferWidth = width;
            this.GraphicsDevice.PresentationParameters.BackBufferHeight = height;
            this.GraphicsDevice.PresentationParameters.IsFullScreen = true;
            this.GraphicsDevice.PresentationParameters.DeviceWindowHandle = this.Window.Handle;
            GraphicsDevice.Reset();
            ViewportChanged?.Invoke(this, new ViewportChangedEventArgs(new ViewportChangedEventArgs.ViewportContainer(GraphicsDevice.Viewport)));
            
            this.IsMouseVisible = true;
        }

        protected virtual void OnViewportChanged(ViewportChangedEventArgs e)
        {
            ViewportChanged?.Invoke(this, e);
        }

        public void InitializeComponent<T>(EntityWrapper<T> entity) where T : Entity
        {
            Components.Add(entity);
            entity.Initialize(); ;
        }
    }

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
        
        public static implicit operator T(EntityWrapper<T> wrapper) => wrapper.Entity;
    }
}

