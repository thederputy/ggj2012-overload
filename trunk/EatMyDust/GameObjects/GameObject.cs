using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EatMyDust.GameObjects
{
    public abstract class GameObject : DrawableGameComponent
    {
        public GameplayScreen gameplayScreen;

        public Texture2D texture;

        protected float scaleFactor;

        protected Vector2 position;
        public Vector2 Position2
        {
            get { return position; }
            set { position = value; }
        }

        public Vector3 Position3
        {
            get { return new Vector3(position, 0); }
            set { position = new Vector2(value.X, value.Y); }
        }

        public GameObject(GameplayScreen gameplayScreen)
            : base(gameplayScreen.ScreenManager.Game)
        {
            this.gameplayScreen = gameplayScreen;
            this.gameplayScreen.ScreenManager.Game.Components.Add(this);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public void Draw(SpriteBatch spriteBatch, Color color)
        {
            spriteBatch.Draw(texture, position, Color.White);

            //base.Draw(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position2, Color.White);
        }
    }
}
