using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Box2D.XNA;

namespace GameStateManagement.GameObjects
{
    abstract class Sprite : DrawableGameComponent
    {
        protected ScreenManager screenManager;
        protected World physicsWorld;

        public Texture2D texture;
        protected Body body;
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

        public Body Body
        {
            get { return body; }
            set { body = value; }
        }

        public Sprite(ScreenManager screenManager, World physicsWorld)
            : base(screenManager.Game)
        {
            this.physicsWorld = physicsWorld;
        }

        public override void Initialize()
        {
            base.Initialize();
            CreateBody();
        }

        public void Draw(SpriteBatch spriteBatch, Color color)
        {
            spriteBatch.Draw(texture, Position2, null, color, body.Rotation, Vector2.Zero, 1.0f, SpriteEffects.None, 0);

            //base.Draw(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position2, Color.White);
        }

        public abstract void CreateBody();
    }
}
