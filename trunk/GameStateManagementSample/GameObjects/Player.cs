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
    class Player : Sprite
    {
        #region Fields
        public Camera camera;

        protected const int X_LIMIT = 100;
        protected const int Y_LIMIT = 100;
        protected const int CAMERA_PAN_SPEED = 5;

        protected Vector2 velocity;
        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        #endregion

        #region Initialization

        public Player(ScreenManager screenManager)
            :base(screenManager)
        {
            position = Vector2.Zero;
            camera = new Camera(screenManager.GraphicsDevice.Viewport, Position3);
        }

        public Player(ScreenManager screenManager, float x, float y)
            :base(screenManager)
        {
            position = new Vector2(x, y);
            camera = new Camera(screenManager.GraphicsDevice.Viewport, Position3);
        }

        public Player(ScreenManager screenManager, Vector2 position)
            :base(screenManager)
        {
            this.position = position;
            camera = new Camera(screenManager.GraphicsDevice.Viewport, Position3);
        }

        protected override void LoadContent()
        {
            texture = Game.Content.Load<Texture2D>("Sprites/car");
            base.LoadContent();
        }

        #endregion

        public override void Update(GameTime gameTime)
        {
            int tempX = (int)(Position3.X + camera.View.Translation.X) * -1;
            int tempY = (int)(Position3.Y + camera.View.Translation.Y) * -1;
            //int cnt = 0;
            Vector2 cameraEase = new Vector2(tempX, tempY);
            cameraEase.Normalize();

            if (Math.Abs(tempX) > X_LIMIT || Math.Abs(tempY) > Y_LIMIT)
            {
                camera.Update(new Vector3(Position3.X + MathHelper.Clamp(tempX, -X_LIMIT, X_LIMIT), Position3.Y + MathHelper.Clamp(tempY, -Y_LIMIT, Y_LIMIT), Position3.Z));
                //cnt = X_LIMIT;
            }
            else if (cameraEase.X >= -1 && cameraEase.Y >= -1)
            {

              

                if (Math.Abs(MathHelper.Clamp(tempX, -X_LIMIT, X_LIMIT) - cameraEase.X * CAMERA_PAN_SPEED) <= CAMERA_PAN_SPEED)
                    tempX = (int)(MathHelper.Clamp(tempX, -X_LIMIT, X_LIMIT));
                else
                    tempX = (int)(MathHelper.Clamp(tempX, -X_LIMIT, X_LIMIT) - cameraEase.X * CAMERA_PAN_SPEED);
                if (Math.Abs(MathHelper.Clamp(tempY, -Y_LIMIT, Y_LIMIT) - cameraEase.Y * CAMERA_PAN_SPEED) <= CAMERA_PAN_SPEED)
                    tempY = (int)(MathHelper.Clamp(tempY, -Y_LIMIT, Y_LIMIT));
                else
                    tempY = (int)(MathHelper.Clamp(tempY, -Y_LIMIT, Y_LIMIT) - cameraEase.Y * CAMERA_PAN_SPEED);
 
               camera.Update(new Vector3(Position3.X + tempX, Position3.Y + tempY, Position3.Z));
            
            
            
            
            }
                //camera.Update(new Vector3(Position3.X + 10 * cameraEase.X, Position3.Y + 10 * cameraEase.Y, Position3.Z));
            
            base.Update(gameTime);
        }
    }
}
