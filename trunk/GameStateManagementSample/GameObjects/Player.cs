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
        #region Fields
        public Camera camera;

        protected Vector2 position;


        protected int X_LIMIT = 100;
        protected int Y_LIMIT = 100;

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

        protected Vector2 velocity;
        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        #endregion

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
            int x = (int)(Position3.X + camera.View.Translation.X) * -1;
            int y = (int)(Position3.Y + camera.View.Translation.Y) * -1;

            if (Math.Abs(x) >= X_LIMIT || Math.Abs(y) >= Y_LIMIT)
            {

                camera.Update(new Vector3(Position3.X + MathHelper.Clamp(x, -X_LIMIT, X_LIMIT), Position3.Y + MathHelper.Clamp(y, -Y_LIMIT, Y_LIMIT), Position3.Z));
                //camera.Update(Position3);
            }
            base.Update(gameTime);
        }

    }
}
