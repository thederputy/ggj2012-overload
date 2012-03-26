using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EatMyDust.GameObjects
{
    class Barricade : Obstacle
    {
        int length;
        
        public Barricade(GameplayScreen gameplayScreen)
            : base(gameplayScreen)
        {
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            Random rand = new Random();
            currentType = (ObstacleType)rand.Next(3);
            textureFiles = new List<string>();
            // ***ORDER OF STRINGS MUST CORRESPOND TO ORDER OF ENUM***
            textureFiles.Add("Sprites/Powerups/rock");
            //textureFiles.Add("Sprites/Powerups/stonewall");
            textureFiles.Add("Sprites/Powerups/crate_small");
            textureFiles.Add("Sprites/Powerups/barrel_small");
            texture = gameplayScreen.ScreenManager.Game.Content.Load<Texture2D>(textureFiles[(int)currentType]);
            expired = false;
            Position2 = new Vector2(rand.Next(275, 350), -32);
            length = 4;
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (position.Y > gameplayScreen.ScreenManager.GraphicsDevice.Viewport.Height)
                expired = true;
        }

       /* public void Draw(SpriteBatch spriteBatch, int repeat)
        {
            for (int i = 0; i < repeat; i++)
            {
                spriteBatch.Draw(texture, Position2 + new Vector2(texture.Width*i, 0), Color.White);
            }

        }*/


        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < length; i++)
            {
                spriteBatch.Draw(texture, Position2 + new Vector2(texture.Width * i, 0), Color.White);
            }
        }

        public bool CheckCollision(Rectangle playerRect)
        {
           // bool collided = false;
            Rectangle rect = boundingRect;

            rect.Width *= length;

            return playerRect.Intersects(rect);

            //for (int i = 0; i < length; i++)
            //{
                //collided = collided || playerRect.Intersects(rect);
                //rect.X += rect.Width;
            //}
           // return collided;
        }
    }
}
