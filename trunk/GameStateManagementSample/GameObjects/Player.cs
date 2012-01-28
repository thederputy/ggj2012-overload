#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#endregion

namespace GameStateManagement.GameObjects
{
    class Player : DrawableGameComponent
    {
        protected Vector2 position;

        public Camera camera;

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

        public Player(Game game)
            :base (game)
        {
            position = Vector2.Zero;
            camera = new Camera(Position3);
        }

        public Player(Game game, float x, float y)
            :base (game)
        {
            position = new Vector2(x, y);
            camera = new Camera(Position3);
        }

        public Player(Game game, Vector2 position)
            :base (game)
        {
            this.position = position;
            camera = new Camera(Position3);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
