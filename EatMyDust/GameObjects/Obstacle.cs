using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace EatMyDust.GameObjects
{
    public class Obstacle : GameObject
    {
        public enum ObstacleType
        {
            Rock,
            Barricade
        }

        public bool expired;

        public List<String> textureFiles;

        public ObstacleType currentType;

        public Obstacle(GameplayScreen gameplayScreen)
            : base(gameplayScreen)
        {
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            Random rand = new Random();
            currentType = (ObstacleType)rand.Next(2);
            textureFiles = new List<string>();
            // ***ORDER OF STRINGS MUST CORRESPOND TO ORDER OF ENUM***
            textureFiles.Add("Sprites/Powerups/rock");
            textureFiles.Add("Sprites/Powerups/stonewall");
            texture = gameplayScreen.ScreenManager.Game.Content.Load<Texture2D>(textureFiles[(int)currentType]);
            expired = false;
            Position2 = new Vector2(rand.Next(300, 980), -32);
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            if (position.Y > gameplayScreen.ScreenManager.GraphicsDevice.Viewport.Height)
                expired = true;
            base.Update(gameTime);
        }
    }
}
