using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DungeonGame.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonGame
{
    public class GameWindow : Game
    {
        internal event EventHandler<GameUpdateEventArgs>? OnUpdate; 
        internal event EventHandler<GameDrawEventArgs>? OnDraw;
        private bool IsInitialized => this.GraphicsDevice != null;

        public GameWindow(GraphicsDeviceManagerFactory factory) :base()
        {
            factory(this);
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
                this.RunOneFrame();
                return this.GraphicsDevice != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void InitializeGame()
        {
            //get the max resolution of the monitor
            var (width, height) = (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
            this.GraphicsDevice.Viewport = new Viewport(0, 0, width, height);
        }
    }
}

