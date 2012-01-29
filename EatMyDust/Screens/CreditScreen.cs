using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace EatMyDust
{
    class CreditScreen : GameScreen
    {
        String createdBy;
        String thanksTo;
        public override void LoadContent()
        {
            inputManager = new InputManager(ScreenManager.Game);

            createdBy = "Tim Hargreaves@Tatham Johnson@James Karg@Jacob Kwitkoski@Shannon Lee@Tom McIntosh@Shane Morin";
            createdBy = createdBy.Replace("@", System.Environment.NewLine);

            thanksTo = "Nick Waanders@Graham Jans@Jake Birkett@Brian Provinciano for the@Retro City Rampage cars@Shane Neville@Blasterhead for the BGM@Dr. Box & Kim Voll@Starbucks@Vancouver GGJ Community@opengame.org and FreeSound";
            thanksTo = thanksTo.Replace("@", System.Environment.NewLine);


            base.LoadContent();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            inputManager.Update(gameTime);
            if (inputManager.currentKeyboardState.IsKeyDown(Keys.Escape))
            {
                ExitScreen();
                LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(), new BackgroundScreen(),
                                                                                        new MainMenuScreen(),
                                                                                        new TitleScreen());
            }
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            //ScreenManager.GraphicsDevice.Clear(Color.Black);
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            spriteBatch.Begin();
            spriteBatch.DrawString(ScreenManager.Font, "Credits", new Vector2(ScreenManager.GraphicsDevice.Viewport.Width/2 - 30, 30), Color.White);
            spriteBatch.DrawString(ScreenManager.Font, "Created By", new Vector2(250, 90), Color.White);
            spriteBatch.DrawString(ScreenManager.Font, createdBy, new Vector2(250, 150), Color.White);

            spriteBatch.DrawString(ScreenManager.Font, "Thanks To", new Vector2(ScreenManager.GraphicsDevice.Viewport.Width/2 + 90, 90), Color.White);
            spriteBatch.DrawString(ScreenManager.Font, thanksTo, new Vector2(ScreenManager.GraphicsDevice.Viewport.Width/2 + 90, 150), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }


    }
}
