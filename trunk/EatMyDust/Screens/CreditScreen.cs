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
        
        public override void LoadContent()
        {
            inputManager = new InputManager(ScreenManager.Game);
            base.LoadContent();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            inputManager.Update(gameTime);
            if (inputManager.currentKeyboardState.IsKeyDown(Keys.Escape))
            {
                ExitScreen();
            }
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(Color.Black);
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            spriteBatch.Begin();
            spriteBatch.DrawString(ScreenManager.Font, "Credits text", new Vector2(25), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }


    }
}
