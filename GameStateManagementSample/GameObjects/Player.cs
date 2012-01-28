#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#endregion

namespace GameStateManagement.GameObjects
{
    class Player : GameComponent
    {
        protected Vector2 position;

        public Camera camera;

        public Vector2 Position2
        {
            get { return position; }
            set
            {
                position.X = value.X;
                position.Y = value.Y;
            }
        }

        public Vector3 Position3
        {
            get { return new Vector3(position, 0); }
            set
            {
                position.X = value.X;
                position.Y = value.Y;
            }
        }

        public Player(ScreenManager screenManager)
            :base (screenManager.Game)
        {
            position = Vector2.Zero;
            camera = new Camera(screenManager.GraphicsDevice.Viewport, Position3);
        }

        public Player(ScreenManager screenManager, float x, float y)
            :base (screenManager.Game)
        {
            position = new Vector2(x, y);
            camera = new Camera(screenManager.GraphicsDevice.Viewport, Position3);
        }

        public Player(ScreenManager screenManager, Vector2 position)
            :base (screenManager.Game)
        {
            this.position = position;
            camera = new Camera(screenManager.GraphicsDevice.Viewport, Position3);
        }

        public override void Update(GameTime gameTime)
        {
            camera.Update(Position3);
            base.Update(gameTime);
        }




    }
}
