#region File Description
//-------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
#endregion

namespace Castle_X
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    class LevelEditorScreen_old : GameScreen
    {
        #region our 9000+ variables

        ContentManager content;
        Texture2D[] objects;
        Texture2D[] menuobjects;
        Random random;
        int myrandomchoice;
        int numberonrow = 0;
        MessageBoxScreen changeSizeMessageBox;
        MessageBoxScreen confirmQuitMessageBox;
        int screenxpos = 10;
        int screenypos = 40;
        bool moveMapEnabled = false;
        int tileon = 1;
        int[] tilechoice, varx, vary, myextranumber;
        int numberofcells = 100;
        int numberofrows = 0;
        bool isnumberinit = false, isOnMap = true, boxUp = false;
        //bool isnumberinit2 = false;
        int menunumber = 0;
        string statusstring;
        bool buttonapressed = false, buttonrightpressed = false, buttonleftpressed = false, buttonuppressed = false, buttondownpressed = false, buttonbpressed = false, buttonbackpressed = false;
        #endregion

        #region Initializers
        /// <summary>
        /// Constructor for a keyboard that has just one input box
        /// </summary>
        public LevelEditorScreen_old()
        {


        }
        /// <summary>
        /// Constructor for a keyboard that has just one input box
        /// </summary>
        public LevelEditorScreen_old(int newNumberRows)
        {
            numberofcells = newNumberRows * newNumberRows;

        }
#endregion

        #region Loaders and Unloaders
        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            //if (content == null)
            //    content = new ContentManager(ScreenManager.Game.Services, "GameContent");

            ////This is used for the random map generator
            //random = new Random();

            ////Our variables for each cell.
            //tilechoice = new int[numberofcells + 1];
            //varx = new int[numberofcells + 1];
            //vary = new int[numberofcells + 1];

            ////dont remember exactly myextranumber is used for
            ////but it isnt hurting anybody by being there, so keep it.
            //myextranumber = new int[numberofcells + 1];

            ////Split the amount of cells to get the number of rows
            //numberofrows = (int)Math.Sqrt(numberofcells);

            ////These are the tiles used for the map
            //objects = new Texture2D[14];
            //objects[0] = content.Load<Texture2D>("Mapeditor/blank");
            //objects[1] = content.Load<Texture2D>("Mapeditor/FallingTile");
            //objects[2] = content.Load<Texture2D>("Mapeditor/block");
            //objects[3] = content.Load<Texture2D>("Mapeditor/exit");
            //objects[4] = content.Load<Texture2D>("Mapeditor/Coin");
            //objects[5] = content.Load<Texture2D>("Mapeditor/monstera");
            //objects[6] = content.Load<Texture2D>("Mapeditor/monsterb");
            //objects[7] = content.Load<Texture2D>("Mapeditor/monsterc");
            //objects[8] = content.Load<Texture2D>("Mapeditor/monsterd");
            //objects[9] = content.Load<Texture2D>("Mapeditor/passableblock");
            //objects[10] = content.Load<Texture2D>("Mapeditor/platform");
            //objects[11] = content.Load<Texture2D>("Mapeditor/player");
            //objects[12] = content.Load<Texture2D>("Mapeditor/powerup");


            ////These are the graphics used for the menubar

            //menuobjects = new Texture2D[5];
            //menuobjects[0] = content.Load<Texture2D>("Mapeditor/menu_exit");
            //menuobjects[1] = content.Load<Texture2D>("Mapeditor/menu_newdoc");
            //menuobjects[2] = content.Load<Texture2D>("Mapeditor/menu_save");
            //menuobjects[3] = content.Load<Texture2D>("Mapeditor/menu_move");
            //menuobjects[4] = content.Load<Texture2D>("Mapeditor/menu_random");

            // A real game would probably have more content than this sample, so
            // it would take longer to load. We simulate that by delaying for a
            // while, giving you a chance to admire the beautiful loading screen.
            //Thread.Sleep(50);

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
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);


            for (int i = 1; i <= numberofcells; i++)
            {
                myextranumber[i] = i;
                if (!isnumberinit)
                {
                    while (true)
                    {
                        if (myextranumber[i] > numberofrows)
                        {
                            vary[i] += 1;
                            myextranumber[i] -= numberofrows;
                        }
                        else
                        {
                            varx[i] = myextranumber[i];
                            break;
                        }
                    }
                    if (i == numberofcells)
                    {
                        isnumberinit = true;
                    }
                }
            }
        }
