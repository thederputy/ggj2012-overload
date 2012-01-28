using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameStateManagement.GameObjects
{
    class PowerSource : Sprite
    {
        private int charge;
        private TimeSpan timeToLive; //seconds
        public bool expired;
        
        public PowerSource(ScreenManager screenManager, Vector2 position) : base(screenManager)
        {
            this.position = position;
        }

        public override void Initialize()
        {
            charge = 5;
            timeToLive = TimeSpan.FromSeconds(3);
            expired = false;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            texture = Game.Content.Load<Texture2D>("Sprites\\powersource");
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            timeToLive -= gameTime.ElapsedGameTime;
            if (timeToLive <= TimeSpan.FromSeconds(0))
                expired = true;

            base.Update(gameTime);
        }
    }
}
