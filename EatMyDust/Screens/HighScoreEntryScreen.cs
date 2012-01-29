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
        char[] initialsOne;
        char[] initialsTwo;
        bool playerOneReady;
        bool playerTwoReady;
        SpriteFont menuFont;
        SpriteFont initialsFont;
        Texture2D selectionIndicator;
        private Vector2 titlePosition;
        private ContentManager Content;
        private int highScore;
        private int selectedCharOne;//which of the three characters is being changed
        private Vector2 selectionPositionOne;//position of the selection indicator
        private int selectedCharTwo;//which of the three characters is being changed
        private Vector2 selectionPositionTwo;//position of the selection indicator
        private const int SELECTION_INCREMENT = 60;

        public HighScoreEntryScreen(int highScore)
        {
            titlePosition = new Vector2(200, 50);
            this.highScore = highScore;
            initialsOne = new char[3];
            initialsTwo = new char[3];
            initialsOne = HighScoreManager.tempInitialsOne;
            initialsTwo = HighScoreManager.tempInitialsTwo;
            selectedCharOne = 0;
            selectionPositionOne = new Vector2(284, 310);
            selectedCharTwo = 0;
            selectionPositionTwo = new Vector2(784, 310);
        }

        public override void LoadContent()
        {
            base.LoadContent();
            // Don't want to create another content manager if one already exists
            // but we need the one
            if (Content == null)
                Content = new ContentManager(ScreenManager.Game.Services, "Content");

            // Input
            inputManager = new InputManager(ScreenManager.Game);

            menuFont = Content.Load<SpriteFont>("menuFont");
            initialsFont = Content.Load<SpriteFont>("InitialsFont");
            selectionIndicator = Content.Load<Texture2D>("blank");
        }

        public override void HandleInput(InputState input)
        {
            base.HandleInput(input);

            #region Player One

            // Player one ready
            if (inputManager.IsPressed(Keys.Enter, Buttons.A, Buttons.Start, 0))
                playerOneReady = true;

            // Player one initials seletction
            if (inputManager.IsPressed(Keys.A, Buttons.LeftThumbstickLeft, 0)
                || inputManager.IsButtonPressed(Buttons.DPadLeft, 0))
            {
                if (selectedCharOne > 0)
                {
                    selectedCharOne--;
                    selectionPositionOne.X -= SELECTION_INCREMENT;
                }
                else
                {
                    selectedCharOne = 2;
                    selectionPositionOne.X += SELECTION_INCREMENT * 2;
                }
            }

            if (inputManager.IsPressed(Keys.D, Buttons.LeftThumbstickRight, 0)
                || inputManager.IsButtonPressed(Buttons.DPadRight, 0))
            {
                if (selectedCharOne < 2)
                {
                    selectedCharOne++;
                    selectionPositionOne.X += SELECTION_INCREMENT;
                }
                else
                {
                    selectedCharOne = 0;
                    selectionPositionOne.X -= SELECTION_INCREMENT * 2;
                }
            }

            if (inputManager.IsPressed(Keys.W, Buttons.LeftThumbstickUp, 0)
                || inputManager.IsButtonPressed(Buttons.DPadUp, 0))
            {
                if (initialsOne[selectedCharOne] == 'A')
                    initialsOne[selectedCharOne] = 'Z';
                else
                    initialsOne[selectedCharOne]--;
            }

            if (inputManager.IsPressed(Keys.S, Buttons.LeftThumbstickDown, 0)
                || inputManager.IsButtonPressed(Buttons.DPadDown, 0))
            {
                if (initialsOne[selectedCharOne] == 'Z')
                    initialsOne[selectedCharOne] = 'A';
                else
                    initialsOne[selectedCharOne]++;
            }

#endregion

            #region Player Two

            // Player two ready
            if (inputManager.IsPressed(Keys.Enter, Buttons.A, Buttons.Start, 1))
                playerTwoReady = true;

            // Player two initials seletction
            if (inputManager.IsPressed(Keys.Left, Buttons.LeftThumbstickLeft, 1)
                || inputManager.IsButtonPressed(Buttons.DPadLeft, 1))
            {
                if (selectedCharTwo > 0)
                {
                    selectedCharTwo--;
                    selectionPositionTwo.X -= SELECTION_INCREMENT;
                }
                else
                {
                    selectedCharTwo = 2;
                    selectionPositionTwo.X += SELECTION_INCREMENT * 2;
                }
            }

            if (inputManager.IsPressed(Keys.Right, Buttons.LeftThumbstickRight, 1)
                || inputManager.IsButtonPressed(Buttons.DPadRight, 1))
            {
                if (selectedCharTwo < 2)
                {
                    selectedCharTwo++;
                    selectionPositionTwo.X += SELECTION_INCREMENT;
                }
                else
                {
                    selectedCharTwo = 0;
                    selectionPositionTwo.X -= SELECTION_INCREMENT * 2;
                }
            }

            if (inputManager.IsPressed(Keys.Up, Buttons.LeftThumbstickUp, 1)
                || inputManager.IsButtonPressed(Buttons.DPadUp, 1))
            {
                if (initialsTwo[selectedCharTwo] == 'A')
                    initialsTwo[selectedCharTwo] = 'Z';
                else
                    initialsTwo[selectedCharTwo]--;
            }

            if (inputManager.IsPressed(Keys.Down, Buttons.LeftThumbstickDown, 1)
                || inputManager.IsButtonPressed(Buttons.DPadDown, 1))
            {
                if (initialsTwo[selectedCharTwo] == 'Z')
                    initialsTwo[selectedCharTwo] = 'A';
                else
                    initialsTwo[selectedCharTwo]++;
            }
            #endregion

            if (inputManager.IsPressed(Keys.Escape, Buttons.Back, 0)
                || inputManager.IsButtonPressed(Buttons.Back, 1))
            {
                ExitScreen();
            }
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            inputManager.Update(gameTime);

            if (playerOneReady && playerTwoReady)
            {
                char[] totalChars = new char[6];
                totalChars[0] = initialsOne[0];
                totalChars[1] = initialsOne[1];
                totalChars[2] = initialsOne[2];
                totalChars[3] = initialsTwo[0];
                totalChars[4] = initialsTwo[1];
                totalChars[5] = initialsTwo[2];

                HighScoreManager.SaveHighScoreEntry(new String(totalChars), highScore);
                ExitScreen();
                ScreenManager.AddScreen(new HighScoreScreen(ScreenManager.Game), ControllingPlayer);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            SpriteBatch sb = ScreenManager.SpriteBatch;
            sb.Begin();

            sb.DrawString(menuFont, "Player One initials:", new Vector2(titlePosition.X, titlePosition.Y + 200), Color.White);
            sb.DrawString(initialsFont, "" + initialsOne[0], new Vector2(280, 315), Color.White);
            sb.DrawString(initialsFont, "" + initialsOne[1], new Vector2(340, 315), Color.White);
            sb.DrawString(initialsFont, "" + initialsOne[2], new Vector2(400, 315), Color.White);
            sb.Draw(selectionIndicator, new Rectangle((int)selectionPositionOne.X, (int)selectionPositionOne.Y, 30, 5), Color.Yellow);
            sb.Draw(selectionIndicator, new Rectangle((int)selectionPositionOne.X, (int)selectionPositionOne.Y + 65, 30, 5), Color.Yellow);

            sb.DrawString(menuFont, "Player Two initials:", new Vector2(titlePosition.X + 500, titlePosition.Y + 200), Color.White);
            sb.DrawString(initialsFont, "" + initialsTwo[0], new Vector2(780, 315), Color.White);
            sb.DrawString(initialsFont, "" + initialsTwo[1], new Vector2(840, 315), Color.White);
            sb.DrawString(initialsFont, "" + initialsTwo[2], new Vector2(900, 315), Color.White);
            sb.Draw(selectionIndicator, new Rectangle((int)selectionPositionTwo.X, (int)selectionPositionTwo.Y, 30, 5), Color.Yellow);
            sb.Draw(selectionIndicator, new Rectangle((int)selectionPositionTwo.X, (int)selectionPositionTwo.Y + 65, 30, 5), Color.Yellow);

            if (playerOneReady)
                sb.DrawString(menuFont, "Player One Saved", new Vector2(titlePosition.X, titlePosition.Y + 350), Color.White);

            if (playerTwoReady)
                sb.DrawString(menuFont, "Player Two Saved", new Vector2(titlePosition.X + 500, titlePosition.Y + 350), Color.White);
            
#if WINDOWS
            sb.DrawString(menuFont, "Press Enter to save your score", new Vector2(titlePosition.X - 85, titlePosition.Y + 450), Color.White);
#elif XBOX
            sb.DrawString(menuFont, "Press A to save your score", new Vector2(titlePosition.X + 200, titlePosition.Y + 450), Color.White);
#endif

            sb.End();
        }
    }
}
