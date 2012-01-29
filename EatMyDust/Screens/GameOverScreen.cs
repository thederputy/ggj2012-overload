#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
#endregion

namespace EatMyDust
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class GameOverScreen : GameScreen
    {
        ContentManager content;

        SpriteFont font;

        Texture2D gameOverTexture;
        Vector2 gameOverLocation;
        int score;

        #region Initialization

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public GameOverScreen(int scorePassed)
        {
            score = scorePassed;
        }

        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            inputManager = new InputManager(ScreenManager.Game);

            //load font
            font = content.Load<SpriteFont>("gamefont");
            gameOverTexture = content.Load<Texture2D>("Sprites/gameover");
            gameOverLocation = new Vector2((ScreenManager.GraphicsDevice.Viewport.Width / 2) - (gameOverTexture.Width / 2), (ScreenManager.GraphicsDevice.Viewport.Height / 2) - (gameOverTexture.Height / 2)- (int)(font.MeasureString("Game Over!").Y));
 

        }

        public override void UnloadContent()
        {
            content.Unload();
        }

        #endregion
        #region update /draw
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            inputManager.Update(gameTime);
            handleInput();
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            spriteBatch.Begin();
            spriteBatch.Draw(gameOverTexture, new Rectangle((int)gameOverLocation.X, (int)gameOverLocation.Y, gameOverTexture.Width, gameOverTexture.Height), Color.White);
            spriteBatch.DrawString(font, "Score: " + score, new Vector2((ScreenManager.GraphicsDevice.Viewport.Width / 2) - (int)(font.MeasureString("Score: " + score).X / 2), (ScreenManager.GraphicsDevice.Viewport.Height / 2)), Color.White);
            spriteBatch.End();
            base.Draw(gameTime);
        }
        #endregion

        #region Handle Input

        public void handleInput()
        {
            if (inputManager.IsPressed(Keys.Enter, Buttons.Start, Buttons.A, 0))
            {
                LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(),
                                                           new MainMenuScreen(),
                                                           new HighScoreScreen(ScreenManager.Game),
                                                           new HighScoreEntryScreen(score));
            }
        }

        #endregion
    }
}
