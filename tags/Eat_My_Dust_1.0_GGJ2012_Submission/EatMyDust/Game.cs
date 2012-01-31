#region File Description
//-----------------------------------------------------------------------------
// Game.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using System;

#if XBOX
using Microsoft.Xna.Framework.GamerServices;
#endif
#endregion

namespace EatMyDust
{
    /// <summary>
    /// Sample showing how to manage different game states, with transitions
    /// between menu screens, a loading screen, the game itself, and a pause
    /// menu. This main game class is extremely simple: all the interesting
    /// stuff happens in the ScreenManager component.
    /// </summary>
    public class EatMyDustGame : Microsoft.Xna.Framework.Game
    {
        #region Fields

        GraphicsDeviceManager graphics;
        ScreenManager screenManager;


        // By preloading any assets used by UI rendering, we avoid framerate glitches
        // when they suddenly need to be loaded in the middle of a menu transition.
        static readonly string[] preloadAssets =
        {
            "gradient",
        };

        
        #endregion

        #region Initialization


        /// <summary>
        /// The main game constructor.
        /// </summary>
        public EatMyDustGame()
        {
            Content.RootDirectory = "Content";

            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;

#if !DEBUG
            graphics.IsFullScreen = true;
#endif

#if DEBUG
            this.IsMouseVisible = true;
#endif
            // Create the screen manager component.
            screenManager = new ScreenManager(this);

            Components.Add(screenManager);

#if XBOX
            // Required for !Guide.IsVisible calls
            this.Components.Add(new GamerServicesComponent(this));
#endif

            // Activate the first screens.
            // TJH PUT THESE BACK TO DO THE MENUS
            screenManager.AddScreen(new BackgroundScreen(), null);
            screenManager.AddScreen(new TitleScreen(), null);
            //screenManager.AddScreen(new MainMenuScreen(), null);
            // PUT THOSE BACK
            //screenManager.AddScreen(new GameplayScreen(),0);
        }


        /// <summary>
        /// Loads graphics content.
        /// </summary>
        protected override void LoadContent()
        {
            foreach (string asset in preloadAssets)
            {
                Content.Load<object>(asset);
            }
        }


        #endregion

        #region Draw

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);

            base.Draw(gameTime);
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Will prompt at start of game for a storage device
            if (HighScoreManager.storageDevice == null)
            {
#if WINDOWS
                if (!HighScoreManager.deviceRequested)
                {
                    HighScoreManager.deviceRequested = true;
                    HighScoreManager.syncResult = StorageDevice.BeginShowSelector(PlayerIndex.One, null, null);
                }
#elif XBOX
                if ((!Guide.IsVisible) && !HighScoreManager.deviceRequested)
                {
                    HighScoreManager.deviceRequested = true;
                    HighScoreManager.syncResult = StorageDevice.BeginShowSelector(PlayerIndex.One, null, null);
                }
#endif
            }

            if ((HighScoreManager.deviceRequested) && (HighScoreManager.syncResult.IsCompleted))
            {
                // Storage device is chosen, save the reference to it.
                HighScoreManager.deviceRequested = false;
                HighScoreManager.GetStorageDevice();
            }

            // If there are no more screens, exit the game
            if (screenManager.GetScreens().Length == 0)
            {
                this.Exit();
            }
        }

        #endregion
    }


    #region Entry Point

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    static class Program
    {
        static void Main()
        {
            using (EatMyDustGame game = new EatMyDustGame())
            {
                game.Run();
            }
        }
    }

    #endregion
}
