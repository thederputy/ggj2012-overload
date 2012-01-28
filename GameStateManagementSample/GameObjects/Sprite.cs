using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameStateManagement.GameObjects
{
    class Sprite : DrawableGameComponent
    {
        protected ScreenManager screenManager;

        public Texture2D texture;

        protected Vector2 position;

        public Vector2 Position2
        {
            get { return position; }
            set
            {
                position.X = value.X;
                position.Y = value.Y;
            }
        }

        public Vector3 Position3
        {
            get { return new Vector3(position, 0); }
            set
            {
                position.X = value.X;
                position.Y = value.Y;
            }
        }

        public Sprite(ScreenManager screenManager)
            : base(screenManager.Game)
        {
        }

        public void Draw(SpriteBatch spriteBatch, Color color)
        {
            spriteBatch.Draw(texture, Position2, color);

            //base.Draw(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position2, Color.White);
        }
    }
}
