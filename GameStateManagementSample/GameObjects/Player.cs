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
    class Player
    {
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

        public Player()
        {
            position = Vector2.Zero;
        }

        public Player(float x, float y)
        {
            position = new Vector2(x, y);
        }

        public Player(Vector2 position)
        {
            this.position = position;
        }
    }
}
