#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Box2D.XNA;
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
        
        protected float speed;
        public float Speed
        {
            get { return speed; }
            set
            {
                speed = value;
            }
        }

        protected Vector2 velocity;
        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }


        public Body Body
        {
            get { return m_body; }
            set { m_body = value; }
        }
        private Body m_body;


        public int fuel;
        private const int maxFuel = 10;
        private const int fuelPerSecond = 1;
        private TimeSpan fuelTimer;

        #endregion

        #region Initialization

        public Player(ScreenManager screenManager, World world)
            :base(screenManager)
        {
            position = Vector2.Zero;
            camera = new Camera(screenManager.GraphicsDevice.Viewport, Position3);
            CreateBody(world);
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

        public override void Initialize()
        {
            fuelTimer = TimeSpan.FromSeconds(1);
            fuel = 10;
            base.Initialize();
        }

        #endregion

        /// <summary>
        /// Handles keyboard/controller input for the player.
        /// </summary>
        /// <param name="inputManager">Input manager that handles states</param>
        /// <param name="inputKeys">The controls to use for keyboard, WASD or arrows</param>
        /// <param name="playerIndex">player index, 0-3</param>
        public void HandleInput(InputManager inputManager, InputKeys inputKeys, int playerIndex)
        {
            // Otherwise move the player position.
            velocity = Vector2.Zero;

            // Keyboard Controls
            Keys[] keys = new Keys[4];
            switch (inputKeys)
            {
                case InputKeys.WASD:
                    keys[0] = Keys.A;
                    keys[1] = Keys.D;
                    keys[2] = Keys.W;
                    keys[3] = Keys.S;
                    break;
                case InputKeys.Arrows:
                    keys[0] = Keys.Left;
                    keys[1] = Keys.Right;
                    keys[2] = Keys.Up;
                    keys[3] = Keys.Down;
                    break;
            }

#if DEBUG 
            if (inputManager.IsPressed(Keys.F, Buttons.LeftShoulder, playerIndex))
                fuel = maxFuel;
#endif

            // Keyboard/Dpad velocity change
            if (inputManager.IsHeld(keys[0], Buttons.DPadLeft, playerIndex))
                velocity.X--;
            if (inputManager.IsHeld(keys[1], Buttons.DPadRight, playerIndex))
                velocity.X++;
            if (inputManager.IsHeld(keys[2], Buttons.DPadUp, playerIndex))
                velocity.Y--;
            if (inputManager.IsHeld(keys[3], Buttons.DPadDown, playerIndex))
                velocity.Y++;

            // Thumbstick controls
            Vector2 thumbstick = inputManager.currentGamePadStates[playerIndex].ThumbSticks.Left;

            velocity.X += thumbstick.X;
            velocity.Y -= thumbstick.Y;

            if (velocity.Length() > 1)
                velocity.Normalize();

            
        }

        public override void Update(GameTime gameTime)
        {
            if (fuel <= 0)
            {
                velocity = Vector2.Zero;
            }
            if (fuelTimer <= TimeSpan.FromSeconds(0))
            {
                fuel -= fuelPerSecond;
                fuelTimer = TimeSpan.FromSeconds(1);
            }
            else
                fuelTimer -= gameTime.ElapsedGameTime;
            

            position += velocity * 8;
            
            
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
        private void CreateBody(World world)
        {
            BodyDef def = new BodyDef();
            def.userData = this;
            def.position = this.position;
            m_body = world.CreateBody(def);
            PolygonShape shape = new PolygonShape();
            shape.SetAsBox(1, 1);
            m_body.CreateFixture(shape, 1.0f);
        }

    }
}
