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
        public static readonly int mDivisions = 15;
        private VertexPositionColorTexture[] mVertices  = new VertexPositionColorTexture[mDivisions * 6];
        private BasicEffect mEffect;
        private float mSpeed;

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
            mEffect.World = Matrix.CreateTranslation(new Vector3(-0.5f * viewPort.Width, -0.5f * viewPort.Height, -99.0f));
            mEffect.View = Matrix.CreateLookAt(3f * Vector3.UnitZ, Vector3.Zero, Vector3.Up);
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
            //generate the vertices
            float viewportWidth = gameplayScreen.ScreenManager.GraphicsDevice.Viewport.Width;
            float viewportHeight = gameplayScreen.ScreenManager.GraphicsDevice.Viewport.Height;
            float divisionHeight = viewportHeight / (float)mDivisions;
            float zValue = -1.0f;
            vOffset += mSpeed * dt;

            for (int i = 0, index = 0; i < mDivisions; ++i, index += 6)
            {
                mVertices[index] = new VertexPositionColorTexture(new Vector3(viewportWidth, i * divisionHeight, zValue), Color.White, new Vector2(1, vOffset));
                mVertices[index + 1] = new VertexPositionColorTexture(new Vector3(0, i * divisionHeight, zValue), Color.White, new Vector2(0, vOffset));
                mVertices[index + 2] = new VertexPositionColorTexture(new Vector3(viewportWidth, (i + 1) * divisionHeight, zValue), Color.White, new Vector2(1, 1f + vOffset));

                mVertices[index + 3] = new VertexPositionColorTexture(new Vector3(0, i * divisionHeight, zValue), Color.White, new Vector2(0, vOffset));
                mVertices[index + 4] = new VertexPositionColorTexture(new Vector3(0, (i + 1) * divisionHeight, zValue), Color.White, new Vector2(0, 1f + vOffset));
                mVertices[index + 5] = new VertexPositionColorTexture(new Vector3(viewportWidth, (i + 1) * divisionHeight, zValue), Color.White, new Vector2(1, 1f + vOffset));
            }
        }

        public override void Update(GameTime gameTime)
        {
            GenerateVertices((float)gameTime.ElapsedGameTime.TotalSeconds);
            base.Update(gameTime);
        }

    }
}
