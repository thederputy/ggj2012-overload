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

        public HighScoreScreen(Game game)
        {
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

            inputManager = new InputManager(ScreenManager.Game);

            font = Content.Load<SpriteFont>("HighScoreFont");
        }

        public override void HandleInput(InputState input)
        {
            base.HandleInput(input);

            if (inputManager.IsPressed(Keys.Escape, Buttons.B, Buttons.Back, 0)
                || inputManager.IsPressed(Keys.Back, Buttons.B, Buttons.Back, 1))
            {
                ExitScreen();
                ScreenManager.AddScreen(new MainMenuScreen(), null);
                ScreenManager.AddScreen(new TitleScreen(), null);
            }

            if (inputManager.IsPressed(Keys.Space, Buttons.A, Buttons.Start, 0)
                || inputManager.IsPressed(Keys.Space, Buttons.A, Buttons.Start, 1))
            {
                ExitScreen();
                LoadingScreen.Load(ScreenManager, false, PlayerIndex.One, new GameplayScreen());
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

            sb.DrawString(font, "LONGEST RUNS", new Vector2(600, 50), Color.White);

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
                    string initialsOne = currentPair.Value.Substring(0, 3);
                    string initialsTwo = currentPair.Value.Substring(3);
                    sb.DrawString(font, i++.ToString() + ". " + initialsOne + ", " + initialsTwo, new Vector2(initialsX, bothY), Color.White);
                    sb.DrawString(font, currentPair.Key.ToString() + "m", new Vector2(scoreX, bothY), Color.White);
                    bothY += 50;
                    if (i == 11)
                    {
                        scoreX += 355;
                        initialsX += 355;
                        bothY = 100;
                    }
                    if (i == 21)
                    {
                        scoreX += 355;
                        initialsX += 355;
                        bothY = 100;
                    }
                }
#if WINDOWS
                sb.DrawString(font, "Press Space to Play Again, Escape / Backspace to go to the Main Menu", new Vector2(175, 600), Color.White);
#elif XBOX
                sb.DrawString(font, "Press A / Start to Play Again, B / Back to go to the Main Menu", new Vector2(250, 600), Color.White);
#endif
            }

            sb.End();
        }
    }
}
