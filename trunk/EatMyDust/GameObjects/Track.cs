using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml;
using Microsoft.Xna.Framework.Graphics;

namespace EatMyDust.GameObjects
{
    public class Track : GameObject
    {

        public List<Tile> tiles;

        int roadWidthTiles = 15;

        public Track(GameplayScreen gameplayScreen)
            : base(gameplayScreen)
        {
            tiles = new List<Tile>();

        }

        public override void Initialize()
        {
            //foreach (Tile t in tiles)
                //spriteBatch.Draw(t.texture, new Rectangle((int)t.position.X, (int)t.position.Y, (int)t.texture.Width, (int)t.texture.Height), Color.White);
        }

        public void generateTiles()
        {
        }

        public void generateNewTileSet()
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
