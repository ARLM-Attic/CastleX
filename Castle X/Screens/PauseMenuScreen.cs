#region File Description
//-----------------------------------------------------------------------------
// PauseMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace CastleX
{
    /// <summary>
    /// The pause menu comes up over the top of the game,
    /// giving the player options to resume or quit.
    /// </summary>
    class PauseMenuScreen : MenuScreen
    {
        #region Initialization
        bool isinit = false;
        GameplayScreen gamePlayScreen;

        /// <summary>
        /// Constructor.
        /// </summary>
        public PauseMenuScreen(GameplayScreen mygameplayscreen)
            : base("Paused")
        {
            if (!isinit)
            {
                gamePlayScreen = mygameplayscreen;
                isinit = true;
            }
            // Flag that there is no need for the game to transition
            // off when the pause menu is on top of it.
            IsPopup = true;

            // Create our menu entries.
            MenuEntry resumeGameMenuEntry = new MenuEntry("Resume Game");
            resumeGameMenuEntry.Selected += OnCancel;
            MenuEntries.Add(resumeGameMenuEntry);

            MenuEntry levelJumpMenuEntry = new MenuEntry("Level Jump");
            levelJumpMenuEntry.Selected += levelJumpMenuEntrySelected;
            MenuEntries.Add(levelJumpMenuEntry);

            MenuEntry levelSelectMenuEntry = new MenuEntry("Level Select");
            levelSelectMenuEntry.Selected += levelSelectMenuEntrySelected;
            MenuEntries.Add(levelSelectMenuEntry);

            MenuEntry musicMenuEntry = new MenuEntry("Change Music");
            musicMenuEntry.Selected += musicMenuEntrySelected;
#if WINDOWS
#else
            MenuEntries.Add(musicMenuEntry);
#endif

            MenuEntry optionsMenuEntry = new MenuEntry("Options");
            optionsMenuEntry.Selected += optionsMenuEntrySelected;
            MenuEntries.Add(optionsMenuEntry);

            MenuEntry quitGameMenuEntry = new MenuEntry("Quit Game");
            quitGameMenuEntry.Selected += QuitGameMenuEntrySelected;
            MenuEntries.Add(quitGameMenuEntry);
            

        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Event handler for when the Quit Game menu entry is selected.
        /// </summary>
        void QuitGameMenuEntrySelected(object sender, EventArgs e)
        {
            const string message = "Are you sure you want to return to the menu?";

            MessageBoxScreen confirmQuitMessageBox = new MessageBoxScreen(message, true, true);

            confirmQuitMessageBox.Accepted += ConfirmQuitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmQuitMessageBox);
        }

        /// <summary>
        /// Event handler for when the Quit Game menu entry is selected.
        /// </summary>
        void optionsMenuEntrySelected(object sender, EventArgs e)
        {
        SwitchScreen(this, new OptionsMenuScreen(ScreenManager, false, gamePlayScreen));
        }

        void levelSelectMenuEntrySelected(object sender, EventArgs e)
        {

            ScreenManager.AddScreen(new LevelSelectScreen(ScreenManager));
        }


        void levelJumpMenuEntrySelected(object sender, EventArgs e)
        {
            gamePlayScreen.LoadLevel(gamePlayScreen.LevelIndex+1, 1, gamePlayScreen.MyLevel.Player.Lives, gamePlayScreen.MyLevel.Score);
            OnCancel();
            gamePlayScreen.IsPaused = false;
        }

        /// <summary>
        /// Event handler for when the Cancel Game menu entry is selected.
        /// </summary>
        void cancelMenuEntrySelected(object sender, EventArgs e)
        {
            OnCancel();
            gamePlayScreen.IsPaused = false;
        }

        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to quit" message box. This uses the loading screen to
        /// transition from the game back to the main menu screen.
        /// </summary>
        void ConfirmQuitMessageBoxAccepted(object sender, EventArgs e)
        {
                BackgroundScreen backgroundScreen = new BackgroundScreen();
                LoadingScreen.Load(ScreenManager, true, backgroundScreen,
                                                         new MainMenuScreen(ScreenManager));
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

        

        #endregion

        #region Draw


        /// <summary>
        /// Draws the pause menu screen. This darkens down the gameplay screen
        /// that is underneath us, and then chains to the base MenuScreen.Draw.
        /// </summary>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            ScreenManager.FadeBackBufferToBlack(spriteBatch, TransitionAlpha * 2 / 3);

            base.Draw(gameTime, spriteBatch);
        }


        #endregion
    }
}
