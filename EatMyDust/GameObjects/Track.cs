using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml;
using Box2D.XNA;
using Microsoft.Xna.Framework.Graphics;

namespace EatMyDust.GameObjects
{
    public class Track : DrawableGameComponent
    {
        public Track(ScreenManager screenManager)
            : base(screenManager.Game)
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(texture, Position2, Color.White);
        }


        public void Initialise()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("level.oel");


            foreach (XmlElement e in doc["level"]["Track"].GetElementsByTagName("TrackEdge") )
            {
                //TODO: add segment to the collision segments
            }
        }

    }
}
