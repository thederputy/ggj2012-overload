using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.IO;
using System.Collections;

namespace EatMyDust
{
    public class HighScoreScreen : GameScreen
    {
        List<KeyValuePair<int, String>> highScores;
        SpriteFont font;

        ContentManager Content;

        // Holds the background color of the console window
        private Texture2D rectangleTexture;

        // size and position of the console window
        private Rectangle bgrect;
        private Vector2 bgpos;

        public HighScoreScreen(Game game)
        {
            inputManager = new InputManager(game);

            HighScoreManager.LoadHighScores();
            highScores = HighScoreManager.highScoreList;
        }

        public override void LoadContent()
        {
            base.LoadContent();
            // Don't want to create another content manager if one already exists
            // but we need the one
            if (Content == null)
                Content = new ContentManager(ScreenManager.Game.Services, "Content");

            font = Content.Load<SpriteFont>("HighScoreFont");
            // Get the screen size and width
            int x, y, w, h;
            x = ScreenManager.GraphicsDevice.Viewport.X;
            y = ScreenManager.GraphicsDevice.Viewport.Y;
            w = ScreenManager.GraphicsDevice.Viewport.Width;
            h = ScreenManager.GraphicsDevice.Viewport.Height;

            // Create the rectangle with a small "border" around it
            bgrect = new Rectangle(x, y, w - 176, h - 64);
            bgpos = new Vector2(x + 86, y + 32);

            // Create a texture to apply to the rectangle
            rectangleTexture = new Texture2D(ScreenManager.GraphicsDevice, bgrect.Width, bgrect.Height, false, SurfaceFormat.Color);

            Color[] color = new Color[bgrect.Width * bgrect.Height];

            // loop through all the colors setting them to a dark blue
            for (int i = 0; i < color.Length; i++)
            {
                color[i] = new Color(43, 61, 89, 128);
            }

            rectangleTexture.SetData(color);
        }

        public override void HandleInput(InputState input)
        {
            base.HandleInput(input);

            if (inputManager.IsPressed(Keys.Escape, Buttons.Back, 0))
            {
                ExitScreen();
            }
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            SpriteBatch sb = ScreenManager.SpriteBatch;

            sb.Begin();

            // Draw the transparent background
            sb.Draw(rectangleTexture, bgpos, Color.White);

            sb.DrawString(font, "HIGH SCORES", new Vector2(300, 50), Color.White);
            sb.DrawString(font, "Press Esc or Back to Exit", new Vector2(100, bgrect.Height - 40), Color.White);

            int initialsX = 125;
            int scoreX = 325;
            int bothY = 100;
            int i = 1;
            if (highScores != null)
            {
                IEnumerator ScoreEnumerator = highScores.GetEnumerator(); //Getting the Enumerator
                ScoreEnumerator.Reset(); //Position at the Beginning
                while (ScoreEnumerator.MoveNext()) //Till not finished do print
                {
                    KeyValuePair<int, String> currentPair = (KeyValuePair<int, String>)ScoreEnumerator.Current;
                    sb.DrawString(font, i++.ToString() + ". " + currentPair.Value, new Vector2(initialsX, bothY), Color.White);
                    sb.DrawString(font, currentPair.Key.ToString(), new Vector2(scoreX, bothY), Color.White);
                    bothY += 50;
                    if (i == 6)
                    {
                        scoreX += 305;
                        initialsX += 305;
                        bothY = 150;
                    }
                }
            }

            sb.End();
        }
    }
}
