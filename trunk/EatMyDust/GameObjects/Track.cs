﻿using System;
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
        public static readonly int mDivisions = 16;
        private VertexPositionColorTexture[] mVertices  = new VertexPositionColorTexture[mDivisions * 6];
        private float[] mOffsets = new float[mDivisions + 1];
        private BasicEffect mEffect;
        private float mSpeed;

        private float mNextOffsetDistance = 0;
        private float mNextOffsetTime = 0;
        private float mNextOffsetDelay = 0;


        public float Speed
        {
            get { return mSpeed; }
            set { mSpeed = value; }
        }
        private float vOffset;

        public Track(GameplayScreen gameplayScreen)
            : base(gameplayScreen)
        {
        }

        public new void Draw(GameTime gameTime)
        {
            SamplerState ss = new SamplerState();
            ss.AddressU = TextureAddressMode.Wrap;
            ss.AddressV = TextureAddressMode.Wrap;
            gameplayScreen.ScreenManager.GraphicsDevice.SamplerStates[0] = ss; mEffect.Techniques[0].Passes[0].Apply();
            gameplayScreen.ScreenManager.GraphicsDevice.DrawUserPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList, mVertices, 0, mDivisions * 2);
        }

        protected override void LoadContent()
        {
            texture = Game.Content.Load<Texture2D>("roadtile");
            mEffect = new BasicEffect(gameplayScreen.ScreenManager.GraphicsDevice);
            mEffect.LightingEnabled = false;
            mEffect.Texture = texture;
            mEffect.TextureEnabled = true;
            mEffect.VertexColorEnabled = true;
            Viewport viewPort = gameplayScreen.ScreenManager.GraphicsDevice.Viewport;
            mEffect.World = Matrix.CreateTranslation(new Vector3(-0.5f * viewPort.Width, -0.5f * viewPort.Height, -1.0f));
            mEffect.View = Matrix.CreateLookAt(Vector3.UnitZ, Vector3.Zero, Vector3.Up);
            mEffect.Projection = Matrix.CreateOrthographic(viewPort.Width, viewPort.Height, 1f, 1000f);
            base.LoadContent();
        }



        public override void Initialize()
        {
            mSpeed = 0f;
            GenerateVertices(0);
            base.Initialize();
        }

        private void GenerateVertices(float dt)
        {
            float viewportWidth = gameplayScreen.ScreenManager.GraphicsDevice.Viewport.Width;
            float viewportHeight = gameplayScreen.ScreenManager.GraphicsDevice.Viewport.Height;

            //update the offsets
            mNextOffsetDelay -= dt;
            if (mNextOffsetDelay < 0f)
            {
                //randomise the next offset
                Random random = new Random();
                mNextOffsetTime = 0.5f + 1.0f * (float)random.NextDouble();
                float roadFraction = 0.7f;
                mNextOffsetDistance = (roadFraction * viewportWidth * (float)random.NextDouble()) - 0.5f*roadFraction * viewportWidth;
                mNextOffsetDelay = 3f * (float)random.NextDouble();
            }

            //generate the vertices
            float divisionHeight = viewportHeight / (float)(mDivisions-1);
            float zValue = -1.0f;
            vOffset += mSpeed * dt;

            if (vOffset > 1.0f)
            {
                vOffset -= 1.0f;
                //push all the offsets down
                for (int i = 0; i < mDivisions; ++i)
                {
                    mOffsets[i] = mOffsets[i + 1];
                }
                mOffsets[mDivisions] = MathHelper.Lerp(mOffsets[mDivisions - 1], mNextOffsetDistance, dt / mNextOffsetTime);
            }

            for (int i = 0, index = 0; i < mDivisions; ++i, index += 6)
            {
                float offsetBottom = mOffsets[i];
                float offsetTop = mOffsets[i+1];
                mVertices[index] = new VertexPositionColorTexture(new Vector3(viewportWidth + offsetBottom, i * divisionHeight - vOffset*divisionHeight, zValue), Color.White, new Vector2(1, 0));
                mVertices[index + 1] = new VertexPositionColorTexture(new Vector3(offsetBottom, i * divisionHeight - vOffset * divisionHeight, zValue), Color.White, new Vector2(0, 0));
                mVertices[index + 2] = new VertexPositionColorTexture(new Vector3(viewportWidth + offsetTop, (i + 1) * divisionHeight - vOffset * divisionHeight, zValue), Color.White, new Vector2(1, 1f));

                mVertices[index + 3] = new VertexPositionColorTexture(new Vector3(offsetBottom, i * divisionHeight - vOffset * divisionHeight, zValue), Color.White, new Vector2(0, 0));
                mVertices[index + 4] = new VertexPositionColorTexture(new Vector3(offsetTop, (i + 1) * divisionHeight - vOffset * divisionHeight, zValue), Color.White, new Vector2(0, 1f));
                mVertices[index + 5] = new VertexPositionColorTexture(new Vector3(viewportWidth + offsetTop, (i + 1) * divisionHeight - vOffset * divisionHeight, zValue), Color.White, new Vector2(1, 1f));
            }
        }

        public override void Update(GameTime gameTime)
        {
            GenerateVertices((float)gameTime.ElapsedGameTime.TotalSeconds);
            base.Update(gameTime);
        }

    }
}
