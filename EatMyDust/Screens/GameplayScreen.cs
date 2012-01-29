#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using EatMyDust.GameObjects;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
#endregion

namespace EatMyDust
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    public class GameplayScreen : GameScreen
    {
        #region Fields

        ContentManager content;
        SpriteFont gameFont;
        
        PlayerCar playerOne;
        public PlayerCar PlayerOne
        {
            get { return playerOne; }
        }
        
        PlayerCar playerTwo;
        public PlayerCar PlayerTwo
        {
            get { return playerTwo; }
        }

        Track track;

        public float ScrollSpeed
        {
            get { return track.Speed;  }
            set { track.Speed = value; }
        }
        
        Random random = new Random();
        InputManager inputManager;

        float pauseAlpha;

        //Textures
        Texture2D blank;
        Texture2D road;

        BasicEffect effect;

        List<PowerSource> powerSources;
        TimeSpan dropTimer;
        const int dropInterval = 800; //milliseconds

        List<PowerUp> powerUps;
        TimeSpan powerupTimer;
        const int powerupDropInterval = 5000;

        Random rand;

        int FUEL_BAR_Y = 10; 
        int FUEL_BAR_HEIGHT = 30;
        const int FUEL_BAR_WIDTH = 30;
        
        // sounds effects and music         
        SoundEffect collectPower; // BG Music
        SoundEffectInstance collectPowerInstance;
        SoundEffect collideFX; // BG Music
        SoundEffectInstance collideInstance;
        SoundEffect engineFX; // BG Music
        SoundEffectInstance engineInstance;
        SoundEffect doubleHonkFX;
        SoundEffectInstance doubleHonkInstance;

        #endregion

        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            rand = new Random();
        }

        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            gameFont = content.Load<SpriteFont>("gamefont");

            // Input
            inputManager = new InputManager(ScreenManager.Game);

            // Players
            playerOne = new PlayerCar(this, new Vector2(ScreenManager.GraphicsDevice.Viewport.Width/2, ScreenManager.GraphicsDevice.Viewport.Height/2), 1);
            //playerOne.Position2 -= new Vector2(playerOne.texture.Width * 2, 0);
            playerOne.Position2 += new Vector2(-128, -32);
            playerTwo = new PlayerCar(this, new Vector2(ScreenManager.GraphicsDevice.Viewport.Width/2, ScreenManager.GraphicsDevice.Viewport.Height/2), 2);
            //playerTwo.Position2 += new Vector2(playerTwo.texture.Width, 0);
            playerTwo.Position2 += new Vector2(64, -32);

            // Textures
            blank = this.content.Load<Texture2D>("blank");
            road = this.content.Load<Texture2D>("Backgrounds/roads1/preview");

            track = new Track(this);

            // A real game would probably have more content than this sample, so
            // it would take longer to load. We simulate that by delaying for a
            // while, giving you a chance to admire the beautiful loading screen.
            Thread.Sleep(100);

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();

            dropTimer = TimeSpan.FromMilliseconds(dropInterval);
            powerSources = new List<PowerSource>();

            powerUps = new List<PowerUp>();
            /*PowerUp pup = new PowerUp(this, PowerUp.Type.PositionSwap);
            pup.Position2 = Vector2.Zero;
            powerUps.Add(pup);

            pup = new PowerUp(this, PowerUp.Type.SpeedBoost);
            pup.Position2 = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 4, ScreenManager.GraphicsDevice.Viewport.Height / 4);
            powerUps.Add(pup);
            */
            powerupTimer = TimeSpan.FromSeconds(10);
            // Add the game components to the game
            // This allows each component's Initialize, Update, Draw to get called automatically.
            ScreenManager.Game.Components.Add(inputManager);

            //set fuel bar to use the whole screen
            FUEL_BAR_Y = ScreenManager.GraphicsDevice.Viewport.Height / 2;
            FUEL_BAR_HEIGHT = ScreenManager.GraphicsDevice.Viewport.Height / 2;

            //MUSIC AND SOUND LOADING
            
            collectPower = this.content.Load<SoundEffect>("Sounds/collect");
            collectPowerInstance = collectPower.CreateInstance();

            collideFX = this.content.Load<SoundEffect>("Sounds/hit");
            collideInstance = collideFX.CreateInstance();

            doubleHonkFX = this.content.Load<SoundEffect>("Sounds/doubleHonk");
            doubleHonkInstance = doubleHonkFX.CreateInstance();
        }

        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
        }

        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {
                CheckForCollisions();

                if (playerOne.fuel <= 0 && playerTwo.fuel <= 0)
                    ScrollSpeed = 0f;
                else if (playerOne.boosting || playerTwo.boosting)
                    ScrollSpeed = 20f;
                else if (playerOne.Velocity.LengthSquared() > 0f || playerTwo.Velocity.LengthSquared() > 0f)
                    ScrollSpeed = 10f;



                dropTimer -= gameTime.ElapsedGameTime;
                if (dropTimer <= TimeSpan.FromSeconds(0))
                {
                    dropTimer = TimeSpan.FromMilliseconds(dropInterval);
                    PowerSource ps = new PowerSource(this, playerOne.Position2 + new Vector2(0.5f*playerOne.texture.Width, playerOne.texture.Height), playerOne, Color.Red);
                    ps.Position2 += new Vector2(-0.5f * ps.texture.Width, -0.5f * ps.texture.Height);
                    powerSources.Add(ps);
                    ps = new PowerSource(this, playerTwo.Position2 + new Vector2(0.5f * playerTwo.texture.Width, playerTwo.texture.Height), playerTwo, Color.Blue);
                    ps.Position2 += new Vector2(-0.5f * ps.texture.Width, -0.5f * ps.texture.Height);
                    powerSources.Add(ps);
                }
                for (int i = 0; i < powerSources.Count; i++)
                {
                    if (powerSources[i].expired)
                    {
                        ScreenManager.Game.Components.Remove(powerSources[i]);
                        powerSources.Remove(powerSources[i]);
                        i--;
                    }
                }
                for (int i = 0; i < powerUps.Count; i++)
                {
                    if (powerUps[i].expired)
                    {
                        ScreenManager.Game.Components.Remove(powerUps[i]);
                        powerUps.Remove(powerUps[i]);
                        i--;
                    }
                }

                powerupTimer -= gameTime.ElapsedGameTime;
                if (powerupTimer <= TimeSpan.FromSeconds(0))
                {
                    int i = rand.Next(2);
                    PowerUp pup = new PowerUp(this, (PowerUp.PowerUpType)i);
                    pup.Position2 = new Vector2(rand.Next(0, 1280), rand.Next(0, 720));
                    powerUps.Add(pup);
                    powerupTimer = TimeSpan.FromSeconds(8);
                }
            }
        }

        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[playerIndex];

            if (input.IsPauseGame(ControllingPlayer) || gamePadDisconnected)
            {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
            else
            {
                // TODO: make each player handle its own input.
                playerOne.HandleInput(inputManager, InputKeys.WASD, 0);
                playerTwo.HandleInput(inputManager, InputKeys.Arrows, 1);
            }
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.CornflowerBlue, 0, 0);

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            // Drawing HUD stuff for now
            track.Draw(gameTime);
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, DepthStencilState.Default, null);

            DrawGameScreen(spriteBatch, gameTime);

            //FUEL_BAR_Y
            //FUEL_BAR_HEIGHT

            spriteBatch.End();


            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);
                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }

        protected void DrawGameScreen(SpriteBatch spriteBatch, GameTime gameTime)
        {
            //TODO: replace with road drawablegamecomponent draw call
            //spriteBatch.Draw(road, new Rectangle(0, 0, 1024, 768), Color.White);
            
            playerOne.Draw(spriteBatch, Color.Blue);
            playerTwo.Draw(spriteBatch, Color.Red);
            

            //Debug
            //spriteBatch.DrawString(gameFont, String.Format("Pos: {0}", playerOne.Position2.ToString()), playerOne.Position2, Color.White);
            //spriteBatch.DrawString(gameFont, String.Format("Pos: {0}", playerTwo.Position2.ToString()), playerTwo.Position2, Color.White);
            

            //draw game items:
            foreach(PowerSource ps in powerSources)
            {
                ps.Draw(spriteBatch, ps.color * ps.fade);
            }

            foreach (PowerUp pup in powerUps)
            {
                pup.Draw(spriteBatch);
            }

            spriteBatch.Draw(blank, new Rectangle(0, ScreenManager.GraphicsDevice.Viewport.Height - (int)(FUEL_BAR_Y * playerOne.getFuelPercent()), FUEL_BAR_WIDTH, FUEL_BAR_HEIGHT), Color.Goldenrod);
            spriteBatch.Draw(blank, new Rectangle(ScreenManager.GraphicsDevice.Viewport.Width - FUEL_BAR_WIDTH, ScreenManager.GraphicsDevice.Viewport.Height - (int)(FUEL_BAR_Y * playerTwo.getFuelPercent()), FUEL_BAR_WIDTH, FUEL_BAR_HEIGHT), Color.Goldenrod);
        }

        #endregion

        private void FuelUp(PlayerCar p, PowerSource ps)
        {
            if (ps.createdBy != p)
            {
                p.AddFuel();
                //SoundManager.playSound(collectPowerInstance, 0.1f);
                ps.expired = true;
            }
        }

        private void CheckForCollisions()
        {
            //check for edge of screen detection
            playerOne.CheckCollisionWithEdgeOfScreen();
            playerTwo.CheckCollisionWithEdgeOfScreen();

            if (playerOne.boundingRect.Intersects(playerTwo.boundingRect))
            {
                //player collision!
                SoundManager.playSound(collideInstance, 0.1f);
                SoundManager.playSound(doubleHonkInstance, 0.1f);
                Vector2 difference = playerTwo.Position2 - playerOne.Position2;
                playerTwo.Position2 += difference / 2;
                playerOne.Position2 -= difference / 2;
                playerOne.Velocity *= -1;
                playerTwo.Velocity *= -1;
            }

            //check for collisions between the players and powersources
            foreach (PowerSource p in powerSources)
            {
                if (p.createdBy != playerOne && playerOne.boundingRect.Intersects(p.boundingRect))
                {
                    FuelUp(playerOne, p);
                }
                if (p.createdBy != playerTwo && playerTwo.boundingRect.Intersects(p.boundingRect))
                {
                    FuelUp(playerTwo, p);
                }
            }
            foreach (PowerUp pup in powerUps)
            {
                // Either player powerups
                if (playerOne.boundingRect.Intersects(pup.boundingRect) || playerTwo.boundingRect.Intersects(pup.boundingRect))
                {
                    if (pup.currentType == PowerUp.PowerUpType.PositionSwap)
                    {
                        Vector2 tmp = playerOne.Position2;
                        playerOne.Position2 = playerTwo.Position2;
                        playerTwo.Position2 = tmp;
                        pup.expired = true;
                    }
                    
                    // Single player powerups
                    if (playerOne.boundingRect.Intersects(pup.boundingRect))
                    {
                        if (pup.currentType == PowerUp.PowerUpType.SpeedBoost)
                        {
                            playerOne.boosting = true;
                            pup.expired = true;
                        }
                    }
                    if (playerTwo.boundingRect.Intersects(pup.boundingRect))
                    {
                        if (pup.currentType == PowerUp.PowerUpType.SpeedBoost)
                        {
                            playerTwo.boosting = true;
                            pup.expired = true;
                        }
                    }
                }
            }

            //TODO: check for collisions between the players and powerups

        }
    }
}
