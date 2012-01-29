using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml;
using Microsoft.Xna.Framework.Graphics;

namespace EatMyDust.GameObjects
{
    public class Tile
    {
        public Vector2 position;
        public Texture2D texture;

        public Tile(Texture2D texturePassed, Vector2 positionPassed)
        {
            position = positionPassed;
            texture = texturePassed;
        }


    }
}
