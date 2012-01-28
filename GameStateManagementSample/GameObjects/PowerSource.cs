using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameStateManagement.GameObjects
{
    class PowerSource : Sprite
    {
        private int charge;
        private float timeToLive; //seconds
        
        public PowerSource(ScreenManager screenManager) : base(screenManager)
        {
        }

        public override void Initialize()
        {
            charge = 5;
            timeToLive = 5;
            position = new Vector2(288, 464);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            texture = Game.Content.Load<Texture2D>("Sprites\\powersource");
            base.LoadContent();
        }
    }
}
