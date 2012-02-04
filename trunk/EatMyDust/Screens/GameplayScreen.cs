#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using EatMyDust.GameObjects;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
#endregion

namespace EatMyDust
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    public class GameplayScreen : GameScreen
    {
        #region Tweaking vars
        public static readonly float DefaultScrollSpeed = 15.0f;
        public static readonly float GrassScrollSpeed = 8.0f;
        public static readonly float BoostScrollSpeed = 25.0f;

        public static readonly float PowerSourceDropTime = 0.1f;
        public static readonly float PowerUpDropTime = 8f;

        public static readonly float BarricadeSpawnTimeMin = 10f;
        public static readonly float BarricadeSpawnTimeMax = 20f;

        public static readonly Color playerOneColor = Color.Red;
        public static readonly Color playerTwoColor = Color.Yellow;

        public static readonly float ObstacleSpawnTimeMin = 1f;
        public static readonly float ObstacleSpawnTimeMax = 6f;


        #endregion

        #region Fields

        ContentManager content;
        SpriteFont gameFont;
        
        PlayerCar playerOne;
        public PlayerCar PlayerOne
        {
            get { return playerOne; }
        }
        
        PlayerCar playerTwo;
        public PlayerCar PlayerTwo
        {
            get { return playerTwo; }
        }

        Track track;
        public Track Track
        {
            get { return track; }
            set { track = value; }
        }

        public float ScrollSpeed
        {
            get { return track.Speed;  }
            set { track.Speed = value; }
        }
        
        Random random = new Random();

        float pauseAlpha;

        //Textures
        Texture2D blank;
        Texture2D road;
        Texture2D gasBarL;
        Texture2D gasBarR;
        Texture2D gasBarLH;
        Texture2D gasBarRH;
        Texture2D billBoardLeft;
        Texture2D billBoardRight;
        Texture2D waves;
        Texture2D explosion;

        Vector2 explosionPosition;

        BasicEffect effect;

        List<PowerSource> powerSources;
        TimeSpan dropTimer;

        TimeSpan flashTimerP1;
        TimeSpan flashTimerP2;
        int flashSpeed = 2000;

        List<PowerUp> powerUps;
        TimeSpan powerupTimer;
        const int powerupDropInterval = 5000;

        List<Obstacle> obstacles;
        TimeSpan obstacleTimer;
        TimeSpan barricadeTimer;

        const int BILLBOARD_INSET = 80;
        float billBoard_Y;

        Random rand;

        int FUEL_BAR_Y = 10; 
        int FUEL_BAR_WIDTH = 40;
        int FUEL_BAR_HEIGHT = 30;
        const int FUEL_BAR_INSET = 30;
        const int FUEL_BAR_INC = 70;
        float waveDDisplace = 0;
        const int BILLBOARD_UPPERSET_Y_INSET = 400;


        // sounds effects and music         
        SoundEffect collectPower; // BG Music
        SoundEffectInstance collectPowerInstance;
        SoundEffect collideFX; // BG Music
        SoundEffectInstance collideInstance;
        SoundEffect engineFX; // BG Music
        SoundEffectInstance engineInstance;
        SoundEffect doubleHonkFX;
        SoundEffectInstance doubleHonkInstance;

        public int score;

        bool gameOverCondition;
        bool gameStarted = false;
        #endregion

        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            rand = new Random();
        }

        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            // Input
            inputManager = new InputManager(ScreenManager.Game);

            gameFont = content.Load<SpriteFont>("gamefont");

            // Music
            MediaPlayer.Stop();
            MediaPlayer.Play(ScreenManager.gameplayMusic);
            MediaPlayer.Volume = 0.25f;

            // Players
            playerOne = new PlayerCar(this, new Vector2(ScreenManager.GraphicsDevice.Viewport.Width/2, ScreenManager.GraphicsDevice.Viewport.Height/2), 0);
            //playerOne.Position2 -= new Vector2(playerOne.texture.Width * 2, 0);
            playerOne.Position2 += new Vector2(-128, -32);
            playerOne.SetTexture();
            playerTwo = new PlayerCar(this, new Vector2(ScreenManager.GraphicsDevice.Viewport.Width/2, ScreenManager.GraphicsDevice.Viewport.Height/2), 1);
            //playerTwo.Position2 += new Vector2(playerTwo.texture.Width, 0);
            playerTwo.Position2 += new Vector2(64, -32);
            playerTwo.SetTexture();

            // Textures
            blank = this.content.Load<Texture2D>("blank");
            road = this.content.Load<Texture2D>("Backgrounds/roads1/preview");
            gasBarL = this.content.Load<Texture2D>("Sprites/gasBar_A");
            gasBarR = this.content.Load<Texture2D>("Sprites/gasBar_B");
            gasBarLH = this.content.Load<Texture2D>("Sprites/gasBar_D");
            gasBarRH = this.content.Load<Texture2D>("Sprites/gasBar_C");
            billBoardLeft = this.content.Load<Texture2D>("Sprites/billboardA");
            billBoardRight = this.content.Load<Texture2D>("Sprites/billboardB");
            waves = this.content.Load<Texture2D>("waves");
            explosion = this.content.Load<Texture2D>("Sprites/bigboom");


            track = new Track(this);

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();

            dropTimer = TimeSpan.FromSeconds(PowerSourceDropTime);
            powerSources = new List<PowerSource>();

            powerUps = new List<PowerUp>();
            /*PowerUp pup = new PowerUp(this, PowerUp.Type.PositionSwap);
            pup.Position2 = Vector2.Zero;
            powerUps.Add(pup);

            pup = new PowerUp(this, PowerUp.Type.SpeedBoost);
            pup.Position2 = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 4, ScreenManager.GraphicsDevice.Viewport.Height / 4);
            powerUps.Add(pup);
            */
            powerupTimer = TimeSpan.FromSeconds(PowerUpDropTime);
            flashTimerP1 = TimeSpan.FromMilliseconds(flashSpeed);
            flashTimerP2 = TimeSpan.FromMilliseconds(flashSpeed);
            obstacles = new List<Obstacle>();
            obstacleTimer = TimeSpan.FromSeconds(ObstacleSpawnTimeMax);
            barricadeTimer = TimeSpan.FromSeconds(BarricadeSpawnTimeMax);
            // Add the game components to the game
            // This allows each component's Initialize, Update, Draw to get called automatically.
            ScreenManager.Game.Components.Add(inputManager);

            //set fuel bar to use the whole screen
            FUEL_BAR_Y = ScreenManager.GraphicsDevice.Viewport.Height / 2;
            FUEL_BAR_HEIGHT = ScreenManager.GraphicsDevice.Viewport.Width / 4;
            billBoard_Y = ScreenManager.GraphicsDevice.Viewport.Height - 300;
            waveDDisplace = -waves.Height;

            //MUSIC AND SOUND LOADING
            
            collectPower = this.content.Load<SoundEffect>("Sounds/collect");
            collectPowerInstance = collectPower.CreateInstance();

            collideFX = this.content.Load<SoundEffect>("Sounds/hit");
            collideInstance = collideFX.CreateInstance();

            doubleHonkFX = this.content.Load<SoundEffect>("Sounds/doubleHonk");
            doubleHonkInstance = doubleHonkFX.CreateInstance();

            //SCORING
            score = 0;

            //GAME STATE
            gameOverCondition = false;
        }

        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
        }

        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            // input
            inputManager.Update(gameTime);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {
                CheckForCollisions();

                if (playerOne.fuel <= 0 && playerTwo.fuel <= 0)
                    GameOver();
                else if (playerOne.boosting || playerTwo.boosting)
                    ScrollSpeed = BoostScrollSpeed;


                dropTimer -= gameTime.ElapsedGameTime;
                gameStarted = playerOne.started || playerTwo.started;
                if (dropTimer <= TimeSpan.FromSeconds(0))
                {
                    dropTimer = TimeSpan.FromSeconds(PowerSourceDropTime);
                    if (gameStarted)
                    {
                        PowerSource ps = new PowerSource(this, playerOne.Position2 + new Vector2(0.5f * playerOne.texture.Width, playerOne.texture.Height), playerOne, playerOneColor);
                        ps.Position2 += new Vector2(-0.5f * ps.texture.Width, -0.5f * ps.texture.Height);
                        powerSources.Add(ps);
                        ps = new PowerSource(this, playerTwo.Position2 + new Vector2(0.5f * playerTwo.texture.Width, playerTwo.texture.Height), playerTwo, playerTwoColor);
                        ps.Position2 += new Vector2(-0.5f * ps.texture.Width, -0.5f * ps.texture.Height);
                        powerSources.Add(ps);
                    }
                }
                for (int i = 0; i < powerSources.Count; i++)
                {
                    if (powerSources[i].expired)
                    {
                        ScreenManager.Game.Components.Remove(powerSources[i]);
                        powerSources.Remove(powerSources[i]);
                        i--;
                    }
                }
                for (int i = 0; i < powerUps.Count; i++)
                {
                    if (powerUps[i].expired)
                    {
                        ScreenManager.Game.Components.Remove(powerUps[i]);
                        powerUps.Remove(powerUps[i]);
                        i--;
                    }
                    else
                    {
                        powerUps[i].Position2 += new Vector2(0, ScrollSpeed/2);
                    }
                }

                powerupTimer -= gameTime.ElapsedGameTime;
                if (powerupTimer <= TimeSpan.FromSeconds(0))
                {
                    powerupTimer = TimeSpan.FromSeconds(PowerUpDropTime);

                    if (gameStarted)
                    {
                        PowerUp pup = new PowerUp(this);
                        powerUps.Add(pup);
                    }
                }

                for (int i = 0; i < obstacles.Count; i++)
                {
                    if (obstacles[i].expired)
                    {
                        ScreenManager.Game.Components.Remove(obstacles[i]);
                        obstacles.Remove(obstacles[i]);
                        i--;
                    }
                }

                obstacleTimer -= gameTime.ElapsedGameTime;
                if (obstacleTimer <= TimeSpan.FromSeconds(0))
                {
                    obstacleTimer = TimeSpan.FromSeconds(ObstacleSpawnTimeMin + rand.NextDouble() * (ObstacleSpawnTimeMax - ObstacleSpawnTimeMin));
                    if (gameStarted)
                    {
                        Obstacle obs = new Obstacle(this);
                        obstacles.Add(obs);
                    }
                }

                barricadeTimer -= gameTime.ElapsedGameTime;
                if (barricadeTimer <= TimeSpan.FromSeconds(0))
                {
                    barricadeTimer = TimeSpan.FromSeconds(BarricadeSpawnTimeMin + rand.NextDouble()*(BarricadeSpawnTimeMax-BarricadeSpawnTimeMin) );
                    if (gameStarted)
                    {
                        Barricade bar = new Barricade(this);
                        obstacles.Add(bar);
                    }
                }

                score += (int)(Math.Ceiling(0.1f*ScrollSpeed));

                // Check for game over state
                if (playerOne.fuel == 0 || playerTwo.fuel == 0)
                {
                    gameOverCondition = true;
                }

                if (gameOverCondition)
                    GameOver();
            }
        }

        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[playerIndex];

            if (input.IsPauseGame(ControllingPlayer) || gamePadDisconnected)
            {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
            else
            {
                // Update each player's input
                playerOne.HandleInput(inputManager, InputKeys.WASD, 0);
                playerTwo.HandleInput(inputManager, InputKeys.Arrows, 1);
            }
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.DodgerBlue, 0, 0);

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();
            for(int i = (int)(waveDDisplace); i <= ScreenManager.GraphicsDevice.Viewport.Height + 200; i += waves.Height/2){
               spriteBatch.Draw(waves, new Rectangle(0, i - 200, waves.Width, waves.Height/2), Color.White);
            }
            spriteBatch.End();
            waveDDisplace = (waveDDisplace + ScrollSpeed) % (waves.Height / 2);
            
            //if (waveDDisplace >= 500) waveDDisplace = -waves.Height - 500 - waveDDisplace % (waves.Height/2);
            


            // Drawing HUD stuff for now
            track.Draw(gameTime);
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, DepthStencilState.Default, null);

            DrawGameScreen(spriteBatch, gameTime);

            //FUEL_BAR_Y
            //FUEL_BAR_HEIGHT

            spriteBatch.End();


            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);
                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }

        protected void DrawGameScreen(SpriteBatch spriteBatch, GameTime gameTime)
        {
            //TODO: replace with road drawablegamecomponent draw call
            //spriteBatch.Draw(road, new Rectangle(0, 0, 1024, 768), Color.White);
            
            playerOne.Draw(spriteBatch, playerOneColor);
            playerTwo.Draw(spriteBatch, playerTwoColor);
            

            //Debug
            //spriteBatch.DrawString(gameFont, "Score: " + score.ToString(), new Vector2(25), Color.White);
            //spriteBatch.DrawString(gameFont, String.Format("Pos: {0}", playerTwo.Position2.ToString()), playerTwo.Position2, Color.White);
            

            //draw game items:
            foreach(PowerSource ps in powerSources)
            {
                ps.Draw(spriteBatch, ps.color * ps.fade);
            }

            foreach (PowerUp pup in powerUps)
            {
                pup.Draw(spriteBatch);
            }

            foreach (Obstacle obs in obstacles)
            {
                if (obs is Barricade)
                {
                    ((Barricade)obs).Draw(spriteBatch); 
                }
                else
                    obs.Draw(spriteBatch);
            }

            if (gameOverCondition)
                spriteBatch.Draw(explosion, explosionPosition, Color.White);

            Viewport viewPort = ScreenManager.GraphicsDevice.Viewport;


            
            billBoard_Y += ScrollSpeed;

            //billBoard
            spriteBatch.Draw(billBoardLeft, new Rectangle(BILLBOARD_INSET, (int)(billBoard_Y), billBoardLeft.Width, billBoardLeft.Height), Color.White);
            spriteBatch.Draw(billBoardRight, new Rectangle(ScreenManager.GraphicsDevice.Viewport.Width - billBoardRight.Width - BILLBOARD_INSET, (int)(billBoard_Y), billBoardLeft.Width, billBoardLeft.Height), Color.White);

            spriteBatch.Draw(billBoardLeft, new Rectangle(BILLBOARD_INSET, (int)(billBoard_Y) - BILLBOARD_UPPERSET_Y_INSET, billBoardLeft.Width, billBoardLeft.Height), Color.White);
            spriteBatch.Draw(billBoardRight, new Rectangle(ScreenManager.GraphicsDevice.Viewport.Width - billBoardRight.Width - BILLBOARD_INSET, (int)(billBoard_Y) - BILLBOARD_UPPERSET_Y_INSET, billBoardLeft.Width, billBoardLeft.Height), Color.White);


            //if (billBoard_Y >= billBoardLeft.Height * 5) billBoard_Y = -billBoardLeft.Height;

            //side bars
            //spriteBatch.Draw(blank, new Rectangle(FUEL_BAR_INSET, viewPort.Height - (int)(FUEL_BAR_Y * playerOne.getFuelPercent()), FUEL_BAR_HEIGHT, (int)(FUEL_BAR_WIDTH* playerOne.getFuelPercent() )), Color.DarkRed);
            //spriteBatch.Draw(blank, new Rectangle(viewPort.Width - FUEL_BAR_HEIGHT - FUEL_BAR_INSET, viewPort.Height - (int)(FUEL_BAR_Y * playerTwo.getFuelPercent()), FUEL_BAR_HEIGHT, (int)(FUEL_BAR_WIDTH * playerTwo.getFuelPercent() )), Color.Yellow);
            //spriteBatch.Draw(gasBarL, new Rectangle(FUEL_BAR_INSET, FUEL_BAR_Y, FUEL_BAR_HEIGHT, FUEL_BAR_WIDTH), Color.White);
            //spriteBatch.Draw(gasBarR, new Rectangle(viewPort.Width - FUEL_BAR_HEIGHT - FUEL_BAR_INSET, FUEL_BAR_Y, FUEL_BAR_HEIGHT, FUEL_BAR_WIDTH), Color.White);


            //top bars
            spriteBatch.Draw(blank, new Rectangle(ScreenManager.GraphicsDevice.Viewport.Width / 2 - (int)(FUEL_BAR_HEIGHT * playerOne.getFuelPercent()), FUEL_BAR_WIDTH, (int)(FUEL_BAR_HEIGHT* playerOne.getFuelPercent()), FUEL_BAR_WIDTH), playerOneColor);
            spriteBatch.Draw(blank, new Rectangle(ScreenManager.GraphicsDevice.Viewport.Width / 2, FUEL_BAR_WIDTH, (int)(FUEL_BAR_HEIGHT* playerTwo.getFuelPercent()), FUEL_BAR_WIDTH), playerTwoColor);

            flashTimerP1 -= gameTime.ElapsedGameTime;
            Color c;
            c = Color.White;
            if (flashTimerP1 <= TimeSpan.FromSeconds(0)){
                flashTimerP1 = TimeSpan.FromMilliseconds(flashSpeed * playerOne.getFuelPercent());
                c = Color.Red;
            }

            spriteBatch.Draw(gasBarLH, new Rectangle(ScreenManager.GraphicsDevice.Viewport.Width / 2 - (FUEL_BAR_HEIGHT + FUEL_BAR_INC), FUEL_BAR_WIDTH, FUEL_BAR_HEIGHT + FUEL_BAR_INC, FUEL_BAR_WIDTH + 5), c);

            flashTimerP2 -= gameTime.ElapsedGameTime;
            c = Color.White;
            if (flashTimerP2 <= TimeSpan.FromSeconds(0))
            {
                flashTimerP2 = TimeSpan.FromMilliseconds(flashSpeed * playerTwo.getFuelPercent());
                c = Color.Red;
            }

            spriteBatch.Draw(gasBarRH, new Rectangle(ScreenManager.GraphicsDevice.Viewport.Width / 2, FUEL_BAR_WIDTH, FUEL_BAR_HEIGHT + FUEL_BAR_INC, FUEL_BAR_WIDTH + 5), c);

            spriteBatch.DrawString(gameFont, score.ToString() + "m", new Vector2(25), Color.White);
            //draw helper
            if (!gameStarted)
            {
                spriteBatch.DrawString(gameFont, "Press up to start!", new Vector2((ScreenManager.GraphicsDevice.Viewport.Width / 2) - (int)(gameFont.MeasureString("Press up to start!").X / 2), ScreenManager.GraphicsDevice.Viewport.Height - (int)(gameFont.MeasureString("Press up to start!").Y)), Color.White);
            }

        }

        #endregion

        private void FuelUp(PlayerCar p, PowerSource ps)
        {
            if (ps.createdBy != p)
            {
                p.AddFuel();
                //SoundManager.playSound(collectPowerInstance, 0.1f);
                ps.expired = true;
            }
        }

        private void CheckForCollisions()
        {
            //check for edge of screen detection
            playerOne.CheckCollisionWithEdgeOfScreen();
            playerTwo.CheckCollisionWithEdgeOfScreen();

            if (playerOne.boundingRect.Intersects(playerTwo.boundingRect))
            {
                //player collision!
                SoundManager.playSound(collideInstance, 0.1f);
                SoundManager.playSound(doubleHonkInstance, 0.1f);
                Vector2 difference = playerTwo.Position2 - playerOne.Position2;
                playerTwo.Position2 += difference / 2;
                playerOne.Position2 -= difference / 2;
                playerOne.Velocity *= -1;
                playerTwo.Velocity *= -1;
            }

            Track.TrackAreaType player1Area = track.GetAreaAtPosition(new Vector2(playerOne.boundingRect.Center.X, playerOne.boundingRect.Center.Y));
            Track.TrackAreaType player2Area = track.GetAreaAtPosition(new Vector2(playerTwo.boundingRect.Center.X, playerTwo.boundingRect.Center.Y));
            if (player1Area == Track.TrackAreaType.None || player2Area == Track.TrackAreaType.None)
            {
                GameOver();
            }
            else if (player1Area == Track.TrackAreaType.Grass || player2Area == Track.TrackAreaType.Grass)
                ScrollSpeed = GrassScrollSpeed;
            else if (!playerOne.boosting && !playerTwo.boosting && (playerOne.started || playerTwo.started))
                ScrollSpeed = DefaultScrollSpeed;

            //check for collisions between the players and powersources
            foreach (PowerSource p in powerSources)
            {
                if (p.createdBy != playerOne && playerOne.boundingRect.Intersects(p.boundingRect))
                {
                    FuelUp(playerOne, p);
                }
                if (p.createdBy != playerTwo && playerTwo.boundingRect.Intersects(p.boundingRect))
                {
                    FuelUp(playerTwo, p);
                }
            }
            foreach (PowerUp pup in powerUps)
            {
                // Either player powerups
                if (playerOne.boundingRect.Intersects(pup.boundingRect) || playerTwo.boundingRect.Intersects(pup.boundingRect))
                {
                    if (pup.currentType == PowerUp.PowerUpType.PositionSwap)
                    {
                        Vector2 tmp = playerOne.Position2;
                        playerOne.Position2 = playerTwo.Position2;
                        playerTwo.Position2 = tmp;
                        pup.expired = true;
                    }
                    
                    // Single player powerups
                    if (playerOne.boundingRect.Intersects(pup.boundingRect))
                    {
                        if (pup.currentType == PowerUp.PowerUpType.SpeedBoost)
                        {
                            playerOne.boosting = true;
                            pup.expired = true;
                        }
                    }
                    if (playerTwo.boundingRect.Intersects(pup.boundingRect))
                    {
                        if (pup.currentType == PowerUp.PowerUpType.SpeedBoost)
                        {
                            playerTwo.boosting = true;
                            pup.expired = true;
                        }
                    }
                }
            }

            bool boosting = playerOne.boosting || playerTwo.boosting;

            foreach (Obstacle obs in obstacles)
            {
                if (obs is Barricade)
                {
                    if (((Barricade)obs).CheckCollision(playerOne.boundingRect))
                    {
                        obs.expired = true;
                        if (!boosting)
                        {
                            gameOverCondition = true;
                            explosionPosition = playerOne.Position2;
                            SoundManager.playSound(ScreenManager.crashInstance, 0.6f);
                        }
                    }
                    if (((Barricade)obs).CheckCollision(playerTwo.boundingRect))
                    {
                        obs.expired = true;
                        if (!boosting)
                        {
                            gameOverCondition = true;
                            explosionPosition = playerTwo.Position2;
                            SoundManager.playSound(ScreenManager.crashInstance, 0.6f);
                        }
                    }
                }
                if (playerOne.boundingRect.Intersects(obs.boundingRect))
                {
                    obs.expired = true;
                    if (!boosting)
                    {
                        gameOverCondition = true;
                        explosionPosition = playerOne.Position2;
                        SoundManager.playSound(ScreenManager.crashInstance, 0.6f);
                    }
                }
                if (playerTwo.boundingRect.Intersects(obs.boundingRect))
                {
                    obs.expired = true;
                    if (!boosting)
                    {
                        gameOverCondition = true;
                        explosionPosition = playerTwo.Position2;
                        SoundManager.playSound(ScreenManager.crashInstance, 0.6f);
                    }
                }
            }
        }

        private void GameOver()
        {
            this.ScrollSpeed = 0;
            playerOne.ignoreInput = true;
            playerTwo.ignoreInput = true;
            powerUps.Clear();
            powerSources.Clear();
            obstacles.Clear();
            if (playerOne.engineInstance != null)
                SoundManager.stopSound(playerOne.engineInstance);
            if (playerTwo.engineInstance != null)
                SoundManager.stopSound(playerTwo.engineInstance);

            this.ExitScreen();

            MediaPlayer.Stop();

            GamePad.SetVibration(PlayerIndex.One, 0, 0);
            GamePad.SetVibration(PlayerIndex.Two, 0, 0);

            ExitScreen();
            ScreenManager.AddScreen(new BackgroundScreen(), null);
            ScreenManager.AddScreen(new GameOverScreen(score), null);
//             LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(),
//                                                            new HighScoreScreen(ScreenManager.Game),
//                                                            new HighScoreEntryScreen(score),
//                                                            new GameOverScreen(score));
        }
    }
}
