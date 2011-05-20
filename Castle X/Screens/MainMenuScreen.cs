#region File Description
//-----------------------------------------------------------------------------
// MainMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Reflection;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using Microsoft.Xna.Framework.Input;
#endregion

namespace CastleX
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class MainMenuScreen : MenuScreen
    {
        #region Initialization
        bool isinit = false;

        TimeSpan IdleTimer;
        TimeSpan TargetElapsedTime;

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen(ScreenManager screen)
            : base("Main Menu", 1)
        {
            makeMenu();
        }
        BackgroundDemoScreen backgroundDemoScreen;

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen(ScreenManager screen, BackgroundDemoScreen backgrounddemoscreen)
            : base("Main Menu", 1)
        {
            if (!isinit)
            {
                backgroundDemoScreen = backgrounddemoscreen;
                isinit = true;
            }
            TargetElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / 30);
        

            makeMenu();
        }
        void makeMenu()
        {

            // Create our menu entries.
            MenuEntry playGameMenuEntry = new MenuEntry("Play Game");
            playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
            MenuEntries.Add(playGameMenuEntry);

            // Create our menu entries.
            MenuEntry playCustomLevelsMenuEntry = new MenuEntry("Play Custom Levels");
            playCustomLevelsMenuEntry.Selected += PlayCustomLevelsMenuEntrySelected;
            //MenuEntries.Add(playCustomLevelsMenuEntry);

            MenuEntry musicMenuEntry = new MenuEntry("Change Music");
            musicMenuEntry.Selected += musicMenuEntrySelected;

            MenuEntry levelSelectMenuEntry = new MenuEntry("Level Select");
            levelSelectMenuEntry.Selected += levelSelectMenuEntrySelected;
            MenuEntries.Add(levelSelectMenuEntry);

            //MenuEntry levelEditorMenuEntry = new MenuEntry("Level Editor");
            //levelEditorMenuEntry.Selected += levelEditorMenuEntrySelected;
            //MenuEntries.Add(levelEditorMenuEntry);

            MenuEntry optionsMenuEntry = new MenuEntry("Options");
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;
            MenuEntries.Add(optionsMenuEntry);

            MenuEntry exitMenuEntry = new MenuEntry("Exit");
            exitMenuEntry.Selected += OnCancel;
            MenuEntries.Add(exitMenuEntry);
        }


        #endregion

        #region Handle Input

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            int seconds = (int)Math.Round(gameTime.ElapsedGameTime.TotalSeconds * 100.0f);
            seconds = Math.Min(seconds, (int)Math.Ceiling(gameTime.ElapsedGameTime.TotalSeconds));
            IdleTimer += TimeSpan.FromSeconds(seconds);

            if (isAnyButtonPressed)
                IdleTimer = TimeSpan.Zero;

            if (IdleTimer > TimeSpan.FromSeconds(500))
            {
                traceIdleTimer();
                LoadingScreen.Load(ScreenManager, true, new BackgroundDemoScreen(ScreenManager));
            }
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        private void traceIdleTimer()
        {
            Trace.Write("IdleTimer: " + IdleTimer.ToString() + "\n");
        }

        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void PlayGameMenuEntrySelected(object sender, EventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, new GameplayScreen(ScreenManager, false));
        }

        /// <summary>
        /// Event handler for when the Play Created Levels menu entry is selected.
        /// </summary>
        void PlayCustomLevelsMenuEntrySelected(object sender, EventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, new GameplayScreen(ScreenManager, true));
        }

        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        void musicMenuEntrySelected(object sender, EventArgs e)
        {
#if WINDOWS
                const string message = "Not available in the pc version.";

                MessageBoxScreen notAvailableInPCMessageBox = new MessageBoxScreen(message, false, false);

                ScreenManager.AddScreen(notAvailableInPCMessageBox);
#else
            if (ScreenManager.Settings.InGameMusic)
            {
                const string message = "To use this, the \"In Game Music\" option must be disabled. Do you want to temporarily disable it? (you can save it by going to the options screen)";

                MessageBoxScreen disableInGameMusicMessageBox = new MessageBoxScreen(message, true, false);
                disableInGameMusicMessageBox.Accepted += disableInGameMusicAccepted;

                ScreenManager.AddScreen(disableInGameMusicMessageBox);
            }
            else
            {
                if (!Guide.IsVisible)
                    Guide.Show();
            }
#endif
        }
        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        void disableInGameMusicAccepted(object sender, EventArgs e)
        {
            if (ScreenManager.Settings.InGameMusic)
                ScreenManager.Settings.InGameMusic = false;
            MediaPlayer.Stop();
        }


        void levelEditorMenuEntrySelected(object sender, EventArgs e)
        {

            LoadingScreen.Load(ScreenManager, true, new LevelEditorScreen(ScreenManager));
        }

        void levelSelectMenuEntrySelected(object sender, EventArgs e)
        {

            ScreenManager.AddScreen(new LevelSelectScreen(ScreenManager));
        }


        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        void OptionsMenuEntrySelected(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new OptionsMenuScreen(ScreenManager, false, backgroundDemoScreen));
        }


        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the game.
        /// </summary>
        protected override void OnCancel()
        {
            const string message = "Are you sure you want to leave Castle X?";

            MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message, true, false);

            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmExitMessageBox);
        }

        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        void ConfirmExitMessageBoxAccepted(object sender, EventArgs e)
        {
            ScreenManager.Game.Exit();
        }



        #endregion
    }
}
