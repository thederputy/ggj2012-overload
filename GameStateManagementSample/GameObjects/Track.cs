using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml;
using Box2D.XNA;

namespace GameStateManagement.GameObjects
{
    public class Track
    {
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
