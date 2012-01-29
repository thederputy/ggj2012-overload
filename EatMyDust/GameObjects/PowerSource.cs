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
        public float fade = 0;  //used for fade in/out purposes in spritebatch call
        public bool FADING_IN = true;

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
            texture = Game.Content.Load<Texture2D>("Sprites/dust");
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            timeToLive -= gameTime.ElapsedGameTime;
            if (timeToLive <= TimeSpan.FromSeconds(0))
                expired = true;
            handleFading();

            position.Y += gameplayScreen.ScrollSpeed;

            base.Update(gameTime);
        }

        public void handleFading()
        {
            if (FADING_IN)
            {
                if (fade < 1.0f)
                    fade += .05f;
                else
                    FADING_IN = false;
            }
        }
    }
}
