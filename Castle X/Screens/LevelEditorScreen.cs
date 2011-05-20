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
    #region Enums
    enum LevelEditorState
    {
        /// <summary>
        /// When the level editor is focused on the on-screen keyboard. (1)
        /// </summary>
        Keyboard = 1,

        /// <summary>
        /// When the level editor is focused on the Level Coordinations Screen. (2)
        /// </summary>
        LevelCords = 2,


        /// <summary>
        /// When the level editor is focused on the map. (3)
        /// </summary>
        Map = 3,


        /// <summary>
        /// When the level editor is focused on the menu bar. (4)
        /// </summary>
        Menu = 4,



    }
    enum LevelEditorToolbar
    {
        /// <summary>
        /// When the Menu Cursor is focused on the "Exit" button. (1)
        /// </summary>
        Exit = 1,

        /// <summary>
        /// When the Menu Cursor is focused on the "New" button. (2)
        /// </summary>
        New = 2,

        /// <summary>
        /// When the Menu Cursor is focused on the "Save" button. (3)
        /// </summary>
        Save = 3,
    }
    #endregion

    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    class LevelEditorScreen : GameScreen
    {
        #region Fields

        LevelEditorToolbar menuchoice = LevelEditorToolbar.Exit;

        ContentManager content;
        private int levelIndex = -1;
        private LevelEditor level;
        public LevelEditor MyLevel
        {
            get { return myLevel; }
            set { myLevel = value; }
        }
        public Vector2 LevelSize = Vector2.Zero;
        string Status = "Go Ahead!";

        int cursorx = 0;
        int cursory = 0;
        int menuchoiceint;
        int menuchoicevalues;
        LevelEditor myLevel;
        public LevelEditorState EditState = LevelEditorState.Map;
        /// <summary>
        /// 1
        /// </summary>
        Texture2D Button_Exit;
        /// <summary>
        /// 2
        /// </summary>
        Texture2D Button_New;
        /// <summary>
        /// 3
        /// </summary>
        Texture2D Button_Save;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="playSavedLevels">If true, the game will load the levels that were built using the in game level editor. if false, the game will load the original levels.</param>
        public LevelEditorScreen(ScreenManager screenManager)
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            // Framerate differs between platforms.
            levelIndex = -1;
            screenManager.IsDemo = false;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="playSavedLevels">If true, the game will load the levels that were built using the in game level editor. if false, the game will load the original levels.</param>
        public LevelEditorScreen(ScreenManager screenManager, Vector2 levelSize)
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            // Framerate differs between platforms.
            levelIndex = -1;
            this.LevelSize = levelSize;
            screenManager.IsDemo = false;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="loadLevelNumber">The number of a certain level to load.</param>
        /// <param name="playSavedLevels">If true, the game will load the levels that were built using the in game level editor. if false, the game will load the original levels.</param>
        public LevelEditorScreen(int loadLevelNumber, ScreenManager screenManager, bool playSavedLevels)
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            // Framerate differs between platforms.
            levelIndex = loadLevelNumber;
            screenManager.IsDemo = false;
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


            // Load overlay textures

            Button_Exit = content.Load<Texture2D>("mapeditor/menu_exit");
            Button_New = content.Load<Texture2D>("mapeditor/menu_newdoc");
            Button_Save = content.Load<Texture2D>("mapeditor/menu_save");
            LoadNextLevel();
            menuchoicevalues = int.Parse(Enum.GetValues(typeof(LevelEditorToolbar)).Length.ToString());
            // A real game would probably have more content than this sample, so
            // it would take longer to load. We simulate that by delaying for a
            // while, giving you a chance to admire the beautiful loading screen.
            Thread.Sleep(1000);

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

        #region Update


        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {

            level.Update(gameTime);
          
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

        }

#endregion

        #region HandleInput

        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            menuchoiceint = (int)menuchoice;
            if (input.Up_Pressed)
            {
                switch (EditState)
                {
                    case LevelEditorState.Map:
                        if (level.CursorLocation.Y > 0)
                            level.CursorLocation.Y--;
                        else
                            level.CursorLocation.Y = level.tilesAmount.Y;
                        UpdateCursorCords();
                        UpdateStatus();
                        break;
                    default:
                        Status = "UP Unavailable in: " + EditState.ToString();
                        break;
                }
            }

            if (input.Left_Pressed)
            {
                switch (EditState)
                {
                    case LevelEditorState.Map:
                        if (level.CursorLocation.X > 0)
                            level.CursorLocation.X--;
                        else
                            level.CursorLocation.X = level.tilesAmount.X - 1;
                        UpdateCursorCords();
                        UpdateStatus();
                        break;
                    case LevelEditorState.Menu:
                        if ((int)menuchoice > 1)
                            menuchoice--;
                        else
                            menuchoice = LevelEditorToolbar.Save;
                        UpdateStatus();
                        break;
                    default:
                        Status = "LEFT Unavailable in: " + EditState.ToString();
                        break;

                }
            }
            if (input.Down_Pressed)
            {
                switch (EditState)
                {
                    case LevelEditorState.Map:
                        if (level.CursorLocation.Y < level.tilesAmount.Y)
                            level.CursorLocation.Y += 1;
                        else
                            level.CursorLocation.Y = 0;
                        UpdateCursorCords();
                        UpdateStatus();
                        break;
                    default:
                        Status = "DOWN Unavailable in: " + EditState.ToString();
                        break;

                }
            }

            if (input.Right_Pressed)
            {
                switch (EditState)
                {
                    case LevelEditorState.Map:
                        if (level.CursorLocation.X < level.tilesAmount.X - 1)
                            level.CursorLocation.X++;
                        else
                            level.CursorLocation.X = 0;
                        UpdateCursorCords();
                        UpdateStatus();
                        break;
                    case LevelEditorState.Menu:
                        if ((int)menuchoice < menuchoicevalues)
                            menuchoice++;
                        else
                            menuchoice = LevelEditorToolbar.Exit;
                        UpdateStatus();
                        break;
                    default:
                        Status = "RIGHT Unavailable in: " + EditState.ToString();
                        break;
                }
                
            }

            if (input.A_Pressed)
            {
                switch (EditState)
                {
                    case LevelEditorState.Map:
                        UpdateCursorCords();
                        level.tiles[cursorx, cursory].Toggle();
                        UpdateStatus();
                        break;
                    case LevelEditorState.Menu:
                        switch (menuchoice)
                        {
                            case LevelEditorToolbar.Exit:
                                LoadingScreen.Load(ScreenManager, true, new BackgroundScreen(), new MainMenuScreen(ScreenManager));
                                break;
                            default:
                                Status = "Not Available Yet!";
                                break;
                        }
                        break;
                    default:
                        Status = "A Unavailable in: " + EditState.ToString();
                        break;
                }
            }
            if (input.B_Pressed)
            {
                switch (EditState)
                {
                    case LevelEditorState.Map:
                        EditState = LevelEditorState.Menu;
                        UpdateStatus();
                        break;
                    case LevelEditorState.Menu:
                        EditState = LevelEditorState.Map;
                        UpdateStatus();
                        break;
                    default:
                        Status = "B Unavailable in: " + EditState.ToString();
                        break;
                }
            }
        }

        #endregion

        #region Update SubMethods

        private void UpdateStatus()
        {
            switch (EditState)
            {
                case LevelEditorState.Map:
                    UpdateStatusToChosenTile();
                    break;
                case LevelEditorState.Menu:
                    UpdateMenuStatus();
                    break;
                default:
                    Status = "Unavailable";
                    break;
            }
        }

        private void UpdateCursorCords()
        {
            cursorx = int.Parse(level.CursorLocation.X.ToString());
            cursory = int.Parse(level.CursorLocation.Y.ToString());
        }

        private void UpdateStatusToChosenTile()
        {
            try
            {
                Status = "Tile(" + cursorx.ToString() + "," + cursory.ToString() + "): " + level.tiles[cursorx, cursory].ItemType.ToString();
            }
            catch
            {
                Status = "Tile(" + cursorx.ToString() + "," + cursory.ToString() + "): Error";
            }
        }

        void UpdateMenuStatus()
        {
            switch (menuchoice)
            {
                case LevelEditorToolbar.Exit:
                    Status = "Return To Menu";
                    break;
                case LevelEditorToolbar.Save:
                    Status = "Save Level";
                    break;
                case LevelEditorToolbar.New:
                    Status = "New Level";
                    break;
                default:
                    Status = "Unavailable";
                    break;

            }
        }

        public void LoadNextLevel()
        {
            level = new LevelEditor(ScreenManager.Game.Services, Path.Combine("GameContent", "demoscreen.txt"), ScreenManager, this);
        }

        #endregion

        #region Draw


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.Black, 0, 0);

            int ScreenWidth = ScreenManager.GraphicsDevice.Viewport.Width;
            int ScreenHeight = ScreenManager.GraphicsDevice.Viewport.Height;
            string statusWrapped = ScreenManager.WordWrap(ScreenWidth / 4 * 3, Status, ScreenManager.SmallFont);
            Vector2 StatusSize = ScreenManager.SmallFont.MeasureString(statusWrapped);
            int HeightFromBottom = ScreenHeight - (int)StatusSize.Y;

            ////SpriteBatch.Begin();
            switch (EditState)
            {
                case LevelEditorState.Map:
                    level.Draw(gameTime, spriteBatch, true);
                    DrawMenu(spriteBatch, 0);
                    break;
                case LevelEditorState.Menu:
                    level.Draw(gameTime, spriteBatch, false);
                    DrawMenu(spriteBatch, menuchoiceint);
                    break;
                default:

                    break;

            }

            spriteBatch.DrawString(ScreenManager.SmallFont, statusWrapped, new Vector2(ScreenWidth / 2, HeightFromBottom + 5), Color.White, 0, StatusSize / 2, 1.0f, SpriteEffects.None, 0);
            
            ////SpriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0)
                ScreenManager.FadeBackBufferToBlack(spriteBatch, 255 - TransitionAlpha);
        }

        private void DrawMenu(SpriteBatch spriteBatch, int numberchosen)
        {            
            int menuchoicevaluesplusone = menuchoicevalues + 1;
            Color[] chosencolor = new Color[menuchoicevaluesplusone];
            for (int i = 0; i < menuchoicevaluesplusone; i++)
            {
                if (i != numberchosen)
                    chosencolor[i] = Color.Gray;
                chosencolor[numberchosen] = Color.White;
            }

            //We start off with the 1 chosencolor, rather than 0. when we are off the toolbar
            //, 0 will be the chosen one. Since theres no texture colored for 0, the menu will be gray.

            spriteBatch.Draw(Button_Exit, new Rectangle(10, 10, 20, 20), chosencolor[1]);
            spriteBatch.Draw(Button_New, new Rectangle(40, 10, 20, 20), chosencolor[2]);
            spriteBatch.Draw(Button_Save, new Rectangle(70, 10, 20, 20), chosencolor[3]);
        
        
        }

        #endregion
    }
}
