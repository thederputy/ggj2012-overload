using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;
using Box2D.XNA;

namespace GameStateManagement
{
    class Track
    {
        public XmlDocument Xml { get; private set; }
        Body Body { get; set; }


        public void Load(string level)
        {
            Xml = new XmlDocument();
            Xml.Load(level);

            World world = new World(Vector2.Zero, false);

            BodyDef def = new BodyDef();
            def.type = BodyType.Static;
            Body body = world.CreateBody(def);
            body.SetUserData(this);
            EdgeShape shape = new EdgeShape();
            shape.Set(Vector2.Zero, Vector2.One);
            body.CreateFixture(shape, 1.0f);
        }
    }
}
