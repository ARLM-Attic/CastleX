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
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.GamerServices;
using System.IO;
using Microsoft.Xna.Framework.Storage;
using System.Diagnostics;
#endregion

namespace CastleX
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    class GameplayScreen : GameScreen
    {
        #region Fields

        // Resources for drawing.
        //private GraphicsDeviceManager graphics;
        //private SpriteBatch spriteBatch;
        ContentManager content;
        // Global content.

        private Texture2D winOverlay;
        private Texture2D loseOverlay;
        private Texture2D diedOverlay;

        // Meta-level game state.
        private int levelIndex = 0;
        public int LevelIndex
        {
            get { return levelIndex; }
            set { levelIndex = value; }
        }
        private Level level;
        private bool wasContinuePressed;
        private const int StartingLives = 3;

        bool ispaused, introisup, leveljumpunlocked = false, nointroset = false;

        public bool IntroIsUp  // Used to pause the game while a intro screen is up 
        {
            get { return introisup; }
            set { introisup = value; }
        }
        public bool IsPaused
        {
            get { return ispaused; }
            set { ispaused = value; }
        }

        public Level MyLevel
        {

            get { return myLevel; }
            set { myLevel = value; }
        }

        Level myLevel;
        bool playSavedLevels = false;
        bool introloaded = false;


        TimeSpan TargetElapsedTime;

        // When the time remaining is less than the warning time, it blinks on the hud
        private static readonly TimeSpan WarningTime = TimeSpan.FromSeconds(30);

        private int TargetFrameRate = 30;
        private Buttons ContinueButton = Buttons.B;


        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="playSavedLevels">If true, the game will load the levels that were built using the in game level editor. if false, the game will load the original levels.</param>
        public GameplayScreen(ScreenManager screenManager, bool playSavedLevels)
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            TargetElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / TargetFrameRate);
            //levelIndex = -1;
            screenManager.IsDemo = false;
            this.playSavedLevels = playSavedLevels;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="loadLevelNumber">The number of a certain level to load.</param>
        /// <param name="playSavedLevels">If true, the game will load the levels that were built using the in game level editor. if false, the game will load the original levels.</param>
        public GameplayScreen(int loadLevelNumber, ScreenManager screenManager, bool playSavedLevels)
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            // Framerate differs between platforms.
            TargetElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / TargetFrameRate);
            levelIndex = loadLevelNumber+1;
            screenManager.IsDemo = false;
            this.playSavedLevels = playSavedLevels;
        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {

            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "GameContent");
            // Create a new SpriteBatch, which can be used to draw textures.
            //spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load fonts

            // Load overlay textures
            winOverlay = ScreenManager.You_WinTexture;
            loseOverlay = ScreenManager.You_LoseTexture;
            diedOverlay = ScreenManager.You_DiedTexture;

            LoadLevel(levelIndex, 1, StartingLives, 0);
            introisup = true;
            introloaded = false;
            // A real game would probably have more content than this sample, so
            // it would take longer to load. We simulate that by delaying for a
            // while, giving you a chance to admire the beautiful loading screen.
            //Thread.Sleep(1000);

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();
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
            if (!level.MetaDescriptionIsSet && !level.MetaTitleIsSet && !nointroset)
            {
                introisup = false;
                introloaded = true;
                nointroset = true;
            }
            if (!ScreenManager.Settings.ShowLevelIntro && !nointroset)
            {
                introisup = false;
                introloaded = true;
                nointroset = true;
            }
            //if (ScreenManager.Settings.ShowLevelIntro)
            //{
            //    if (introisup)
            //    {
            //        if (!introloaded)
            //        {
            //            string mymessage = "  Level Title:\n" + level.LevelTitle + "\n  Level Description:\n" + level.LevelDescription;
            //            ScreenManager.AddScreen(new MessageBoxScreen(mymessage, false, this, true));
            //            introloaded = true;
            //        }
            //    }
            //}
            if (level.Score > 8000 && !ScreenManager.Settings.LevelJumpUnlocked && !leveljumpunlocked)
            {
                if (!ScreenManager.Settings.DebugMode)
                ScreenManager.Settings.LevelJumpUnlocked = true;
                leveljumpunlocked = true;
                string leveljumpmessage = "Congratulations, you have unlocked level jumping! Now, you can press A and B to go to the next level! (go to your options to enable it)";
                ScreenManager.AddScreen(new MessageBoxScreen(leveljumpmessage, false, true));
            }

            if (!ispaused && IsActive && !introisup)
            {
                    level.Update(gameTime);
            }

            if (!ScreenManager.Game.IsActive && !introisup && introloaded)
            {
                if (!ispaused)
                {
                    ScreenManager.AddScreen(new PauseMenuScreen(this));
                    ispaused = true;
                }

            }
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");
           
            if (input.PauseGame)
            {
                // If they pressed pause, bring up the pause menu screen.
                ScreenManager.AddScreen(new PauseMenuScreen(this));
                if (!ispaused)
                    ispaused = true;
            }
            else
            {
                if (ispaused)
                    ispaused = false;

                // Otherwise move the player position.
                KeyboardState keyboardState = Keyboard.GetState();
                GamePadState gamepadState = GamePad.GetState(PlayerIndex.One);
                bool continuePressed =
                    keyboardState.IsKeyDown(Keys.Space) ||
                    gamepadState.IsButtonDown(ContinueButton);

                // Load next level without waiting for any user input
                 if (level.ReachedExit)
                     LoadLevel(level.NextLevel, level.ExitNumber, level.Player.Lives + 1, level.Score);
                 else
                // Perform the appropriate action to advance the game and
                // to get the player back to playing.
                if (!wasContinuePressed && continuePressed)
                {
                    if (!level.Player.IsAlive)
                    {
                        level.StartNewLife();
                    }
                    //else //if (level.TimeRemaining == TimeSpan.Zero)
                    //{
                    //    if (level.ReachedExit)
                    //    {
                    //        LoadLevel(level.NextLevel, level.ExitNumber, level.Player.Lives + 1, level.Score);
                    //        //if (ScreenManager.Settings.ShowLevelIntro)
                    //        //{
                    //        //    introisup = true;
                    //        //    introloaded = false;
                    //        //    nointroset = false;
                    //        //}
                    //    }
                    //    //else
                    //    //{
                    //    //    ReloadCurrentLevel(level.Player.Lives - 1);
                    //    //}
                    //}
                }
                wasContinuePressed = continuePressed;
            }
        }


        public void LoadLevel(int levelNumber, int exitNumber, int currentLives, int score)
        {
            // Find the path of the next level.
            string levelPath = "";

            //IGNORE THE ERROR, STUPID SHIT DOESNT SEE THAT IS USED WITHIN THE LEVEL LOADER!!!
            string levelsfolder = "Levels";

            if (playSavedLevels)
                levelsfolder = "SavedLevels";

            // Try to find the level to load. 
            if (ScreenManager.Settings.LoadLevelsFrom == 0)  // Load level from game folder
            {
                levelPath = String.Format(levelsfolder + "/{0}.txt", levelNumber);
                levelPath = Path.Combine(StorageContainer.TitleLocation, "GameContent/" + levelPath);
            }
            else if (ScreenManager.Settings.LoadLevelsFrom == 1)  // Load level from saved levels folder
            {
                StorageContainer mycontainer = ScreenManager.Device.OpenContainer("Castle_X");

                //Computer format
                levelPath = String.Format("{00}.txt", levelNumber);

                if (!File.Exists(Path.Combine(mycontainer.Path, "GameContent\\" + levelsfolder + "\\" + levelPath)))
                {
                    if (File.Exists(Path.Combine(StorageContainer.TitleLocation, "GameContent/" + levelsfolder + "/" + levelPath)))
                    {
                        if (!Directory.Exists(Path.Combine(StorageContainer.TitleLocation, "GameContent/")))
                        {
                            Directory.CreateDirectory(Path.Combine(StorageContainer.TitleLocation, "GameContent/" + levelsfolder + "/"));
                        }

                        if (!Directory.Exists(Path.Combine(StorageContainer.TitleLocation, "GameContent/" + levelsfolder + "")))
                        {
                            Directory.CreateDirectory(Path.Combine(StorageContainer.TitleLocation, "GameContent/" + levelsfolder + "/"));
                        }

                        File.Copy(Path.Combine(StorageContainer.TitleLocation, "GameContent/" + levelsfolder + "/" + levelPath), Path.Combine(mycontainer.Path, "GameContent\\" + levelsfolder + "\\" + levelPath));
                        Trace.Write("Copied the internal storage version of level " + levelNumber + " to the container folder");
                        levelPath = Path.Combine(mycontainer.Path, "GameContent\\" + levelsfolder + "\\" + levelPath);
                        Trace.Write("Level Path: " + levelPath + "\n");
                    }
                }
                else
                {
                    levelPath = Path.Combine(mycontainer.Path, "GameContent\\" + levelsfolder + "\\" + levelPath);
                    Trace.Write("Level Path: " + levelPath + "\n");
                }
                mycontainer.Dispose();
            }
            else if (ScreenManager.Settings.LoadLevelsFrom == 2)  // Load level from folder informed by user
            {

                //Computer format
                levelPath = String.Format("{00}.txt", levelNumber);

                if (!File.Exists(ScreenManager.Settings.LevelFolderPath + "\\" + levelPath))
                {
                    if (File.Exists(Path.Combine(StorageContainer.TitleLocation, "GameContent/" + levelsfolder + "/" + levelPath)))
                    {

                        File.Copy(Path.Combine(StorageContainer.TitleLocation, "GameContent/" + levelsfolder + "/" + levelPath), ScreenManager.Settings.LevelFolderPath + "\\" + levelPath);
                        Trace.Write("Copied the internal storage version of level " + levelNumber + " to the container folder");
                        levelPath = ScreenManager.Settings.LevelFolderPath + "\\" + levelPath;
                        Trace.Write("Level Path: " + levelPath + "\n");
                    }
                }
                else
                {
                    levelPath = ScreenManager.Settings.LevelFolderPath + "\\" + levelPath;
                    Trace.Write("Level Path: " + levelPath + "\n");
                }
            }

            if (!File.Exists(levelPath))
                LoadingScreen.Load(ScreenManager, true, new BackgroundScreen(), new MainMenuScreen(ScreenManager), new MessageBoxScreen("Level " + levelNumber.ToString() +  " not found!", false, false));
            
            // Unloads the content for the current level before loading the next one.
            if (level != null)
                level.Dispose();

            // Load the level.
            level = new Level(ScreenManager.Game.Services, levelPath, currentLives, score, levelIndex, exitNumber, ScreenManager);
            myLevel = level;
        }

        private void ReloadCurrentLevel(int currentLives)
        {
            --currentLives;
            LoadLevel(levelIndex, 1, currentLives, level.Score);
        }

        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.Black, 0, 0);

            level.Draw(gameTime, spriteBatch);
            DrawHud(spriteBatch);

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0)
                ScreenManager.FadeBackBufferToBlack(spriteBatch, 255 - TransitionAlpha);
        }

        private void DrawHud(SpriteBatch spriteBatch)
        {
            Rectangle titleSafeArea = ScreenManager.GraphicsDevice.Viewport.TitleSafeArea;
            Vector2 hudLocation = new Vector2(titleSafeArea.X, titleSafeArea.Y);
            Vector2 center = new Vector2(titleSafeArea.X + titleSafeArea.Width / 2.0f,
                                         titleSafeArea.Y + titleSafeArea.Height / 2.0f);
            float fontsize = ScreenManager.Font.MeasureString("Test").Y / 2;

            // Draw time remaining. Uses modulo division to cause blinking when the
            // player is running out of time.
            //string timeString = "Time: " + level.TimeRemaining.Minutes.ToString("00") + ":" + level.TimeRemaining.Seconds.ToString("00");
            //Color timeColor;
            //if (level.TimeRemaining > WarningTime ||
            //    level.ReachedExit ||
            //    (int)level.TimeRemaining.TotalSeconds % 2 == 0)
            //{
            //    timeColor = Color.Yellow;
            //}
            //else
            //{
            //    timeColor = Color.Red;
            //}
            //ScreenManager.DrawShadowedString(spriteBatch, ScreenManager.HudMedium, timeString, hudLocation + new Vector2(0.0f, ScreenManager.ViewPort.Height - (fontsize * 2.4f)), timeColor);
            float rightcolumn = ScreenManager.GraphicsDevice.Viewport.Width / 2 * 1;
            // Draw score
            //ScreenManager.DrawShadowedString(spriteBatch, ScreenManager.Font, "Score: " + level.Score.ToString(), hudLocation + new Vector2(0.0f, 0.0f), Color.Yellow);
            // Draw Level and Lives on the right side
            //ScreenManager.DrawShadowedString(spriteBatch, ScreenManager.Font, "Level: " + levelIndex, hudLocation + new Vector2(rightcolumn, 0.0f), Color.Yellow);
            //ScreenManager.DrawShadowedString(spriteBatch, ScreenManager.Font, "Lives: " + level.Player.Lives.ToString(), hudLocation + new Vector2(rightcolumn, fontsize * 1.2f), Color.Yellow);
            //ScreenManager.DrawShadowedString(spriteBatch, ScreenManager.Font, "Score: " + level.Score.ToString(), hudLocation + new Vector2(0.0f, 0.0f), Color.Yellow);
            ScreenManager.DrawShadowedString(spriteBatch, ScreenManager.Font, "x " + level.Score.ToString(), new Vector2(430, 50), Color.Yellow);
            ScreenManager.DrawShadowedString(spriteBatch, ScreenManager.Font, "x " + level.Player.Lives.ToString(), new Vector2(430, 10), Color.Yellow);
            ScreenManager.DrawShadowedString(spriteBatch, ScreenManager.Font, "+ " + level.Player.CurrentHealth.ToString(), new Vector2(430, 30), Color.Yellow);

            //  Number of each Key 
            ScreenManager.DrawShadowedString(spriteBatch, ScreenManager.Font, "x " + level.Player.YellowKeys.ToString(), new Vector2(350, 10), Color.Yellow);
            ScreenManager.DrawShadowedString(spriteBatch, ScreenManager.Font, "x " + level.Player.GreenKeys.ToString(), new Vector2(350, 30), Color.Yellow);
            ScreenManager.DrawShadowedString(spriteBatch, ScreenManager.Font, "x " + level.Player.RedKeys.ToString(), new Vector2(350, 50), Color.Yellow);

            //  Draw map
            if(level.Player.hasMap)
                spriteBatch.Draw(ScreenManager.HUDOpenMapTexture, new Vector2(465, 10), Color.White);
            else
                spriteBatch.Draw(ScreenManager.HUDClosedMapTexture, new Vector2(480, 10), Color.White);


            if (level.CoinsRemaining > 0)
            {
               ScreenManager.DrawShadowedString(spriteBatch, ScreenManager.Font, "Coins Left: " + level.CoinsRemaining, hudLocation + new Vector2(0.0f, fontsize * 1.2f), Color.Yellow);
            }
            // Determine the status overlay message to show.
            Texture2D status = null;
            //if (level.ReachedExit)
            //     status = winOverlay;
            //else 
            if (!level.Player.IsAlive)
            {
                if (level.Player.Lives <= 0)
                    status = loseOverlay;
                else
                    status = diedOverlay;
            }

            if (status != null)
            {
                // Draw status message.
                Vector2 statusSize = new Vector2(status.Width, status.Height);
                spriteBatch.Draw(status, center - statusSize / 2, Color.White);
            }

            Vector2 HealthPosition = new Vector2(10, 60);
            int offset = 0;
            for (int i = 1; i <= level.Player.CurrentHealth; ++i)
            {
                spriteBatch.Draw(ScreenManager.HeartIconTexture, HealthPosition, Color.White);
                HealthPosition.X += 20;
                offset = i;
            }
            HealthPosition.X -= offset * 20; 

            //spriteBatch.End();
        }
        #endregion
    }
}
