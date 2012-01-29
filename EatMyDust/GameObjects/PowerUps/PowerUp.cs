using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace EatMyDust.GameObjects
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class PowerUp : GameObject
    {
        /// <summary>
        /// The type of powerup it is.
        /// </summary>
        public enum PowerUpType
        {
            PositionSwap,
            SpeedBoost
        }

        public bool expired;

        public List<String> textureFiles;

        public PowerUpType currentType;

        public PowerUp(GameplayScreen gameplayScreen, PowerUpType powerUpType)
            : base(gameplayScreen)
        {
            // TODO: Construct any child components here
            currentType = powerUpType;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            textureFiles = new List<string>();
            // ***ORDER OF STRINGS MUST CORRESPOND TO ORDER OF ENUM***
            textureFiles.Add("Sprites/Powerups/swap");
            textureFiles.Add("Sprites/Powerups/star");
            texture = gameplayScreen.ScreenManager.Game.Content.Load<Texture2D>(textureFiles[(int)currentType]);
            expired = false;
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }
    }
}
