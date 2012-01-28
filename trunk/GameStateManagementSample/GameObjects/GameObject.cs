using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Box2D.XNA;

namespace GameStateManagement.GameObjects
{
    abstract class GameObject : DrawableGameComponent
    {
        protected ScreenManager screenManager;
        protected World physicsWorld;

        public Texture2D texture;
        protected Body body;

        public Vector2 Position2
        {
            get { return body.GetPosition(); }
            set { body.Position = value; }
        }

        public Vector3 Position3
        {
            get { return new Vector3(body.GetPosition(), 0); }
            set { body.Position = new Vector2(value.X, value.Y); }
        }

        public Body Body
        {
            get { return body; }
            set { body = value; }
        }

        public GameObject(ScreenManager screenManager, World physicsWorld)
            : base(screenManager.Game)
        {
            this.physicsWorld = physicsWorld;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public void Draw(SpriteBatch spriteBatch, Color color)
        {
            spriteBatch.Draw(texture, Position2, null, color, body.Rotation, Vector2.Zero, 3.0f, SpriteEffects.None, 0);

            //base.Draw(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position2, Color.White);
        }

        /// <summary>
        /// Must call on subclasses and override.
        /// </summary>
        public virtual void CreateBody(Vector2 position)
        {
        }
    }
}
