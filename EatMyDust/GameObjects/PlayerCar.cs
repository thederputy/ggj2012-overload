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
    class PlayerCar : GameObject
    {
        #region Fields
        public Camera camera;

        protected int tempX = 0;
        protected int tempY = 0;


        protected const int X_LIMIT = 150;//150
        protected const int Y_LIMIT = 150;//150
        protected const float MAX_SPEED = 10;
        protected const float MAX_ACCELERATION = 5;
        protected int CAMERA_PAN_SPEED = 5;//5
        protected const int VERT_OFFSET = 175;//175
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


        public int fuel;
        private const int maxFuel = 10;
        private const int fuelPerSecond = 1;
        private const int turboFuelPerSecond = 2;
        private TimeSpan fuelTimer;

        private const float turboMultiplier = 1.5f;

        bool spinningOut;
        TimeSpan spinDuration;

        #endregion

        #region Initialization

        public PlayerCar(ScreenManager screenManager, World world, Vector2 position)
            :base(screenManager, world)
        {
            CreateBody(position);
            camera = new Camera(screenManager.GraphicsDevice.Viewport, Position3);
        }

        protected override void LoadContent()
        {
            texture = Game.Content.Load<Texture2D>("Sprites/cars/car1");
            base.LoadContent();
            tempX = (int)(Position3.X);
            tempY = (int)(Position3.Y);
        }

        public override void Initialize()
        {
            fuelTimer = TimeSpan.FromSeconds(1);
            fuel = 10;
            tempX = (int)(Position3.X);
            tempY = (int)(Position3.Y);
            scaleFactor = 3.0f;
            spinningOut = false;
            spinDuration = TimeSpan.FromSeconds(1);
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
            Keys[] keys = new Keys[5];
            switch (inputKeys)
            {
                case InputKeys.WASD:
                    keys[0] = Keys.A;
                    keys[1] = Keys.D;
                    keys[2] = Keys.W;
                    keys[3] = Keys.S;
                    keys[4] = Keys.LeftShift;
                    break;
                case InputKeys.Arrows:
                    keys[0] = Keys.Left;
                    keys[1] = Keys.Right;
                    keys[2] = Keys.Up;
                    keys[3] = Keys.Down;
                    keys[4] = Keys.RightControl;
                    break;
            }

#if DEBUG 
            if (inputManager.IsPressed(Keys.F, Buttons.LeftShoulder, playerIndex))
                fuel = maxFuel;
            if (inputManager.IsPressed(Keys.B, Buttons.LeftShoulder, playerIndex))
                SpinOut();
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

            // Limits to one unit of movement, i.e. gets the direction of movement
            if (velocity.Length() > 1)
                velocity.Normalize();

            if (inputManager.IsHeld(keys[4], Buttons.A, playerIndex))
                velocity = Vector2.Multiply(velocity, turboMultiplier);
            
        }

        public override void Update(GameTime gameTime)
        {
            camera.Update(new Vector3(tempX, tempY, Position3.Z));

            if (fuel <= 0)
            {
                velocity = Vector2.Zero;
            }
            if (fuelTimer <= TimeSpan.FromSeconds(0))
            {
                if (velocity.Length() > 1.0f)
                    fuel -= turboFuelPerSecond;
                else
                    fuel -= fuelPerSecond;
                fuelTimer = TimeSpan.FromSeconds(1);

            }
            else
                fuelTimer -= gameTime.ElapsedGameTime;
            
            body.ApplyLinearImpulse(velocity * 100, body.GetPosition());

            if (spinningOut)
            {
                spinDuration -= gameTime.ElapsedGameTime;
                if (spinDuration <= TimeSpan.FromSeconds(0))
                {
                    body.SetAngularVelocity(0);
                    spinningOut = false;
                }
            }

            tempX = (int)(Position3.X + camera.View.Translation.X) * -1;
            tempY = (int)(Position3.Y + camera.View.Translation.Y) * -1;
            //int cnt = 0;
            Vector2 cameraEase = new Vector2(tempX, tempY);
            cameraEase.Normalize();

            if (Math.Abs(tempX) > X_LIMIT || Math.Abs(tempY) > Y_LIMIT)
            {
                tempX = (int)(Position3.X + MathHelper.Clamp(tempX, -X_LIMIT, X_LIMIT));
                tempY = (int)(Position3.Y + MathHelper.Clamp(tempY, -Y_LIMIT, Y_LIMIT));

            }
            else if (cameraEase.X >= -1 && cameraEase.Y >= -1)
            {
                if (Math.Abs(MathHelper.Clamp(tempX, -X_LIMIT, X_LIMIT) - cameraEase.X * CAMERA_PAN_SPEED) < CAMERA_PAN_SPEED)
                {
                    tempX = (int)(Position3.X + MathHelper.Clamp(tempX, -X_LIMIT, X_LIMIT));
                }
                else
                {
                    tempX = (int)(Position3.X + MathHelper.Clamp(tempX, -X_LIMIT, X_LIMIT) - cameraEase.X * CAMERA_PAN_SPEED); 
                }
                if (Math.Abs(MathHelper.Clamp(tempY, -Y_LIMIT, Y_LIMIT) - cameraEase.Y * CAMERA_PAN_SPEED) < CAMERA_PAN_SPEED)
                {
                    tempY = (int)(Position3.Y + MathHelper.Clamp(tempY, -Y_LIMIT, Y_LIMIT));                  
                }
                else
                {                  
                    tempY = (int)(Position3.Y + MathHelper.Clamp(tempY, -Y_LIMIT, Y_LIMIT) - cameraEase.Y * CAMERA_PAN_SPEED);                
                }

            }
            else
            {
                tempX = (int)(Position3.X);
                tempY = (int)(Position3.Y);
            }

            camera.Update(new Vector3(tempX + texture.Width / 2, tempY + texture.Height / 2 - VERT_OFFSET, Position3.Z));
            //camera.Update(new Vector3(tempX, tempY, Position3.Z));
            
            base.Update(gameTime);
        }

        public override void CreateBody(Vector2 position)
        {
            BodyDef def = new BodyDef();
            def.userData = this;
            def.position = position;
            def.type = BodyType.Dynamic;
            def.linearDamping = 0.75f;
            body = physicsWorld.CreateBody(def);
            PolygonShape shape = new PolygonShape();
            shape.SetAsBox(1, 1);
            body.CreateFixture(shape, 1.0f);
        }

        public float getFuelPercent()
        {
            int temp = 1;

            if (fuelTimer.Milliseconds == 0) temp = 0;
 
            return (float)(fuel - temp) / (float)maxFuel + ((float)(fuelTimer.Milliseconds) / 1000 / (float)(maxFuel));
        }

        public void AddFuel()
        {
            if (fuel < maxFuel)
                fuel += fuelPerSecond;
        }

        public void SpinOut()
        {
            if (!spinningOut)
            {
                spinningOut = true;
                //body.SetTransform(body.Position, (float)(body.Rotation + Math.PI / 16));
                //body.SetAngularVelocity((float)(Math.PI * 2 + Math.PI/32));
                body.SetAngularVelocity((float)(Math.PI/16));
                spinDuration = TimeSpan.FromSeconds(1);
            }
        }

    }
}
