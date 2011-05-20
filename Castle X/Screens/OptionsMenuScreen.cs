#region File Description
//-----------------------------------------------------------------------------
// OptionsMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
#if WINDOWS
using System.Windows.Forms;
#endif
using System.Diagnostics;
using System.Threading;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace CastleX
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class OptionsMenuScreen : MenuScreen
    {
        #region Fields

        //MenuEntry showLevelIntroMenuEntry;
        MenuEntry inGameMusicMenuEntry;
        MenuEntry soundEffectsMenuEntry;
        MenuEntry musicVolumeMenuEntry;
        MenuEntry skinsMenuEntry;
        MenuEntry loadLevelsFromMenuEntry;
        MenuEntry levelFolderPathMenuEntry;
        MenuEntry debugModeMenuEntry;
        MenuEntry aboutMenuEntry;


        GameplayScreen gamePlayScreen;
        PauseMenuScreen pauseMenuScreen;
        BackgroundDemoScreen backgroundDemoScreen;

        public BackgroundDemoScreen optionBGDemoScreen
        {
            get { return backgroundDemoScreen; }
        }


        bool isinit = false;
        bool changedsetting = false;

        public bool isFromMenu = false;

        string[] loadLevelsFromString = { "Interior", "Container", "Custom" };
        string[] skinNames = { "Maze of Galious Skin", "Mario Skin - In Progress"};
        string skinName = "";

        ScreenManager screenManager;
        #endregion

        #region Initialization



        /// <summary>
        /// Constructor for when entering from the menu screen.
        /// </summary>
        public OptionsMenuScreen(ScreenManager screen, bool myChangedSetting)
            : base("Options")
        {
            if (!isinit)
            {
                screenManager = screen;
                isinit = true;
                changedsetting = myChangedSetting;
                isFromMenu = true;

            }

            makeMenu();

        }

        /// <summary>
        /// Constructor for when entering from the menu screen.
        /// </summary>
        public OptionsMenuScreen(ScreenManager screen, bool myChangedSetting, BackgroundDemoScreen backgrounddemoscreen)
            : base("Options")
        {
            if (!isinit)
            {
                screenManager = screen;
                isinit = true;
                changedsetting = myChangedSetting;
                isFromMenu = true;
                backgroundDemoScreen = backgrounddemoscreen;

            }

            makeMenu();

        }

        /// <summary>
        /// Constructor for when entering from the game screen.
        /// </summary>
        public OptionsMenuScreen(ScreenManager screen, bool myChangedSetting, GameplayScreen gameplayscreen)
            : base("Options")
        {
            if (!isinit)
            {
                screenManager = screen;
                isinit = true;
                changedsetting = myChangedSetting;
                isFromMenu = false;
                gamePlayScreen = gameplayscreen;
                IsPopup = true;
            }
            makeMenu();
        }

        /// <summary>
        /// Constructor for when entering from the game screen.
        /// </summary>
        public OptionsMenuScreen(ScreenManager screen, bool myChangedSetting, GameplayScreen gameplayscreen, PauseMenuScreen pausemenuscreen)
            : base("Options")
        {
            if (!isinit)
            {
                screenManager = screen;
                isinit = true;
                changedsetting = myChangedSetting;
                isFromMenu = false;
                gamePlayScreen = gameplayscreen;
                IsPopup = true;
                pauseMenuScreen = pausemenuscreen;
            }
            makeMenu();
        }

        void makeMenu()
        {
            MenuEntry errorMenuEntry = new MenuEntry("Test BSOD");
            errorMenuEntry.Selected += showError;
            //MenuEntries.Add(errorMenuEntry);

            //showLevelIntroMenuEntry = new MenuEntry(string.Empty);
            //showLevelIntroMenuEntry.Selected += showLevelIntroSelected;
            //MenuEntries.Add(showLevelIntroMenuEntry);

            aboutMenuEntry = new MenuEntry(string.Empty);
            aboutMenuEntry.Selected += aboutMenuEntrySelected;
            MenuEntries.Add(aboutMenuEntry);

            inGameMusicMenuEntry = new MenuEntry(string.Empty);
            inGameMusicMenuEntry.Selected += inGameMusicSelected;
            MenuEntries.Add(inGameMusicMenuEntry);

            musicVolumeMenuEntry = new MenuEntry(string.Empty);
            musicVolumeMenuEntry.Selected += musicVolumeSelected;
            if (screenManager.Settings.InGameMusic)
            {
                MenuEntries.Add(musicVolumeMenuEntry);
            }

            soundEffectsMenuEntry = new MenuEntry(string.Empty);
            soundEffectsMenuEntry.Selected += soundEffectsSelected;
            MenuEntries.Add(soundEffectsMenuEntry);

            skinsMenuEntry = new MenuEntry(string.Empty);
            skinsMenuEntry.Selected += skinsMenuEntrySelected;
            MenuEntries.Add(skinsMenuEntry);


            loadLevelsFromMenuEntry = new MenuEntry(string.Empty);
            loadLevelsFromMenuEntry.Selected += loadLevelsFromMenuEntrySelected;

#if WINDOWS
            MenuEntries.Add(loadLevelsFromMenuEntry);
#endif

            levelFolderPathMenuEntry = new MenuEntry(string.Empty);
            levelFolderPathMenuEntry.Selected += levelFolderPathMenuEntrySelected;
#if WINDOWS
            if (screenManager.Settings.LoadLevelsFrom == 2)
            {
                MenuEntries.Add(levelFolderPathMenuEntry);
            }
#endif

            debugModeMenuEntry = new MenuEntry(string.Empty);
            debugModeMenuEntry.Selected += debugModeMenuEntrySelected;
            MenuEntries.Add(debugModeMenuEntry);

            MenuEntry saveSettingsMenuEntry = new MenuEntry("Save Settings");
            saveSettingsMenuEntry.Selected += saveSettingsSelected;
            MenuEntries.Add(saveSettingsMenuEntry);

            MenuEntry backMenuEntry = new MenuEntry("Back");
            backMenuEntry.Selected += backSelected;
            MenuEntries.Add(backMenuEntry);

            SetMenuEntryText();
        }


        /// <summary>
        /// Fills in the latest values for the options screen menu text.
        /// </summary>
        void SetMenuEntryText()
        {
            aboutMenuEntry.Text = "About Castle X";
            skinsMenuEntry.Text = "Skin: " + screenManager.SkinSettings.skinTitle.ToString();
            inGameMusicMenuEntry.Text = "In Game Music: " + (screenManager.Settings.InGameMusic ? "On" : "Off");
            musicVolumeMenuEntry.Text = "Music Volume: " + screenManager.Settings.MusicVolumeString;
            soundEffectsMenuEntry.Text = "Sound Volume: " + screenManager.Settings.SoundVolumeString;
            loadLevelsFromMenuEntry.Text = "Load Levels From: " + loadLevelsFromString[screenManager.Settings.LoadLevelsFrom];
            levelFolderPathMenuEntry.Text = "Level Folder Path: " + screenManager.Settings.LevelFolderPath;
            debugModeMenuEntry.Text = "Debug mode (FPS, bounding boxes, etc.): " + (screenManager.Settings.DebugMode ? "Enabled" : "Disabled");
            //showLevelIntroMenuEntry.Text = "Level Intros: " + (screenManager.Settings.ShowLevelIntro ? "Enabled" : "Disabled");
        }


        #endregion

        #region Handle Input

        #region show error
        /// <summary>
        /// Event handler for when the In Game Music menu entry is selected.
        /// </summary>
        void showError(object sender, EventArgs e)
        {

            throw new Exception("zomg");
        }
        #endregion

        //#region show level intro toggle
        //void showLevelIntroSelected(object sender, EventArgs e)
        //{
        //    screenManager.Settings.ShowLevelIntro = !screenManager.Settings.ShowLevelIntro;
        
        //    if (!changedsetting)
        //        changedsetting = true;
        //    SetMenuEntryText();

        //}
        //#endregion


        #region "about" menu selected
        void aboutMenuEntrySelected(object sender, EventArgs e)
        {
            SetMenuEntryText();
            string aboutMessage = @"Castle X is an open-source game, created in my spare time and released under the GNU licensing model, which means that you are free to copy and modify, as long as you mantain the credits
---------------------------
Credits
---------------------------
Programming:  Alexandre Lobao - aslobao@hotmail.com - http://www.AlexandreLobao.com
--------------------------- 
Icon:    
   * Carl Johan Rehbinder - Rehbinder MultiArt Productions - cjr@telia.com - http://www.multiart.nu/cci
--------------------------- 
Sounds and Graphics: 
   * Remake of Maze of Galious from Brain Games - http://www.braingames.getput.com/mog/
   * sample games from Click Team's Multimedia Fusion - http://www.clickteam.com 
   * other collected from internet - if your name should be here, just tell me!
--------------------------- 
Code based on: 
   * Platformer Expanded, expansions to XNA Platformer Starter Kit by LordKtulu - http://forums.create.msdn.com/forums/t/34901.aspx, https://users.create.msdn.com/Profile/LordKtulu
";
            ScreenManager.AddScreen(new MessageBoxScreen(aboutMessage, false, true));

        }
        #endregion


        #region load levels from selected
        void loadLevelsFromMenuEntrySelected(object sender, EventArgs e)
        {
            if (screenManager.Settings.LoadLevelsFrom == 2)
                screenManager.Settings.LoadLevelsFrom = 0;
            else
                screenManager.Settings.LoadLevelsFrom += 1;

            SetMenuEntryText();
            SwitchScreen(this, new OptionsMenuScreen(ScreenManager, true, backgroundDemoScreen));

        }
        #endregion

        #region skins selected
        void skinsMenuEntrySelected(object sender, EventArgs e)
        {
            #region extra

                screenManager.Settings.Skin += 1;
                screenManager.loadSkinSettings(true);
                try
                {
                    skinName = screenManager.SkinSettings.skinTitle;
                }
                catch
                {
                    skinName = "Skin " + screenManager.Settings.Skin.ToString();
                }
                try
                {
                    if (screenManager.SkinSettings.hasMusic)
                    {
                        MediaPlayer.Stop();
                        MediaPlayer.IsRepeating = true;
                        MediaPlayer.Play(screenManager.BackgroundMusic);
                    }
                }
                catch
                {
                }

            #endregion

                    const string message = "For the current version, the game may need an in game restart to have skin changes take effect. Would you like to restart the game?";

                    MessageBoxScreen restartRequiredBox = new MessageBoxScreen(message, true, false);
                    restartRequiredBox.Accepted += restartRequiredBoxAccepted;
                    ScreenManager.AddScreen(restartRequiredBox);

            SetMenuEntryText();


        }

        void restartRequiredBoxAccepted(object sender, EventArgs e)
        {
            screenManager.SaveData();
            throw new Exception("RestartRequired");
        }
        
        #endregion

        #region levelfolderpath selected
        void levelFolderPathMenuEntrySelected(object sender, EventArgs e)
        {

#if WINDOWS
            try
            {
                FolderBrowserDialog FolderBrowser = new FolderBrowserDialog();
                FolderBrowser.Description = "Select a folder for the game to load levels from. If there are any missing level numbers that exist in the in game levels, the missing in game levels will then be copied over. Any numbers that pre exist will NOT be overwritten.";
                FolderBrowser.ShowNewFolderButton = true;
                FolderBrowser.ShowDialog();
                screenManager.Settings.LevelFolderPath = FolderBrowser.SelectedPath.ToString();
                Trace.Write(FolderBrowser.SelectedPath.ToString());
            }
            catch
            {
                screenManager.AddScreen(new MessageBoxScreen("An error has occured.", false, true));
            }

#endif
            SetMenuEntryText();

        }
        #endregion

        #region level jump selected
        void levelJumpSelected(object sender, EventArgs e)
        {
            screenManager.Settings.LevelJumpEnabled = !screenManager.Settings.LevelJumpEnabled;

            if (!changedsetting)
                changedsetting = true;
            SetMenuEntryText();
        }
        #endregion

        #region in game music selected

        void inGameMusicSelected(object sender, EventArgs e)
        {
            if (screenManager.Settings.InGameMusic)
            {
                screenManager.Settings.InGameMusic = false;
                MediaPlayer.Stop();
            }
            else
            {
                screenManager.Settings.InGameMusic = true;
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Play(screenManager.BackgroundMusic);
            }

            if (!changedsetting)
                changedsetting = true;

            SetMenuEntryText();
            screenManager.RemoveScreen(this);

            SwitchScreen(this, new OptionsMenuScreen(ScreenManager, true, backgroundDemoScreen));
        }
        #endregion

        #region music volume selected

        void musicVolumeSelected(object sender, EventArgs e)
        {
            screenManager.Settings.MusicVolumeNumber += 1;
            switch (screenManager.Settings.MusicVolumeNumber)
            {
                case 1:
                    screenManager.Settings.MusicVolumeAmount = 0.4f;
                    screenManager.Settings.MusicVolumeString = "Low";
                    screenManager.Settings.MusicVolumeNumber = 1;
                    break;
                case 2:
                    screenManager.Settings.MusicVolumeAmount = 0.6f;
                    screenManager.Settings.MusicVolumeString = "Medium";
                    screenManager.Settings.MusicVolumeNumber = 2;
                    break;
                case 3:
                    screenManager.Settings.MusicVolumeAmount = 1.0f;
                    screenManager.Settings.MusicVolumeString = "High";
                    screenManager.Settings.MusicVolumeNumber = 3;
                    break;
            }
            MediaPlayer.Volume = screenManager.Settings.MusicVolumeAmount;

            if (screenManager.Settings.MusicVolumeNumber >= 3)
                screenManager.Settings.MusicVolumeNumber = 0;

            if (!changedsetting)
                changedsetting = true;
            SetMenuEntryText();
        }
        #endregion

        #region sound effects selected

        void soundEffectsSelected(object sender, EventArgs e)
        {
            screenManager.Settings.SoundVolumeNumber += 1;
            switch (screenManager.Settings.SoundVolumeNumber)
            {
                case 1:
                    screenManager.Settings.SoundVolumeAmount = 0.0f;
                    screenManager.Settings.SoundVolumeString = "Off";
                    screenManager.OptionsSound.Play(0.0f, 0, 0);
                    screenManager.Settings.SoundVolumeNumber = 1;
                    break;
                case 2:
                    screenManager.Settings.SoundVolumeAmount = 0.4f;
                    screenManager.Settings.SoundVolumeString = "Low";
                    screenManager.OptionsSound.Play(0.4f, 0, 0);
                    screenManager.Settings.SoundVolumeNumber = 2;
                    break;
                case 3:
                    screenManager.Settings.SoundVolumeAmount = 0.6f;
                    screenManager.Settings.SoundVolumeString = "Medium";
                    screenManager.OptionsSound.Play(0.6f, 0, 0);
                    screenManager.Settings.SoundVolumeNumber = 3;
                    break;
                case 4:
                    screenManager.Settings.SoundVolumeAmount = 1.0f;
                    screenManager.Settings.SoundVolumeString = "High";
                    screenManager.OptionsSound.Play(1.0f, 0, 0);
                    screenManager.Settings.SoundVolumeNumber = 4;
                    break;
            }

            if (screenManager.Settings.SoundVolumeNumber >= 4)
                screenManager.Settings.SoundVolumeNumber = 0;

            if (!changedsetting)
                changedsetting = true;
            SetMenuEntryText();

        }
        #endregion

        #region FPS Selected
        void debugModeMenuEntrySelected(object sender, EventArgs e)
        {
            screenManager.Settings.DebugMode = !screenManager.Settings.DebugMode;

            if (!changedsetting)
                changedsetting = true;
            SetMenuEntryText();

        }
        #endregion

        #region back selected
        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the game.
        /// </summary>
        void backSelected(object sender, EventArgs e)
        {
            if (changedsetting)
            {
                const string message = "You have changed some settings. Do you want to save these settings?";

                MessageBoxScreen confirmSaveMessageBox = new MessageBoxScreen(message, true, false);

                confirmSaveMessageBox.Accepted += ConfirmSaveMessageBoxAccepted;
                confirmSaveMessageBox.Cancelled += ConfirmSaveMessageBoxCancelled;

                ScreenManager.AddScreen(confirmSaveMessageBox);
            }
            else
            {
                if (isFromMenu)
                    OnCancel();
                else
                    SwitchScreen(this, new PauseMenuScreen(gamePlayScreen));
            }
        }
        #endregion

        #region confirm save accepted
        /// <summary>
        /// Event handler for when the user selects ok on the save confirmation page
        /// </summary>
        void ConfirmSaveMessageBoxAccepted(object sender, EventArgs e)
        {
            screenManager.SaveData();
            if (changedsetting)
                changedsetting = false;
            saveSuccessAlert();

            if (isFromMenu)
                OnCancel();
            else
                SwitchScreen(this, new PauseMenuScreen(gamePlayScreen));
        }
        #endregion

        #region save settings selected

        void saveSettingsSelected(object sender, EventArgs e)
        {
            screenManager.SaveData();
            if (changedsetting)
                changedsetting = false;
            saveSuccessAlert();
        }
        #endregion


        #region save success alert
        void saveSuccessAlert()
        {
            const string message = "Settings saved\nsuccessfully!";

            MessageBoxScreen saveSuccessAlertBox = new MessageBoxScreen(message, false, false);

            ScreenManager.AddScreen(saveSuccessAlertBox);
        }
        #endregion

        #region confirm save cancelled
        /// <summary>
        /// Event handler for when the user selects cancel on the save confirmation page
        /// </summary>
        void ConfirmSaveMessageBoxCancelled(object sender, EventArgs e)
        {

            if (isFromMenu)
                OnCancel();
            else
                SwitchScreen(this, new PauseMenuScreen(gamePlayScreen));
        }
        #endregion

        #endregion


        #region draw section
        /// <summary>
        /// Draws the pause menu screen. This darkens down the gameplay screen
        /// that is underneath us, and then chains to the base MenuScreen.Draw.
        /// </summary>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!isFromMenu)
            {
                ScreenManager.FadeBackBufferToBlack(spriteBatch, TransitionAlpha * 2 / 3);
                base.Draw(gameTime, spriteBatch);
            }
            else
            {
                base.Draw(gameTime, spriteBatch);
            }
        }
        #endregion

    }
}
