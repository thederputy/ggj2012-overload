﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EatMyDust.GameObjects
{
    public abstract class GameObject : DrawableGameComponent
    {
        public GameplayScreen gameplayScreen;

        public Texture2D texture;

        public Rectangle boundingRect;
        public Vector2 previousPosition;
       
        protected float scaleFactor;
                
        protected Vector2 position;
        public Vector2 Position2
        {
            get { return position; }
            set { position = value; }
        }

        public Vector3 Position3
        {
            get { return new Vector3(position, 0); }
            set { position = new Vector2(value.X, value.Y); }
        }

        public GameObject(GameplayScreen gameplayScreen)
            : base(gameplayScreen.ScreenManager.Game)
        {
            this.gameplayScreen = gameplayScreen;
            this.gameplayScreen.ScreenManager.Game.Components.Add(this);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (texture != null)
                boundingRect = new Rectangle((int)Position2.X, (int)Position2.Y, texture.Width, texture.Height);

        }

        public void Draw(SpriteBatch spriteBatch, Color color)
        {
            spriteBatch.Draw(texture, position, color);

            //base.Draw(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position2, Color.White);
        }

    }
}
