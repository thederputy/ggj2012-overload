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

namespace EatMyDust
{
    public class HighScoreEntryScreen : GameScreen
    {
        char[] initials;
        SpriteFont menuFont;
        SpriteFont initialsFont;
        Texture2D selectionIndicator;
        private Vector2 titlePosition;
        private ContentManager Content;
        private int highScore;
        private int selectedChar;//which of the three characters is being changed
        private Vector2 selectionPosition;//position of the selection indicator
        private const int SELECTION_INCREMENT = 60;

        // Holds the background color of the console window
        private Texture2D rectangleTexture;

        // size and position of the console window
        private Rectangle bgrect;
        private Vector2 bgpos;

        public HighScoreEntryScreen(int highScore)
        {
            titlePosition = new Vector2(200, 50);
            this.highScore = highScore;
            initials = new char[3];
            initials = HighScoreManager.tempInitials;
            selectedChar = 0;
            selectionPosition = new Vector2(290, 105);
        }

        public override void LoadContent()
        {
            base.LoadContent();
            // Don't want to create another content manager if one already exists
            // but we need the one
            if (Content == null)
                Content = new ContentManager(ScreenManager.Game.Services, "Content");

            menuFont = Content.Load<SpriteFont>("menuFont");
            initialsFont = Content.Load<SpriteFont>("initialsFont");
            selectionIndicator = Content.Load<Texture2D>("blank");

            // Get the screen size and width
            int x, y, w, h;
            x = ScreenManager.GraphicsDevice.Viewport.X;
            y = ScreenManager.GraphicsDevice.Viewport.Y;
            w = ScreenManager.GraphicsDevice.Viewport.Width;
            h = ScreenManager.GraphicsDevice.Viewport.Height;

            // Create the rectangle with a small "border" around it
            bgrect = new Rectangle(x, y, w - 200, h - 200);
            bgpos = new Vector2(x + 100, y + 32);

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

            if (inputManager.IsPressed(Keys.Enter, Buttons.A, 0))
            {
                HighScoreManager.SaveHighScoreEntry(new String(initials), highScore);
                ExitScreen();
                ScreenManager.AddScreen(new HighScoreScreen(ScreenManager.Game), ControllingPlayer);
            }

            if (inputManager.IsPressed(Keys.Left, Buttons.LeftThumbstickLeft, 0)
                || inputManager.IsButtonPressed(Buttons.DPadLeft, 0))
            {
                if (selectedChar > 0)
                {
                    selectedChar--;
                    selectionPosition.X -= SELECTION_INCREMENT;
                }
                else
                {
                    selectedChar = 2;
                    selectionPosition.X += SELECTION_INCREMENT * 2;
                }
            }

            if (inputManager.IsPressed(Keys.Right, Buttons.LeftThumbstickRight, 0)
                || inputManager.IsButtonPressed(Buttons.DPadRight, 0))
            {
                if (selectedChar < 2)
                {
                    selectedChar++;
                    selectionPosition.X += SELECTION_INCREMENT;
                }
                else
                {
                    selectedChar = 0;
                    selectionPosition.X -= SELECTION_INCREMENT * 2;
                }
            }

            if (inputManager.IsPressed(Keys.Up, Buttons.LeftThumbstickUp, 0)
                || inputManager.IsButtonPressed(Buttons.DPadUp, 0))
            {
                if (initials[selectedChar] == 'A')
                    initials[selectedChar] = 'Z';
                else
                    initials[selectedChar]--;
            }

            if (inputManager.IsPressed(Keys.Down, Buttons.LeftThumbstickDown, 0)
                || inputManager.IsButtonPressed(Buttons.DPadDown, 0))
            {
                if (initials[selectedChar] == 'Z')
                    initials[selectedChar] = 'A';
                else
                    initials[selectedChar]++;
            }

            if (inputManager.IsPressed(Keys.Escape, Buttons.Back, 0))
            {
                ExitScreen();
            }
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            inputManager.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            SpriteBatch sb = ScreenManager.SpriteBatch;
            sb.Begin();

            // Draw the transparent background
            sb.Draw(rectangleTexture, bgpos, Color.White);

            sb.DrawString(menuFont, "Enter your initials:", titlePosition, Color.White);
            sb.DrawString(initialsFont, "" + initials[0], new Vector2(280, 115), Color.White);
            sb.DrawString(initialsFont, "" + initials[1], new Vector2(340, 115), Color.White);
            sb.DrawString(initialsFont, "" + initials[2], new Vector2(400, 115), Color.White);
            sb.Draw(selectionIndicator, new Rectangle((int)selectionPosition.X, (int)selectionPosition.Y, 30, 5), Color.Yellow);
            sb.Draw(selectionIndicator, new Rectangle((int)selectionPosition.X, (int)selectionPosition.Y + 65, 30, 5), Color.Yellow);
            sb.DrawString(menuFont, "A/Enter to save\nBack/Escape to return to game", new Vector2(titlePosition.X, titlePosition.Y + 150), Color.White);

            sb.End();
        }
    }
}
