#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
#endregion

namespace EatMyDust.GameObjects
{
    public class PlayerCar : GameObject
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
            set { speed = value; }
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
        private TimeSpan bfTimer;
        Random rnd = new Random();
        private const float turboMultiplier = 1.5f;
        protected bool started = false;

        public bool boosting;
        private TimeSpan boostTimer;

        private int playerIndex;


        SoundEffect engineFX; // BG Music
        SoundEffectInstance engineInstance;
        SoundEffect honkFX; // BG Music
        SoundEffectInstance honkInstance;
        SoundEffect backFireFX; // BG Music
        SoundEffectInstance backFireInstance;
        SoundEffect startupFX; // BG Music
        SoundEffectInstance startupInstance;
        SoundEffect jesusHornFX;
        SoundEffectInstance jesusHornInstance;
        SoundEffect carEngineRevFX;
        SoundEffectInstance carEngineRevInstance;

        protected int bfRngMax = 8;
        protected int bfRngMin = 2;
        #endregion

        #region Initialization

        public PlayerCar(GameplayScreen gameplayScreen, Vector2 position, int index)
            : base(gameplayScreen)
        {
            this.position = position;
            this.playerIndex = index;
            camera = new Camera(gameplayScreen.ScreenManager.GraphicsDevice.Viewport, Position3);
        }

        protected override void LoadContent()
        {
            if (playerIndex == 1)
                texture = Game.Content.Load<Texture2D>("Sprites/Cars/car1");
            if (playerIndex == 2)
                texture = Game.Content.Load<Texture2D>("Sprites/Cars/car2");
            base.LoadContent();
            tempX = (int)(Position3.X);
            tempY = (int)(Position3.Y);
            boundingRect = new Rectangle((int)Position2.X, (int)Position2.Y, texture.Width, texture.Height);
        }

        public override void Initialize()
        {
            fuelTimer = TimeSpan.FromSeconds(1);
            fuel = 10;
            tempX = (int)(Position3.X);
            tempY = (int)(Position3.Y);
            scaleFactor = 3.0f;

            boosting = false;
            boostTimer = TimeSpan.FromSeconds(3);
            base.Initialize();

            bfTimer = TimeSpan.FromSeconds(bfRngMin + rnd.Next(bfRngMax - bfRngMin));

            engineFX = Game.Content.Load<SoundEffect>("Sounds/engine");
            engineInstance = engineFX.CreateInstance();

            honkFX = Game.Content.Load<SoundEffect>("Sounds/honk");
            honkInstance = honkFX.CreateInstance();

            backFireFX = Game.Content.Load<SoundEffect>("Sounds/backFire");
            backFireInstance = backFireFX.CreateInstance();

            startupFX = Game.Content.Load<SoundEffect>("Sounds/startup");
            startupInstance = startupFX.CreateInstance();

            jesusHornFX = Game.Content.Load<SoundEffect>("Sounds/jesusHorn");
            jesusHornInstance = jesusHornFX.CreateInstance();

            carEngineRevFX = Game.Content.Load<SoundEffect>("Sounds/carEngineRev");
            carEngineRevInstance = carEngineRevFX.CreateInstance();
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

            if (inputManager.IsPressed(Keys.E, Buttons.RightShoulder, playerIndex) || inputManager.IsPressed(Keys.RightShift, Buttons.RightShoulder, playerIndex))
                SoundManager.playSound(honkInstance, 0.6f);
#endif
            Boolean isMoving = false;
            // Keyboard/Dpad velocity change
            if (inputManager.IsHeld(keys[0], Buttons.DPadLeft, playerIndex))
            {
                velocity.X--;
                isMoving = true;
            }
            if (inputManager.IsHeld(keys[1], Buttons.DPadRight, playerIndex))
            {
                velocity.X++;
                isMoving = true;
            }
            if (inputManager.IsHeld(keys[2], Buttons.DPadUp, playerIndex))
            {
                velocity.Y--;
                isMoving = true;
            }
            if (inputManager.IsHeld(keys[3], Buttons.DPadDown, playerIndex))
            {
                velocity.Y++;
                isMoving = true;
            }

            if (isMoving && fuel > 0 && !started)
            {
                SoundManager.playSound(startupInstance, 0.6f);
                gameplayScreen.ScrollSpeed = 10f;
                started = true;
            }
            if (isMoving && fuel > 0) SoundManager.playSound(engineInstance, 0.6f);
            else SoundManager.stopSound(engineInstance);

            //if (startupInstance.State == SoundState.Playing) velocity = new Vector2(0,0);

            // Thumbstick controls
            Vector2 thumbstick = inputManager.currentGamePadStates[playerIndex].ThumbSticks.Left;

            velocity.X += thumbstick.X;
            velocity.Y -= thumbstick.Y;

            // Limits to one unit of movement, i.e. gets the direction of movement
            if (velocity.Length() > 1)
            {
                velocity.Normalize();
            }

            if (inputManager.IsHeld(keys[4], Buttons.A, playerIndex))
                velocity = Vector2.Multiply(velocity, turboMultiplier);
            
        }

