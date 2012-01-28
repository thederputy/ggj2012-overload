using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Box2D.XNA;

namespace GameStateManagement.GameObjects
{
    class PowerSource : GameObject
    {
        private int charge;
        private TimeSpan timeToLive; //seconds
        public bool expired;

        public PowerSource(ScreenManager screenManager, World physicsWorld, Vector2 position)
            : base(screenManager, physicsWorld)
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

        public override void CreateBody()
        {
            BodyDef def = new BodyDef();
            def.userData = this;
            def.position = this.position;
            def.type = BodyType.Static;
            body = physicsWorld.CreateBody(def);
            CircleShape shape = new CircleShape();
            shape._radius = 10.0f;
            FixtureDef fixtureDef = new FixtureDef();
            fixtureDef.isSensor = true;
            fixtureDef.shape = shape;
            body.CreateFixture(shape, 1.0f);
        }

    }
}
