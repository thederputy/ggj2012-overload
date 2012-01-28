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
using System.Collections.Generic;
#endregion

namespace GameStateManagement
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    class GameplayScreen : GameScreen, IContactListener
    {
        #region Fields

        ContentManager content;
        SpriteFont gameFont;
        DebugRenderer debugRenderer;


        PlayerCar playerOne;
        PlayerCar playerTwo;

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
        Texture2D road;

        //Physics World
        World physicsWorld;

        BasicEffect effect;

        List<PowerSource> powerSources;
        TimeSpan dropTimer;
        const int dropInterval = 300; //milliseconds

        const int FUEL_BAR_Y = 10; 
        const int FUEL_BAR_HEIGHT = 30; 
        int FUEL_BAR_WIDTH; 

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
            physicsWorld.ContactListener = this;
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

            //initialise the debug renderer
            debugRenderer = new DebugRenderer(ScreenManager, gameFont);
            uint flags = 0;
            flags += (uint)DebugDrawFlags.Shape;
            flags += (uint)DebugDrawFlags.Joint;
            flags += (uint)DebugDrawFlags.AABB;
            flags += (uint)DebugDrawFlags.Pair;
            flags += (uint)DebugDrawFlags.CenterOfMass;
            debugRenderer.Flags = (DebugDrawFlags)flags;
            physicsWorld.DebugDraw = debugRenderer;
            
            
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

            effect = new BasicEffect(ScreenManager.GraphicsDevice);
            effect.VertexColorEnabled = true;
            //effect.EnableDefaultLighting(); //required?

            // Input
            inputManager = new InputManager(ScreenManager.Game);

            // Players
            playerOne = new PlayerCar(ScreenManager, physicsWorld, new Vector2(288, 344));
            playerTwo = new PlayerCar(ScreenManager, physicsWorld, new Vector2(352, 344));

            // Textures
            blank = this.content.Load<Texture2D>("blank");
            road = this.content.Load<Texture2D>("Backgrounds/roads1/preview");

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
            ScreenManager.Game.Components.Add(playerOne);
            ScreenManager.Game.Components.Add(playerTwo);
            ScreenManager.Game.Components.Add(inputManager);

            //set fuel bar to use the whole screen
            FUEL_BAR_WIDTH = ScreenManager.GraphicsDevice.Viewport.Width / 2;
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
                physicsWorld.Step((float)gameTime.ElapsedGameTime.TotalSeconds, 10, 10);

                dropTimer -= gameTime.ElapsedGameTime;
                if (dropTimer <= TimeSpan.FromSeconds(0))
                {
                    dropTimer = TimeSpan.FromMilliseconds(dropInterval);
                    PowerSource ps = new PowerSource(ScreenManager, physicsWorld, playerOne.Position2 + new Vector2(playerOne.texture.Width/2, playerOne.texture.Height));
                    powerSources.Add(ps);
                    ScreenManager.Game.Components.Add(ps);
                    ps = new PowerSource(ScreenManager, physicsWorld, playerTwo.Position2);
                    powerSources.Add(ps);
                    ScreenManager.Game.Components.Add(ps);
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

#if DEBUG
            //Draw physics world debug data
            physicsWorld.DrawDebugData();
#endif

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
            spriteBatch.Draw(blank, new Rectangle(ScreenManager.GraphicsDevice.Viewport.Width / 2 - (int)(FUEL_BAR_WIDTH * playerOne.getFuelPercent()), FUEL_BAR_Y, (int)(FUEL_BAR_WIDTH * playerOne.getFuelPercent()), FUEL_BAR_HEIGHT), Color.Goldenrod);
            spriteBatch.Draw(blank, new Rectangle(ScreenManager.GraphicsDevice.Viewport.Width / 2, FUEL_BAR_Y, (int)(FUEL_BAR_WIDTH * playerOne.getFuelPercent()), FUEL_BAR_HEIGHT), Color.Goldenrod);
            spriteBatch.Draw(blank, new Rectangle(ScreenManager.GraphicsDevice.Viewport.Width / 2 - 1, 0, 3, ScreenManager.GraphicsDevice.Viewport.Height), Color.Black); // Draws Black bar down Center
            //spriteBatch.Draw(blank, new Rectangle(ScreenManager.GraphicsDevice.Viewport.Width / 2, 0, 100, 5), Color.Pink);

            spriteBatch.DrawString(gameFont, "FUEL", new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2 + 10, -5), Color.Black);
            spriteBatch.DrawString(gameFont, "FUEL", new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2 - 95, -5), Color.Black);

            debugRenderer.FinishDrawString();
            spriteBatch.End();


            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);
                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }

        protected void DrawGameScreen(SpriteBatch spriteBatch, GameTime gameTime, PlayerCar player)
        {
            Matrix trans = Matrix.Identity;
            trans.M41 = ScreenManager.GraphicsDevice.Viewport.Width / 2;
            trans.M42 = ScreenManager.GraphicsDevice.Viewport.Height / 2;

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, RasterizerState.CullNone, null, trans * player.camera.View * halfprojectionMatrix);

            //TODO: replace with road drawablegamecomponent draw call
            spriteBatch.Draw(road, new Rectangle(0, 0, 1024, 768), Color.White);
            
            playerOne.Draw(spriteBatch, Color.Green);
            playerTwo.Draw(spriteBatch, Color.Red);
            

            //powerSource.Draw(spriteBatch);
            foreach(PowerSource ps in powerSources)
            {
                ps.Draw(spriteBatch);
            }


            spriteBatch.End();
            //debug rendering physics
            //effect.World = trans;
            //effect.View = player.camera.View;
            //effect.Projection = halfprojectionMatrix;

            effect.Techniques[0].Passes[0].Apply();
            debugRenderer.FinishDrawShapes();
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

        #region IContactListener Members

        public void BeginContact(Contact contact)
        {
            Body bodyA = contact.GetFixtureA().GetBody();
            Body bodyB = contact.GetFixtureB().GetBody();

            if (bodyA.GetUserData() != null && bodyB.GetUserData() != null)
            {
                GameObject bNodeA = (GameObject)bodyA.GetUserData();
                GameObject bNodeB = (GameObject)bodyB.GetUserData();

                //Collision scenarios
                if (bNodeA is PlayerCar && bNodeB is PowerSource)
                {
                    FuelUp((PlayerCar)bNodeA, (PowerSource)bNodeB);
                }
                if (bNodeA is PowerSource && bNodeB is PlayerCar)
                {
                    FuelUp((PlayerCar)bNodeB, (PowerSource)bNodeA);
                }

                if (bNodeA is PlayerCar && bNodeB is PlayerCar)
                {
                    ((PlayerCar)bNodeA).fuel = 0;
                }
            }
        }

        public void EndContact(Contact contact)
        {
        }

        public void PreSolve(Contact contact, ref Manifold oldManifold)
        {
        }

        public void PostSolve(Contact contact, ref ContactImpulse impulse)
        {
        }

        #endregion

        private void FuelUp(PlayerCar p, PowerSource ps)
        {
            //p.AddFuel();
            powerSources.Remove(ps);
            ScreenManager.Game.Components.Remove(ps);
        }
    }
}
