using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EatMyDust.GameObjects
{
    class PowerSource : GameObject
    {
        private int charge;
        private TimeSpan timeToLive; //seconds
        public bool expired;
        public PlayerCar createdBy;
        public Color color;

        public PowerSource(GameplayScreen gameplayScreen, Vector2 position, PlayerCar createdBy, Color color)
            : base(gameplayScreen)
        {
            this.createdBy = createdBy;
            this.color = color;
            this.position = position;
        }

        public override void Initialize()
        {
            charge = 5;
            timeToLive = TimeSpan.FromSeconds(3);
            expired = false;
            scaleFactor = 1.0f;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            texture = Game.Content.Load<Texture2D>("Sprites/powersource");
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
