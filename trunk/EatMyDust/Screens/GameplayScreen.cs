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

        int FUEL_BAR_Y = 10; 
        int FUEL_BAR_HEIGHT = 30;
        const int FUEL_BAR_WIDTH = 30;

        #endregion

        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
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
            playerOne = new PlayerCar(this, new Vector2(ScreenManager.GraphicsDevice.Viewport.Width/2, ScreenManager.GraphicsDevice.Viewport.Height/2));
            //playerOne.Position2 -= new Vector2(playerOne.texture.Width * 2, 0);
            playerOne.Position2 += new Vector2(-128, -32);
            playerTwo = new PlayerCar(this, new Vector2(ScreenManager.GraphicsDevice.Viewport.Width/2, ScreenManager.GraphicsDevice.Viewport.Height/2));
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

            // Add the game components to the game
            // This allows each component's Initialize, Update, Draw to get called automatically.
            ScreenManager.Game.Components.Add(inputManager);

            //set fuel bar to use the whole screen
            FUEL_BAR_Y = ScreenManager.GraphicsDevice.Viewport.Height / 2;
            FUEL_BAR_HEIGHT = ScreenManager.GraphicsDevice.Viewport.Height / 2;
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

            CheckForCollisions();
            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {
                dropTimer -= gameTime.ElapsedGameTime;
                if (dropTimer <= TimeSpan.FromSeconds(0))
                {
                    dropTimer = TimeSpan.FromMilliseconds(dropInterval);
                    PowerSource ps = new PowerSource(this, playerOne.Position2 + new Vector2(playerOne.texture.Width, playerOne.texture.Height*2), playerOne, Color.Green);
                    powerSources.Add(ps);
                    ps = new PowerSource(this, playerTwo.Position2 + new Vector2(playerTwo.texture.Width, playerTwo.texture.Height*2), playerTwo, Color.Red);
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


            /* TJH Debugger
            SpriteBatch debugSB = ScreenManager.SpriteBatch;
            debugSB.Begin();
            debugSB.DrawString(gameFont, "P1 Position: " + playerOnePosition.ToString(), new Vector2(10, 10), Color.White);
            debugSB.End();
            */

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            // Drawing HUD stuff for now
            spriteBatch.Begin();

            DrawGameScreen(spriteBatch, gameTime);
            //spriteBatch.Draw(blank, new Rectangle(ScreenManager.GraphicsDevice.Viewport.Width / 2 - 2, 0, 5, ScreenManager.GraphicsDevice.Viewport.Height), Color.Black); // Draws Black bar down Center
            //spriteBatch.Draw(blank, new Rectangle(ScreenManager.GraphicsDevice.Viewport.Width / 2, 0, 100, 5), Color.Pink);

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
            spriteBatch.Draw(road, new Rectangle(0, 0, 1024, 768), Color.White);
            
            playerOne.Draw(spriteBatch, Color.Green);
            playerTwo.Draw(spriteBatch, Color.Red);

            //Debug
            //spriteBatch.DrawString(gameFont, String.Format("Pos: {0}", playerOne.Position2.ToString()), playerOne.Position2, Color.White);
            //spriteBatch.DrawString(gameFont, String.Format("Pos: {0}", playerTwo.Position2.ToString()), playerTwo.Position2, Color.White);
            
            foreach(PowerSource ps in powerSources)
            {
                ps.Draw(spriteBatch, ps.color);
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
                ps.expired = true;
            }
        }

        private void CheckForCollisions()
        {
            //check for collisions between the players
            if (playerOne.texture.Bounds.Intersects(playerTwo.texture.Bounds))
            {
                //player collision!
            }

            //check for collisions between the players and powersources
            foreach (PowerSource p in powerSources)
            {
                if (p.createdBy != playerOne && playerOne.texture.Bounds.Intersects(p.texture.Bounds))
                {
                    FuelUp(playerOne, p);
                }
                if (p.createdBy != playerTwo && playerTwo.texture.Bounds.Intersects(p.texture.Bounds))
                {
                    FuelUp(playerTwo, p);
                }
            }

            //TODO: check for collisions between the players and powerups

        }
    }
}
