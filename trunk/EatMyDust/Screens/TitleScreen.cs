#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
#endregion

namespace EatMyDust
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class TitleScreen : GameScreen
    {
        ContentManager content;

        SpriteFont font;

        Texture2D carTexture;
        Texture2D eatTitleTexture;
        Texture2D myTitleTexture;
        Texture2D dustTitleTexture;
        Texture2D dustCloudTexture;
        Texture2D dustCloudBigTexture;

        Vector2 carLocation;
        Vector2 carFinalLocation;
        Vector2 carVelocity;
        int carYOffset;
        List<Vector2> previousCarLocations;
        int MAX_PREVIOUS_LOCATIONS = 30;

        //action flags
        bool CAR_DRIVING = true;
        bool TITLE_SEQUENCE = false;
        bool EAT = false;
        bool MY = false;
        bool DUST = false;

        bool drawHelper = true;

        float totalTitleTime = 0;

        Vector2 titleLocation;
        Vector2 dustCloudLocationCar;
        Vector2 dustCloudLocationEat;
        Vector2 dustCloudLocationMy;
        Vector2 dustCloudLocationDust;
        float dustCloudCarFade = 1.0f;
        float dustCloudEatFade = 1.0f;
        float dustCloudMyFade = 1.0f;
        float dustCloudDustFade = 1.0f;


        #region Initialization

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public TitleScreen()
        {
            
        }

        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            inputManager = new InputManager(ScreenManager.Game);

            //load font
            font = content.Load<SpriteFont>("gamefont"); 

            //load textures
            carTexture = content.Load<Texture2D>("Sprites/titlescreen_car"); 
            eatTitleTexture = content.Load<Texture2D>("Sprites/titlescreen_eat"); 
            myTitleTexture = content.Load<Texture2D>("Sprites/titlescreen_my"); 
            dustTitleTexture = content.Load<Texture2D>("Sprites/titlescreen_dust"); 
            dustCloudTexture = content.Load<Texture2D>("Sprites/titlescreen_dustcloud");
            dustCloudBigTexture = content.Load<Texture2D>("Sprites/titlescreen_dustcloud2"); 
            
            //set qualities
            carYOffset = ScreenManager.GraphicsDevice.Viewport.Height / 6;
            carLocation = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width, (ScreenManager.GraphicsDevice.Viewport.Height / 2) - (carTexture.Height / 2) + carYOffset);
            carFinalLocation = new Vector2((ScreenManager.GraphicsDevice.Viewport.Width / 2) - (carTexture.Width / 2), (ScreenManager.GraphicsDevice.Viewport.Height / 2) - (carTexture.Height / 2) + carYOffset);
            carVelocity = Vector2.Zero;
            previousCarLocations = new List<Vector2>();
            titleLocation = new Vector2((ScreenManager.GraphicsDevice.Viewport.Width / 2) - (carTexture.Width / 2), (ScreenManager.GraphicsDevice.Viewport.Height / 6));
            dustCloudLocationCar = new Vector2(carFinalLocation.X + carTexture.Width - 75, carFinalLocation.Y + carTexture.Height - 150);
            dustCloudLocationEat = new Vector2(titleLocation.X, titleLocation.Y + eatTitleTexture.Height / 2);
            dustCloudLocationMy = new Vector2(titleLocation.X + myTitleTexture.Width, titleLocation.Y + eatTitleTexture.Height / 2);
            dustCloudLocationDust = new Vector2(titleLocation.X + dustTitleTexture.Width / 2, titleLocation.Y + dustTitleTexture.Height);
        }

        public override void UnloadContent()
        {
            content.Unload();
        }

        #endregion
        #region update /draw
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            inputManager.Update(gameTime);
            handleInput();

            //adhere to max number of previous locations
            if (previousCarLocations.Count > MAX_PREVIOUS_LOCATIONS)
                previousCarLocations.RemoveAt(0);

            //car driving in sequence
            if (CAR_DRIVING)
            {
                previousCarLocations.Add(carLocation);
                carVelocity.X += .04f;
                carLocation.X -= carVelocity.X;
                base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

                if (carLocation.X <= carFinalLocation.X)
                    CAR_DRIVING = false;
            }
            else if (previousCarLocations.Count > 0)
            {
                //fade the blur effect once the car has stopped.
                previousCarLocations.RemoveAt(0);
                TITLE_SEQUENCE = true;
            }

            //title screen appearing sequence
            if (TITLE_SEQUENCE)
            {
                totalTitleTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                dustCloudCarFade -= .007f;
                dustCloudLocationCar.Y -= .3f;
                if (totalTitleTime >= 1.0)
                    EAT = true;
                if (totalTitleTime >= 2.0)
                    MY = true;
                if (totalTitleTime >= 3.0)
                    DUST = true;

                if (EAT)
                {
                    dustCloudEatFade -= .01f;
                    dustCloudLocationEat.Y -= .5f;
                }
                if (MY)
                {
                    dustCloudMyFade -= .01f;
                    dustCloudLocationMy.Y -= .5f;
                }
                if (DUST)
                {
                    dustCloudDustFade -= .01f;
                    dustCloudLocationDust.Y -= .5f;
                }

            }

        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            spriteBatch.Begin();
            spriteBatch.Draw(carTexture, new Rectangle((int)carLocation.X, (int)carLocation.Y, carTexture.Width, carTexture.Height), Color.White);
            foreach (Vector2 v in previousCarLocations) 
            {
                spriteBatch.Draw(carTexture, new Rectangle((int)v.X, (int)v.Y, carTexture.Width, carTexture.Height), Color.White * .05f);
            }
            if (TITLE_SEQUENCE)
            {
                spriteBatch.Draw(dustCloudTexture, new Rectangle((int)dustCloudLocationCar.X, (int)dustCloudLocationCar.Y, dustCloudTexture.Width, dustCloudTexture.Height), Color.White * dustCloudCarFade);
                spriteBatch.Draw(dustCloudBigTexture, new Rectangle((int)dustCloudLocationCar.X + 45, (int)dustCloudLocationCar.Y - 20, dustCloudTexture.Width, dustCloudTexture.Height), Color.White * dustCloudCarFade);
            }

            if (EAT)
            {
                spriteBatch.Draw(eatTitleTexture, new Rectangle((int)titleLocation.X, (int)titleLocation.Y, eatTitleTexture.Width, eatTitleTexture.Height), Color.White);
                spriteBatch.Draw(dustCloudTexture, new Rectangle((int)dustCloudLocationEat.X, (int)dustCloudLocationEat.Y, dustCloudTexture.Width, dustCloudTexture.Height), Color.White * dustCloudEatFade);
                spriteBatch.Draw(dustCloudBigTexture, new Rectangle((int)dustCloudLocationEat.X - 20, (int)dustCloudLocationEat.Y - 20, dustCloudBigTexture.Width, dustCloudBigTexture.Height), Color.White * dustCloudEatFade); 
            }
            if (MY)
            {
                spriteBatch.Draw(myTitleTexture, new Rectangle((int)titleLocation.X, (int)titleLocation.Y, eatTitleTexture.Width, eatTitleTexture.Height), Color.White);
                spriteBatch.Draw(dustCloudTexture, new Rectangle((int)dustCloudLocationMy.X - 60, (int)dustCloudLocationMy.Y, dustCloudTexture.Width, dustCloudTexture.Height), Color.White * dustCloudMyFade);
                spriteBatch.Draw(dustCloudBigTexture, new Rectangle((int)dustCloudLocationMy.X - 40, (int)dustCloudLocationMy.Y - 30, dustCloudBigTexture.Width, dustCloudBigTexture.Height), Color.White * dustCloudEatFade); 
            }
            if (DUST)
            {
                spriteBatch.Draw(dustTitleTexture, new Rectangle((int)titleLocation.X, (int)titleLocation.Y, eatTitleTexture.Width, eatTitleTexture.Height), Color.White);
                spriteBatch.Draw(dustCloudTexture, new Rectangle((int)dustCloudLocationDust.X, (int)dustCloudLocationDust.Y, dustCloudTexture.Width, dustCloudTexture.Height), Color.White * dustCloudDustFade);
                spriteBatch.Draw(dustCloudBigTexture, new Rectangle((int)dustCloudLocationDust.X + 20, (int)dustCloudLocationDust.Y - 30, dustCloudBigTexture.Width, dustCloudBigTexture.Height), Color.White * dustCloudDustFade);
                spriteBatch.Draw(dustCloudBigTexture, new Rectangle((int)dustCloudLocationDust.X - 40, (int)dustCloudLocationDust.Y - 10, dustCloudBigTexture.Width, dustCloudBigTexture.Height), Color.White * dustCloudDustFade);
                if (drawHelper)
                    spriteBatch.DrawString(font, "Press Enter/Start!", new Vector2((ScreenManager.GraphicsDevice.Viewport.Width / 2) - (int)(font.MeasureString("Press Enter/Start!").X / 2), carFinalLocation.Y + carTexture.Height), Color.White);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
        #endregion

        #region Handle Input

        public void handleInput()
        {
            if (inputManager.IsPressed(Keys.Enter, Buttons.Start, 0))
            {
                setFinalPosition();
                drawHelper = false;
                ScreenManager.AddScreen(new MainMenuScreen(), null);
            }
        }

        public void setFinalPosition()
        {
            if (!DUST)
                carLocation = carFinalLocation;

        }


        #endregion
    }
}
