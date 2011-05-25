using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Threading;
using Microsoft.Xna.Framework.Storage;
using System.IO;
using System.Diagnostics;
using Microsoft.Xna.Framework.Input;

namespace CastleX
{

    class BackgroundDemoScreen : GameScreen
    {
        #region Fields
        // Resources for drawing.
        //private GraphicsDeviceManager graphics;
        //private SpriteBatch spriteBatch;
        ContentManager content;

        public ContentManager backgroundContent
        {
            get { return content; }
            set { content = value; }
        }
        // Global content.
        private SpriteFont hudFont;

        float deathstatustimer = 0;
        float endstatustimer = 0;

        // Meta-level game state.
        private int levelIndex = -1;
        public Level level;
        private const int StartingLives = 3;

        SpriteFont gameFont;
        bool ispaused, introisup;

        public bool IntroIsUp
        {
            get { return introisup; }
            set { introisup = value; }
        }
        public bool IsPaused
        {
            get { return ispaused; }
            set { ispaused = value; }
        }

        TimeSpan TargetElapsedTime;

        // When the time remaining is less than the warning time, it blinks on the hud
        private static readonly TimeSpan WarningTime = TimeSpan.FromSeconds(30);

        private const int TargetFrameRate = 30;
        private const int BackBufferWidth = 240;
        private const int BackBufferHeight = 320;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public BackgroundDemoScreen(ScreenManager screenManager)
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            // Framerate differs between platforms.
            TargetElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / TargetFrameRate);
            screenManager.IsDemo = true;
        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "GameContent");

            gameFont = content.Load<SpriteFont>(@"Fonts/Hud");
            // Create a new SpriteBatch, which can be used to draw textures.
            //spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load fonts
            hudFont = content.Load<SpriteFont>("Fonts/Hud");
            LoadNextLevel();

            introisup = true;
            // A real game would probably have more content than this sample, so
            // it would take longer to load. We simulate that by delaying for a
            // while, giving you a chance to admire the beautiful loading screen.
            Thread.Sleep(1000);

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();
        }


        private void LoadNextLevel()
        {
            // Find the path of the next level.
            string levelPath = "";



            // Loop here so we can try again when we can't find a level.
            while (true)
            {
                levelPath = String.Format("demoscreen.txt", ++levelIndex);
                levelPath = Path.Combine(StorageContainer.TitleLocation, "GameContent/" + levelPath);

                if (File.Exists(levelPath))
                    break;

                // If there isn't even a level 0, something has gone wrong.
                if (levelIndex == 0)
                    throw new Exception("No text file named \"demoscreen.txt\" found.\n\nMake a level to use as the demo, and place it under \"GameContent/demoscreen.txt\"");

                // Whenever we can't find a level, start over again at 0.
                levelIndex = -1;
            }

            // Unloads the content for the current level before loading the next one.
            if (level != null)
                level.Dispose();

            // Load the level.

            level = new Level(ScreenManager.Game.Services, levelPath, 100, 100, levelIndex, 1, ScreenManager);

        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
            Trace.Write("");
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
                  
            level.Update(gameTime);
            base.Update(gameTime, false, false);
            if (!level.Player.IsAlive)
            {
                deathstatustimer += 1;

                if (deathstatustimer > 50)
                {
                    LoadNextLevel();
                    deathstatustimer = 0;
                }
            }
            if (level.ReachedExit)
            {
                endstatustimer += 1;

                if (endstatustimer > 50)
                {
                    EndDemo();
                    endstatustimer = 0;
                }
            }
            if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.A) || 
                GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Back) || 
                GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadUp) || 
                GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadDown) || 
                GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadLeft) || 
                GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadRight) || 
                Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Space) ||
                Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Enter) ||
                Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Back) ||
                Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Escape) || 
                Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Up) ||
                Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Down) || 
                Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Left) ||
                Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Right))
            {
                EndDemo();
            }

        }

        void EndDemo()
        {
            LoadingScreen.Load(ScreenManager, true, new BackgroundScreen(), new MainMenuScreen(ScreenManager));
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.Black, 0, 0);

            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);

            //SpriteBatch.Begin();

            level.Draw(gameTime, spriteBatch);

            DrawHud(spriteBatch);

            //SpriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
        }

        private void DrawHud(SpriteBatch spriteBatch)
        {
            //spriteBatch.Begin();

            Rectangle titleSafeArea = ScreenManager.GraphicsDevice.Viewport.TitleSafeArea;
            Vector2 hudLocation = new Vector2(titleSafeArea.X, titleSafeArea.Y);
            Vector2 center = new Vector2(titleSafeArea.X + titleSafeArea.Width / 2.0f,
                                         titleSafeArea.Y + titleSafeArea.Height / 2.0f);

            // Draw time remaining. Uses modulo division to cause blinking when the
            // player is running out of time.
            // Determine the status overlay message to show.
            Texture2D status = null;


            if (status != null)
            {
                // Draw status message.
                Vector2 statusSize = new Vector2(status.Width, status.Height);
                spriteBatch.Draw(status, center - statusSize / 2, Color.White);
            }

            SpriteFont continuefont = ScreenManager.HudSmall;
            string continuemessage = "Press Any Key To\nReturn To Main Menu";
            Vector2 continuemeasure = continuefont.MeasureString(continuemessage);

           ScreenManager.DrawShadowedString(spriteBatch, continuefont, continuemessage, new Vector2(center.X - continuemeasure.X / 2, center.Y + continuemeasure.Y / 8 * 7), Color.White);
            
            
            //spriteBatch.End();
        }
        #endregion
    }
}
