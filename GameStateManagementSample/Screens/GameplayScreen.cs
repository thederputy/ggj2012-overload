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
using GameStateManagement.GameObjects;
using Box2D.XNA;
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

        Player playerOne;
        Player playerTwo;

        Random random = new Random();
        InputManager inputManager;

        float pauseAlpha;

        // For multiple viewports
        Viewport defaultViewport;
        Viewport leftViewport;
        Viewport rightViewport;
        Matrix projectionMatrix;
        Matrix halfprojectionMatrix;
        float halfAspectRatio;

        //Textures
        Texture2D blank;
        Texture2D carTexture;
        Texture2D road;

        //Physics World
        World physicsWorld;

        //BasicEffect effect;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            physicsWorld = new World(Vector2.Zero, false);
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

            
            //projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
            //    MathHelper.PiOver4, 4.0f / 3.0f, 1.0f, 10000f);
            //halfprojectionMatrix = Matrix.CreatePerspectiveFieldOfView(
            //    MathHelper.PiOver4, 2.0f / 3.0f, 1.0f, 10000f);
            halfAspectRatio = (ScreenManager.GraphicsDevice.Viewport.Width / 2) / ScreenManager.GraphicsDevice.Viewport.Height;
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4, ScreenManager.GraphicsDevice.Viewport.AspectRatio, 1.0f, 10000f);
            
            halfprojectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4, halfAspectRatio, 1.0f, 10000f);
            halfprojectionMatrix.M11 = halfprojectionMatrix.M22;

            //effect = new BasicEffect(ScreenManager.GraphicsDevice);
            //effect.EnableDefaultLighting(); //required?

            // Input
            inputManager = new InputManager(ScreenManager.Game);

            // Players
            playerOne = new Player(ScreenManager, 288, 344);
            playerTwo = new Player(ScreenManager, 352, 344);

            // Textures
            blank = this.content.Load<Texture2D>("blank");
            carTexture = this.content.Load<Texture2D>("Sprites/car");
            road = this.content.Load<Texture2D>("Backgrounds/roads1/preview");

            // TJH Powersource
            PowerSource powerSource = new PowerSource(ScreenManager);
            

            // A real game would probably have more content than this sample, so
            // it would take longer to load. We simulate that by delaying for a
            // while, giving you a chance to admire the beautiful loading screen.
            Thread.Sleep(100);

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();

            // Add the game components to the game
            // This allows each component's Initialize, Update, Draw to get called automatically.
            ScreenManager.Game.Components.Add(playerOne);
            ScreenManager.Game.Components.Add(playerTwo);
            ScreenManager.Game.Components.Add(inputManager);
            ScreenManager.Game.Components.Add(powerSource);
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

            physicsWorld.Step((float)gameTime.ElapsedGameTime.TotalSeconds, 10, 10);

            
            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {
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
                Vector2 movementOne = Vector2.Zero;
                Vector2 movementTwo = Vector2.Zero;

                // Player One
                if (keyboardState.IsKeyDown(Keys.A))
                    movementOne.X--;
                if (keyboardState.IsKeyDown(Keys.D))
                    movementOne.X++;
                if (keyboardState.IsKeyDown(Keys.W))
                    movementOne.Y--;
                if (keyboardState.IsKeyDown(Keys.S))
                    movementOne.Y++;

                // Player Two
                if (keyboardState.IsKeyDown(Keys.Left))
                    movementTwo.X--;
                if (keyboardState.IsKeyDown(Keys.Right))
                    movementTwo.X++;
                if (keyboardState.IsKeyDown(Keys.Up))
                    movementTwo.Y--;
                if (keyboardState.IsKeyDown(Keys.Down))
                    movementTwo.Y++;

                Vector2 thumbstick = gamePadState.ThumbSticks.Left;

                movementOne.X += thumbstick.X;
                movementOne.Y -= thumbstick.Y;

                if (movementOne.Length() > 1)
                    movementOne.Normalize();

                if (movementTwo.Length() > 1)
                    movementTwo.Normalize();

                playerOne.Position2 += movementOne * 8;
                playerTwo.Position2 += movementTwo * 8;
            }
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            ScreenManager.GraphicsDevice.Viewport = defaultViewport;
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.CornflowerBlue, 0, 0);

            /* TJH Debugger
            SpriteBatch debugSB = ScreenManager.SpriteBatch;
            debugSB.Begin();
            debugSB.DrawString(gameFont, "P1 Position: " + playerOnePosition.ToString(), new Vector2(10, 10), Color.White);
            debugSB.End();
            */

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            //DRAW LEFT PLAYER STUFF HERE
            // Our player and enemy are both actually just text strings.

            ScreenManager.GraphicsDevice.Viewport = leftViewport;
            //DrawScene(gameTime, playerOne.camera);
    
            DrawGameScreen(spriteBatch, gameTime, playerOne); 
            //END LEFT PLAYER STUFF

            //DRAW RIGHT PLAYER STUFF HERE
            // Our player and enemy are both actually just text strings.

            ScreenManager.GraphicsDevice.Viewport = rightViewport;
            //DrawScene(gameTime, playerTwo.camera);

            DrawGameScreen(spriteBatch, gameTime, playerTwo);

            //END RIGHT PLAYER STUFF

            //DRAW FULL SCREEN AGAIN
            // If the game is transitioning on or off, fade it out to black.
            ScreenManager.GraphicsDevice.Viewport = defaultViewport;

            // Drawing HUD stuff for now
            spriteBatch.Begin();
            spriteBatch.Draw(blank, new Rectangle(ScreenManager.GraphicsDevice.Viewport.Width / 2 - 1, 0, 3, ScreenManager.GraphicsDevice.Viewport.Height), Color.Black);
            spriteBatch.End();

            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);
                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }

        protected void DrawGameScreen(SpriteBatch spriteBatch, GameTime gameTime, Player player)
        {
            Matrix trans = Matrix.Identity;
            trans.M41 = ScreenManager.GraphicsDevice.Viewport.Width / 2;
            trans.M42 = ScreenManager.GraphicsDevice.Viewport.Height / 2;

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, RasterizerState.CullNone, null, trans * player.camera.View * halfprojectionMatrix);
            

            //TODO replace with road drawablegamecomponent draw call
            spriteBatch.Draw(road, new Rectangle(100, -200, 1024, 768), Color.White);
            
            playerOne.Draw(spriteBatch, Color.Green);
            playerTwo.Draw(spriteBatch, Color.Red);

            spriteBatch.End();
        }


        //protected void DrawScene(GameTime gameTime, Camera camera)
        //{
        //    effect.EnableDefaultLighting();
        //    effect.World = Matrix.Identity;
        //    effect.View = camera.View;
        //    effect.Projection = camera.Projection;
        //        //Draw the mesh, will use the effects set above.
        //}

        #endregion
    }
}