#endregion

        #region Input
        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");
            if (IsActive)
            {
                if (!buttonapressed && (GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed || Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Space)))
                    buttonapressed = true;

                if (buttonapressed && (GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Released && Keyboard.GetState(PlayerIndex.One).IsKeyUp(Keys.Space)))
                {
                    if (moveMapEnabled)
                    {
                        moveMapEnabled = false;
                    }
                    else
                    {
                        if (isOnMap)
                        {
                            try
                            {
                                buttonapressed = false;
                                if (tilechoice[tileon] >= 12)
                                    tilechoice[tileon] = 0;
                                else
                                    tilechoice[tileon] += 1;

                                statusString = "Changed Tile (" + varx[tileon] + ", " + vary[tileon] + ") to " + tileName[tilechoice[tileon]];
                            }
                            catch
                            {
                                statusString = "Please Choose A Valid Location!";
                            }
                        }
                        else
                        {
                            if (menunumber == 1)
                            {
                                if (!boxUp)
                                {
                                    boxUp = true;
                                    exitLevelMaker();
                                }
                            }
                            else if (menunumber == 2)
                            {
                                if (!boxUp)
                                {
                                    boxUp = true;
                                    ChangeMapSize();
                                }
                            }
                            else if (menunumber == 3)
                            {
                                if (!boxUp)
                                {
                                    boxUp = true;
                                    saveLevel();
                                }
                            }
                            else if (menunumber == 4)
                            {
                                screenxpos = 10;
                                screenypos = 40;
                            }
                            else if (menunumber == 5)
                            {
                                bool randomplayerisset = false;
                                for (int i = 1; i <= numberofcells; i++)
                                {
                                    //There are normally 13 options, but we add a few more to
                                    //the randomizer, so if it is over the normal amount, we
                                    //can turn it into a blank spot, making it more random
                                    //and realistic at the same time
                                    myrandomchoice = random.Next(0, 22);
                                    if (myrandomchoice >= 13)
                                        myrandomchoice = 0;
                                    //With the player being our 12th item, we have an true false
                                    //statement to check if we have placed the player somewhere else
                                    //along the map, if he already exists, the second spot becomes blank
                                    if (myrandomchoice == 12)
                                    {
                                        if (randomplayerisset)
                                            myrandomchoice = 0;
                                        else
                                            randomplayerisset = true;
                                    }
                                    tilechoice[i] = myrandomchoice;
                                }
                            }
                        }
                    }
                }
                



                if (!buttondownpressed && (GamePad.GetState(PlayerIndex.One).DPad.Down == ButtonState.Pressed || Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Down)))
                    buttondownpressed = true;

                if (buttondownpressed && (GamePad.GetState(PlayerIndex.One).DPad.Down == ButtonState.Released && Keyboard.GetState(PlayerIndex.One).IsKeyUp(Keys.Down)))
                {
                    buttondownpressed = false;
                    if (!moveMapEnabled)
                    {
                        if (isOnMap)
                        {
                            if (tileon < numberofcells)
                            {
                                tileon += numberofrows;
                                screenypos -= 20;
                                
                            }
                            else
                            {
                                tileon -= numberofcells;
                                screenypos = 40;
                            }
                        }
                    }
                }
                if (!buttonuppressed && (GamePad.GetState(PlayerIndex.One).DPad.Up == ButtonState.Pressed || Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Up)))
                    buttonuppressed = true;

                if (buttonuppressed && (GamePad.GetState(PlayerIndex.One).DPad.Up == ButtonState.Released && Keyboard.GetState(PlayerIndex.One).IsKeyUp(Keys.Up)))
                {
                    buttonuppressed = false;
                    if (!moveMapEnabled)
                    {
                        if (isOnMap)
                        {
                            if (tileon > 0)
                            {
                                tileon -= numberofrows;
                                screenypos += 20;
                            }
                            else
                            {
                                tileon += numberofcells;
                                screenypos = 40;
                            }
                        }
                    }
                }



                if (!buttonrightpressed && (GamePad.GetState(PlayerIndex.One).DPad.Right == ButtonState.Pressed || Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Right)))
                    buttonrightpressed = true;

                if (buttonrightpressed && (GamePad.GetState(PlayerIndex.One).DPad.Right == ButtonState.Released && Keyboard.GetState(PlayerIndex.One).IsKeyUp(Keys.Right)))
                {
                    buttonrightpressed = false;
                    if (boxUp)
                    {
                        boxUp = false;
                    }
                    if (!moveMapEnabled)
                    {
                        if (isOnMap)
                        {
                            if (tileon < numberofcells)
                            {
                                tileon += 1;
                                numberonrow += 1;
                                screenxpos -= 20;
                                if (numberonrow == numberofrows)
                                {
                                    Trace.Write("\nNumber on row1: " + numberonrow.ToString() + "\n");
                                    numberonrow = 0;
                                    Trace.Write("\nNumber on row2: " + numberonrow.ToString() + "\n");

                                    screenxpos = 10;
                                }
                            }
                            else
                            {
                                tileon = 0;
                                numberonrow = 0;
                                screenxpos = 10;
                            }
                        }
                        else
                        {
                            menunumber += 1;
                            switch (menunumber)
                            {
                                case -1:
                                    menunumber = 5;
                                    statusString = "Random Map";
                                    break;
                                case 0:
                                    menunumber = 5;
                                    statusString = "Random Map";
                                    break;
                                case 1:
                                    menunumber = 1;
                                    statusString = "Exit Level Maker";
                                    break;
                                case 2:
                                    menunumber = 2;
                                    statusString = "Change Map Size";
                                    break;
                                case 3:
                                    menunumber = 3;
                                    statusString = "Save Level";
                                    break;
                                case 4:
                                    menunumber = 4;
                                    statusString = "Reset Map Position";
                                    break;
                                case 5:
                                    menunumber = 5;
                                    statusString = "Random Map";
                                    break;
                                case 6:
                                    menunumber = 1;
                                    statusString = "Exit Level Maker";
                                    break;
                            }
                        }
                    }
                }
                if (!buttonleftpressed && (GamePad.GetState(PlayerIndex.One).DPad.Left == ButtonState.Pressed || Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Left)))
                    buttonleftpressed = true;

                if (buttonleftpressed && (GamePad.GetState(PlayerIndex.One).DPad.Left == ButtonState.Released && Keyboard.GetState(PlayerIndex.One).IsKeyUp(Keys.Left)))
                {
                    buttonleftpressed = false;
                    if (boxUp)
                    {
                        boxUp = false;
                    }
                    if (!moveMapEnabled)
                    {
                        if (isOnMap)
                        {
                            if (tileon > 0)
                            {
                                tileon -= 1;
                                screenxpos += 20;
                                numberonrow -= 1;
                                if (numberonrow == -1)
                                {
                                    Trace.Write("\nNumber on row1: " + numberonrow.ToString() + "\n");
                                    numberonrow = numberofrows;
                                    Trace.Write("\nNumber on row2: " + numberonrow.ToString() + "\n");

                                    screenxpos = 10;
                                }
                            }
                            else
                            {
                                tileon += numberofcells;
                                screenxpos = 10;
                                numberonrow = 0;
                            }
                        }
                        else
                        {
                            menunumber -= 1;
                            switch (menunumber)
                            {
                                case -1:
                                    menunumber = 5;
                                    statusString = "Random Map";
                                    break;
                                case 0:
                                    menunumber = 5;
                                    statusString = "Random Map";
                                    break;
                                case 1:
                                    menunumber = 1;
                                    statusString = "Exit Level Maker";
                                    break;
                                case 2:
                                    menunumber = 2;
                                    statusString = "Change Map Size";
                                    break;
                                case 3:
                                    menunumber = 3;
                                    statusString = "Save Level";
                                    break;
                                case 4:
                                    menunumber = 4;
                                    statusString = "Reset Map Position";
                                    break;
                                case 5:
                                    menunumber = 5;
                                    statusString = "Random Map";
                                    break;
                                case 6:
                                    menunumber = 1;
                                    statusString = "Exit Level Maker";
                                    break;
                            }

                        }
                    }
                }

                if (moveMapEnabled)
                {
                    if (buttonuppressed)
                        screenypos -= 4;
                    if (buttondownpressed)
                        screenypos += 4;
                    if (buttonleftpressed)
                        screenxpos -= 4;
                    if (buttonrightpressed)
                        screenxpos += 4;
                }


                if (!buttonbpressed && (GamePad.GetState(PlayerIndex.One).Buttons.B == ButtonState.Pressed || Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Enter)))
                    buttonbpressed = true;

                if (buttonbpressed && (GamePad.GetState(PlayerIndex.One).Buttons.B == ButtonState.Released && Keyboard.GetState(PlayerIndex.One).IsKeyUp(Keys.Enter)))
                {
                    buttonbpressed = false;
                    if (moveMapEnabled)
                    {
                        moveMapEnabled = false;
                    }
                    else
                    {
                        if (isOnMap)
                        {
                            isOnMap = false;
                            menunumber = 0;
                            tileon = 0;
                            statusString = "Switched To Menu";
                        }
                        else
                        {
                            isOnMap = true;
                            moveMapEnabled = false;
                            statusString = "Switched To Map";
                            menunumber = 0;
                            tileon = 1;
                        }
                    }
                }

                if (!buttonbackpressed && (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Back)))
                    buttonbackpressed = true;

                if (buttonbackpressed && (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Released && Keyboard.GetState(PlayerIndex.One).IsKeyUp(Keys.Back)))
                {
                    buttonbackpressed = false;
                    if (moveMapEnabled)
                        moveMapEnabled = false;
                }
            }
        }
        #endregion

        #region Actions for menu

        private void exitLevelMaker()
        {
                const string quitMessage = "Are you sure you want to exit? any unsaved data will be lost.";

                confirmQuitMessageBox = new MessageBoxScreen(quitMessage, true, false);
                confirmQuitMessageBox.Accepted += exitLevelMakerConfirmed;
                confirmQuitMessageBox.Cancelled += exitLevelMakerCancelled;

                ScreenManager.AddScreen(confirmQuitMessageBox);

        }
        void exitLevelMakerConfirmed(object sender, EventArgs e)
        {
            /////////////////////GO TO MAIN MENU///////////////////
            boxUp = false;
            if (ScreenManager.HaveDemoScreen)
            {
                BackgroundDemoScreen backgroundDemoScreen = new BackgroundDemoScreen(ScreenManager);
                LoadingScreen.Load(ScreenManager, true, backgroundDemoScreen,
                                                         new MainMenuScreen(ScreenManager));
            }
            else
            {
                BackgroundScreen backgroundScreen = new BackgroundScreen();
                LoadingScreen.Load(ScreenManager, true, backgroundScreen,
                                                         new MainMenuScreen(ScreenManager));
            }
        }
        private void ChangeMapSize()
        {
            const string changeSizeMessage = "Changing the map size will require a new map, are you sure?";

            changeSizeMessageBox = new MessageBoxScreen(changeSizeMessage, true, true);
            changeSizeMessageBox.Accepted += ChangeMapSizeAccepted;
            changeSizeMessageBox.Cancelled += ChangeMapSizeCancelled;
            ScreenManager.AddScreen(changeSizeMessageBox);

        }
        void ChangeMapSizeAccepted(object sender, EventArgs e)
        {
            boxUp = false;
            LoadingScreen.Load(ScreenManager, true, new BackgroundScreen(),
                                                     new NumberScreen());
        }
        void ChangeMapSizeCancelled(object sender, EventArgs e)
        {
            ScreenManager.RemoveScreen(changeSizeMessageBox);
        }
        void exitLevelMakerCancelled(object sender, EventArgs e)
        {
            ScreenManager.RemoveScreen(confirmQuitMessageBox);
        }

        private void saveLevel()
        {
            ScreenManager.LoadNumberOfLevels();
                try
                {
                    savelevelenginepermanent();
                }
                catch
                {
                    statusString = "Error Saving";
                }
            

            boxUp = false;
        }
        #endregion

        //Since the save engine is still under work,
        //The engine spits out the lines into the output.
        //No part is actually "saved".

        /*

        #region SAVING ENGINE
        void savelevelengine()
        {
            ScreenManager.LoadNumberOfLevels();
            string[] line = new string[numberofrows];
            int currentline = 0;

            for (int i = 1; i <= numberofcells; i++)
                if (line[currentline] == null || line[currentline].Length < numberofrows)
                    line[currentline] += tileVar[tilechoice[i]];
                else
                    line[++currentline] = tileVar[tilechoice[i]];




            if (ScreenManager.ourDevice != null)
            {
                StorageContainer containers = ScreenManager.ourDevice.OpenContainer("Castle_X");
                StreamWriter writer = new StreamWriter(Path.Combine(containers.Path, "GameContent\\SavedLevels\\SavedLevel.txt"));
                for (int i = 0; i < line.Count; i++)
                {
                    for (int i = 0; i < numberofrows / numberofrows; i++)
                        writer.WriteLine(line[i]);
                }
                writer.Flush();
                writer.Close();
                containers.Dispose();
            }
        }
#endregion
        */


        #region SAVING ENGINE
        void savelevelenginepermanent()
        {
            int numberoflevelsplusone = ScreenManager.NumberOfLevels;
            while (true)
            {
                string[] line = new string[numberofrows];
                int currentline = 0;

                for (int i = 1; i <= numberofcells; i++)
                {
                    if (line[currentline] == null || line[currentline].Length < numberofrows)
                        line[currentline] += tileVar[tilechoice[i]];
                    else
                        line[++currentline] = tileVar[tilechoice[i]];
                }

                
                    if (ScreenManager.Device != null)
                    {
                        StorageContainer containers = ScreenManager.Device.OpenContainer("Castle_X");
                        if (!Directory.Exists(Path.Combine(containers.Path, "GameContent")))
                            Directory.CreateDirectory(Path.Combine(containers.Path, "GameContent"));
                        if (!Directory.Exists(Path.Combine(containers.Path, "GameContent\\SavedLevels")))
                            Directory.CreateDirectory(Path.Combine(containers.Path, "GameContent\\SavedLevels"));
                        StreamWriter writer = new StreamWriter(Path.Combine(containers.Path, "GameContent\\SavedLevels\\" + numberoflevelsplusone.ToString() + ".txt"));

                        for (int i = 0; i < numberofrows / numberofrows; i++)
                            writer.WriteLine(line[i]);

                        writer.WriteLine("@tit.Saved Level " + numberoflevelsplusone.ToString());
                        writer.WriteLine("@tim.4");
                        writer.WriteLine("@des.Search the level looking for the exit!");
                        writer.Flush();
                        writer.Close();
                        Trace.Write("\n\nWrote Level\n\n" + numberoflevelsplusone.ToString() + "--" + Path.Combine(containers.Path, "GameContent\\SavedLevels\\" + numberoflevelsplusone.ToString() + ".txt").ToString() + "\n\n");
                        containers.Dispose();
                        break;
                    }

            }
           
            Trace.Write("\n\nDone Writing Level " + numberoflevelsplusone.ToString() + "\n\n");
            statusString = "Wrote Level " + numberoflevelsplusone.ToString();
            /*if (ScreenManager.HaveDemoScreen)
            {
                BackgroundDemoScreen backgroundDemoScreen = new BackgroundDemoScreen(ScreenManager);
                LoadingScreen.Load(ScreenManager, true, backgroundDemoScreen,
                                                         new MainMenuScreen(ScreenManager));
            }
            else
            {
                BackgroundScreen backgroundScreen = new BackgroundScreen();
                LoadingScreen.Load(ScreenManager, true, backgroundScreen,
                                                         new MainMenuScreen(ScreenManager));
            }*/
        }
        #endregion




        #region Drawing section
        void DrawTile(SpriteBatch spriteBatch, int tilenumber, int mynumber, int varx, int vary, Color color)
        {
                spriteBatch.Draw(objects[tilenumber], new Rectangle(varx * 20 + screenxpos, vary * 20 + screenypos, 20, 20), color);
            
        }

        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // This game has a blue background. Why? Because!
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.Black, 0, 0);

            // Our player and enemy are both actually just text strings.
            string statusWrapped = ScreenManager.WordWrap(200, statusString, ScreenManager.SmallFont);
            int ScreenWidth = ScreenManager.GraphicsDevice.Viewport.Width;
            int ScreenHeight = ScreenManager.GraphicsDevice.Viewport.Height;
            Vector2 StatusSize = ScreenManager.SmallFont.MeasureString(statusWrapped);
            int HeightFromBottom = ScreenHeight - (int)StatusSize.Y;
            
            //SpriteBatch.Begin();

            for (int i = 1; i <= numberofcells; i++)
            {
                if (tileon == i)
                {
                    DrawTile(spriteBatch, tilechoice[i], i, varx[i], vary[i], Color.White);
                }
                else
                {
                    DrawTile(spriteBatch, tilechoice[i], i, varx[i], vary[i], Color.Gray);
                }

            }

            if (isOnMap)
            spriteBatch.Draw(ScreenManager.GradientTexture, new Rectangle(0, 0, ScreenWidth, 22), Color.Gray);
            else
                spriteBatch.Draw(ScreenManager.GradientTexture, new Rectangle(0, 0, ScreenWidth, 22), Color.White);


            for (int i = 0; i < menuobjects.Length; i++)
            {
                if (menunumber == i + 1)
                    spriteBatch.Draw(menuobjects[i], new Rectangle(i * 25, 0, 20, 20), Color.White);
                else
                    spriteBatch.Draw(menuobjects[i], new Rectangle(i * 25, 0, 20, 20), Color.Gray);
            }

            spriteBatch.Draw(ScreenManager.GradientTexture, new Rectangle(0, HeightFromBottom, ScreenWidth, (int)StatusSize.Y), Color.White);
            spriteBatch.DrawString(ScreenManager.SmallFont, statusWrapped, new Vector2(ScreenWidth / 2, HeightFromBottom+5), Color.White, 0, StatusSize/2, 1.0f, SpriteEffects.None, 0);
            //SpriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0)
                ScreenManager.FadeBackBufferToBlack(spriteBatch, 255 - TransitionAlpha);


        }
#endregion

    }
}
