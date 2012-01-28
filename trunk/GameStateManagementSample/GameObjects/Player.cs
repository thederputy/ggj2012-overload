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
        protected Vector2 position2;

        public Vector2 Position2
        {
            get { return position2; }
            set
            {
                position2.X = position3.X = value.X;
                position2.Y = position3.Y = value.Y;
                position3.Z = 0;
            }
        }

        protected Vector3 position3;

        public Vector3 Position3
        {
            get { return position3; }
            set
            {
                position3.X = position2.X = value.X;
                position3.Y = position2.Y = value.Y;
                position3.Z = value.X;
            }
        }

        public Player()
        {
            Position2 = Vector2.Zero;
            Position3 = Vector3.Zero;
        }

        public Player(float x, float y)
        {
            Position2 = new Vector2(x, y);
            Position3 = new Vector3(x, y, 0);
        }

        public Player(Vector2 position)
        {
            Position2 = position;
            Position3 = new Vector3(position, 0);
        }
    }
}