        public override void Update(GameTime gameTime)
        {
            camera.Update(new Vector3(tempX, tempY, Position3.Z));

            previousPosition = position;

            if (fuel <= 0)
            {
                SoundManager.playSound(jesusHornInstance, 0.1f);
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

            if (boosting)
            {
                SoundManager.playSound(carEngineRevInstance, 0.6f);
                boostTimer -= gameTime.ElapsedGameTime;
                if (boostTimer <= TimeSpan.FromSeconds(0))
                {
                    boosting = false;
                    boostTimer = TimeSpan.FromSeconds(3);
                }
                else
                    velocity *= turboMultiplier;
            }

            if (bfTimer <= TimeSpan.FromSeconds(0))
            {
                if (engineInstance.State == SoundState.Playing) SoundManager.playSound(backFireInstance, 0.1f);
                bfTimer = TimeSpan.FromSeconds(bfRngMin + rnd.Next(bfRngMax - bfRngMin));
            }
            else
                bfTimer -= gameTime.ElapsedGameTime;

            Vector2 futurePosition = position + Vector2.Multiply(velocity, 8);

            position.X = futurePosition.X;
            position.Y = futurePosition.Y;

            //position += velocity * 8;

            tempX = (int)(Position3.X + camera.View.Translation.X) * -1;
            tempY = (int)(Position3.Y + camera.View.Translation.Y) * -1;
            //int cnt = 0;
            Vector2 cameraEase = new Vector2(tempX, tempY);
            cameraEase.Normalize();
            /*
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
            */
            //bounding rectangle
            
            base.Update(gameTime);
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

        public void CheckCollisionWithEdgeOfScreen()
        {
            //right side
            if (Position2.X > gameplayScreen.ScreenManager.GraphicsDevice.Viewport.Width - texture.Width)
            {
                float difference = Math.Abs(Position2.X - gameplayScreen.ScreenManager.GraphicsDevice.Viewport.Width);
                Position2 = new Vector2(Position2.X - difference, Position2.Y);
                Velocity = new Vector2(Velocity.X * -1, Velocity.Y);
            }
            //left
            if (Position2.X < 0)
            {
                float difference = Math.Abs(Position2.X - 0);
                Position2 = new Vector2(Position2.X + difference, Position2.Y);
                Velocity = new Vector2(Velocity.X * -1, Velocity.Y);
            }
            //bottom
            if (Position2.Y > gameplayScreen.ScreenManager.GraphicsDevice.Viewport.Height - texture.Height)
            {
                float difference = Math.Abs(Position2.Y - gameplayScreen.ScreenManager.GraphicsDevice.Viewport.Height);
                Position2 = new Vector2(Position2.X, Position2.Y - difference);
                Velocity = new Vector2(Velocity.X, Velocity.Y * -1);
            }
            //top
            if (Position2.Y < 200)
            {
                float difference = Math.Abs(Position2.Y - 200);
                Position2 = new Vector2(Position2.X, Position2.Y + difference);
                Velocity = new Vector2(Velocity.X, Velocity.Y * -1);
            }
        }
    }
}
