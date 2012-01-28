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
#endregion

namespace GameStateManagement
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    class GameplayScreen : GameScreen
    {
        #region Fields

        ContentManager content;
        SpriteFont gameFont;

        Vector3 playerOnePosition = new Vector3(-10, 30, 0);
        Vector3 playerTwoPosition = new Vector3(-20, 0, 0);

        Random random = new Random();

        float pauseAlpha;

        // For multiple viewports
        Viewport defaultViewport;
        Viewport leftViewport;
        Viewport rightViewport;
        Matrix projectionMatrix;
        Matrix halfprojectionMatrix;

        BasicEffect effect;

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

            // For multiple viewports
            defaultViewport = ScreenManager.GraphicsDevice.Viewport; // altered from example code to use the Screen Manager's Graphics device
            leftViewport = defaultViewport;
            rightViewport = defaultViewport;
            leftViewport.Width = leftViewport.Width / 2;
            rightViewport.Width = rightViewport.Width / 2;
            rightViewport.X = leftViewport.Width;
            //            rightViewport.X = leftViewport.Width + 1;

            
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4, 4.0f / 3.0f, 1.0f, 10000f);
            halfprojectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4, 2.0f / 3.0f, 1.0f, 10000f);

            effect = new BasicEffect(ScreenManager.GraphicsDevice);
            //effect.EnableDefaultLighting(); //required?

            // A real game would probably have more content than this sample, so
            // it would take longer to load. We simulate that by delaying for a
            // while, giving you a chance to admire the beautiful loading screen.
            Thread.Sleep(1000);

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();
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

            // Do all drawing in here
            if (IsActive)
            {
                // Apply a stabilizing force to stop the players moving off screen.
                // TODO: this game isn't very fun! You could probably improve
                // it by inserting something more interesting in this space :-)
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
                // Otherwise move the player position.
                Vector3 movementOne = Vector3.Zero;
                Vector3 movementTwo = Vector3.Zero;

                // Player One
                if (keyboardState.IsKeyDown(Keys.Left))
                    movementOne.X--;
                if (keyboardState.IsKeyDown(Keys.Right))
                    movementOne.X++;
                if (keyboardState.IsKeyDown(Keys.Up))
                    movementOne.Y--;
                if (keyboardState.IsKeyDown(Keys.Down))
                    movementOne.Y++;

                // Player Two
                if (keyboardState.IsKeyDown(Keys.A))
                    movementTwo.X--;
                if (keyboardState.IsKeyDown(Keys.D))
                    movementTwo.X++;
                if (keyboardState.IsKeyDown(Keys.W))
                    movementTwo.Y--;
                if (keyboardState.IsKeyDown(Keys.S))
                    movementTwo.Y++;

                Vector2 thumbstick = gamePadState.ThumbSticks.Left;

                movementOne.X += thumbstick.X;
                movementOne.Y -= thumbstick.Y;

                if (movementOne.Length() > 1)
                    movementOne.Normalize();

                if (movementTwo.Length() > 1)
                    movementTwo.Normalize();

                playerOnePosition += movementOne * 2;
                playerTwoPosition += movementTwo * 2;
            }
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            ScreenManager.GraphicsDevice.Viewport = defaultViewport;
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.CornflowerBlue, 0, 0);
            ScreenManager.GraphicsDevice.Viewport = leftViewport;
            DrawScene(gameTime, Matrix.CreateLookAt(playerOnePosition + new Vector3(0, 0, 100), playerOnePosition, Vector3.Up), halfprojectionMatrix);
            
            //DRAW LEFT PLAYER STUFF HERE
            
            // Our player and enemy are both actually just text strings.
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            spriteBatch.DrawString(gameFont, "Player1", new Vector2(playerOnePosition.X, playerOnePosition.Y), Color.Green);
            spriteBatch.DrawString(gameFont, "Player2", new Vector2(playerTwoPosition.X, playerTwoPosition.Y), Color.Red);

            spriteBatch.End();
            
            //END LEFT PLAYER STUFF

            ScreenManager.GraphicsDevice.Viewport = rightViewport;
            DrawScene(gameTime, Matrix.CreateLookAt(playerTwoPosition + new Vector3(0, 0, 100), playerTwoPosition, Vector3.Up), halfprojectionMatrix);

            //DRAW RIGHT PLAYER STUFF HERE

            // Our player and enemy are both actually just text strings.
            
            spriteBatch.Begin();

            spriteBatch.DrawString(gameFont, "Player1", new Vector2(playerOnePosition.X, playerOnePosition.Y), Color.Green);
            spriteBatch.DrawString(gameFont, "Player2", new Vector2(playerTwoPosition.X, playerTwoPosition.Y), Color.Red);

            spriteBatch.End();

            //END RIGHT PLAYER STUFF

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }

            
                
        }

        protected void DrawScene(GameTime gameTime, Matrix view,
            Matrix projection)
        {
            effect.EnableDefaultLighting();
            effect.World = Matrix.Identity;
            effect.View = view;
            effect.Projection = projection;
                //Draw the mesh, will use the effects set above.
        }

        #endregion
    }
}
