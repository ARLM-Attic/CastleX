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
using Microsoft.Xna.Framework.Storage;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
#endregion

namespace CastleX
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class LevelSelectScreen : MenuScreen
    {
        #region Fields

        MenuEntry levelNumberMenuEntry;

        bool isinit = false;

        int levelNumber = 0;
        int maxLevels = 0;
        int maxLevelsFromZero = 0;
        int levelIndex = 0;
        string levelPath;

        #region Values used for the metadata when viewing the levels info.

        string levelInfoTitle, levelInfoDescription; //levelInfoTimerString, *** 
        //int levelInfoTimerInt = 0;

        private bool metaIsSet = false, metaTitleIsSet = false, metaDescriptionIsSet = false; //metaTimerIsSet = false, *** Timer
        public bool MetaIsSet
        {
            get { return metaIsSet; }
        }

        public bool MetaDescriptionIsSet
        {
            get { return metaDescriptionIsSet; }
        }

        public bool MetaTitleIsSet
        {
            get { return metaTitleIsSet; }
        }

        //public bool MetaTimerIsSet
        //{
        //    get { return metaTimerIsSet; }
        //} // *** Timer

        #endregion

        ScreenManager screenManager;
        #endregion

        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public LevelSelectScreen(ScreenManager screen)
            : base("Select Level")
        {
            if (!isinit)
            {
                screenManager = screen;
                isinit = true;

                #region retrieve the total amount of levels

                // Find the path of the next level.

            
                //Since there isnt a visible folder viewer on xbox
                //like we can on the computer, we check to see if this game is ran on the computer or not.
                
#if WINDOWS
                
            //string levelPath;

            if (screenManager.Settings.LoadLevelsFrom == 0)
            {
                levelPath = String.Format("Levels\\{0}.txt", ++levelIndex);
                levelPath = "GameContent\\" + levelPath;
                //If the user has the game set to load levels from the interior game,
                //the game will only count the levels from the interior games directory.

                //Open Book Directory and Get all files from inside it
                DirectoryInfo dir = new DirectoryInfo(".\\GameContent\\Levels\\");
                FileInfo[] dirfiles = dir.GetFiles();
                maxLevels = dirfiles.Length;
                //Since the amount of files starts at 1, we subtract one from
                //the amount of files so we have a new copy of the amount starting from 0.
                maxLevelsFromZero = dirfiles.Length - 1;
                Trace.Write("Max Levels: " + maxLevels + "\n");
            }
                /*
            else if (screenManager.Settings.LoadLevelsFrom == 1)
            {
                              //If the user has the game set to load levels from the game folder,
                //we will check if each file exists in the folder,
                //and if it doesnt exist, we are to copy the level from the games
                //interior folder to the external folder.
                //we then count the file that is in the container (after it is copied if it didnt exist before).

                StorageContainer mycontainer = screenManager.Device.OpenContainer("Castle_X");

                //Have a timer running that when a new level is counted it resets, but
                //but if it reaches 5 it will be done counting the levels.
                //This will make the game continue when it hasnt counted any new levels,
                //(meaning it reached the amount of levels), for a couple of seconds.
                int loadContainerTimer = 0;

                //Have a number that represents the current file number and increases it on each count.
                int numberOfLevel = 0;

                while (true)
                {
                    levelPath = String.Format("{00}.txt", ++numberOfLevel);

                    if (!File.Exists(Path.Combine(mycontainer.Path, "GameContent\\Levels\\" + levelPath)))
                    {
                        if (File.Exists(Path.Combine(StorageContainer.TitleLocation, "GameContent/Levels/" + levelPath)))
                        {
                            if (!Directory.Exists(Path.Combine(StorageContainer.TitleLocation, "GameContent/")))
                            {
                                Directory.CreateDirectory(Path.Combine(StorageContainer.TitleLocation, "GameContent/Levels/"));
                            }

                            if (!Directory.Exists(Path.Combine(StorageContainer.TitleLocation, "GameContent/Levels")))
                            {
                                Directory.CreateDirectory(Path.Combine(StorageContainer.TitleLocation, "GameContent/Levels/"));
                            }

                            File.Copy(Path.Combine(StorageContainer.TitleLocation, "GameContent/Levels/" + levelPath), Path.Combine(mycontainer.Path, "GameContent\\Levels\\" + levelPath));
                            Trace.Write("\nCopied the internal storage version of level " + numberOfLevel + " to the containers folder\n");
                            
                            //After we have copied the level from the game container
                            //to the users directory, we restart the timer and count the new level.
                            loadContainerTimer = 0;
                            Trace.Write(" copyTimer: " + loadContainerTimer + "  ");
                            numberOfLevel++;
                        }
                    }
                    else
                    {
                        //If the file already exists in the users directory,
                        //we restart the timer so we can load the next level 
                        loadContainerTimer = 0;
                        Trace.Write(" existTimer: " + loadContainerTimer + "  ");
                        numberOfLevel++;
                    }

                    loadContainerTimer += 1;
                    Trace.Write(" Timer: " + loadContainerTimer + "  ");
                    if (loadContainerTimer >= 10)
                    {
                        loadContainerTimer = 0;
                        DirectoryInfo dir = new DirectoryInfo(Path.Combine(mycontainer.Path, "GameContent\\Levels\\"));
                        FileInfo[] dirfiles = dir.GetFiles();
                        maxLevels = dirfiles.Length;
                        //Since the amount of files starts at 1, we subtract one from
                        //the amount of files so we have a new copy of the amount starting from 0.
                        maxLevelsFromZero = dirfiles.Length - 1;
                        Trace.Write("Max Levels: " + maxLevels + "\n");
                        break;
                    }
                }
                mycontainer.Dispose();
            }
            else if (screenManager.Settings.LoadLevelsFrom == 2)
            {
                //Have a timer running that when a new level is counted it resets, but
                //but if it reaches 5 it will be done counting the levels.
                //This will make the game continue when it hasnt counted any new levels,
                //(meaning it reached the amount of levels), for a couple of seconds.
                int loadContainerTimer = 0;

                //Have a number that represents the current file number and increases it on each count.
                int numberOfLevel = 0;

                while (true)
                {
                    levelPath = String.Format("{00}.txt", ++numberOfLevel);

                    if (!File.Exists(screenManager.Settings.LevelFolderPath +"\\" + levelPath))
                    {
                        if (File.Exists(Path.Combine(StorageContainer.TitleLocation, "GameContent/Levels/" + levelPath)))
                        {
                            if (!Directory.Exists(Path.Combine(StorageContainer.TitleLocation, "GameContent/")))
                            {
                                Directory.CreateDirectory(Path.Combine(StorageContainer.TitleLocation, "GameContent/Levels/"));
                            }

                            if (!Directory.Exists(Path.Combine(StorageContainer.TitleLocation, "GameContent/Levels")))
                            {
                                Directory.CreateDirectory(Path.Combine(StorageContainer.TitleLocation, "GameContent/Levels/"));
                            }

                            File.Copy(Path.Combine(StorageContainer.TitleLocation, "GameContent/Levels/" + levelPath), screenManager.Settings.LevelFolderPath + "\\" + levelPath);
                            Trace.Write("\nCopied the internal storage version of level " + numberOfLevel + " to the containers folder\n");
                            
                            //After we have copied the level from the game container
                            //to the users directory, we restart the timer and count the new level.
                            loadContainerTimer = 0;
                            Trace.Write(" copyTimer: " + loadContainerTimer + "  ");
                            numberOfLevel++;
                        }
                    }
                    else
                    {
                        //If the file already exists in the users directory,
                        //we restart the timer so we can load the next level 
                        loadContainerTimer = 0;
                        Trace.Write(" existTimer: " + loadContainerTimer + "  ");
                        numberOfLevel++;
                    }

                    loadContainerTimer += 1;
                    Trace.Write(" Timer: " + loadContainerTimer + "  ");
                    if (loadContainerTimer >= 10)
                    {
                        loadContainerTimer = 0;
                        DirectoryInfo dir = new DirectoryInfo(screenManager.Settings.LevelFolderPath +"\\" + levelPath);
                        FileInfo[] dirfiles = dir.GetFiles();
                        maxLevels = dirfiles.Length;
                        //Since the amount of files starts at 1, we subtract one from
                        //the amount of files so we have a new copy of the amount starting from 0.
                        maxLevelsFromZero = dirfiles.Length - 1;
                        Trace.Write("Max Levels: " + maxLevels + "\n");
                        break;
                    }
                }
            }
                 */
            
            
#else
                //if the game is ran on xbox, we go ahead and load the files
                //straight from the games itself, since there would be no use having a seperate folder
                //containing the games.
                DirectoryInfo dir = new DirectoryInfo(Path.Combine(StorageContainer.TitleLocation, "GameContent\\Levels\\"));
                FileInfo[] dirfiles = dir.GetFiles();
                maxLevels = dirfiles.Length;
                //Since the amount of files starts at 1, we subtract one from
                //the amount of files so we have a new copy of the amount starting from 0.
                maxLevelsFromZero = dirfiles.Length-1;
                Trace.Write("Max Levels: " + maxLevels + "\n");
#endif
                

                //screenManager.LoadNumberOfLevels();
                //maxLevels = screenManager.NumberOfLevels;


                #endregion

            }

            // Create our menu entries.

            levelNumberMenuEntry = new MenuEntry(string.Empty);
            levelNumberMenuEntry.Selected += increaseLevelNumber;
            MenuEntries.Add(levelNumberMenuEntry);

            MenuEntry loadLevelMenuEntry = new MenuEntry("Load Level");
            loadLevelMenuEntry.Selected += levelNumberSelected;
            MenuEntries.Add(loadLevelMenuEntry);
                
            MenuEntry backMenuEntry = new MenuEntry("Back");
            backMenuEntry.Selected += OnCancel;
            MenuEntries.Add(backMenuEntry);

     

            SetMenuEntryText();
        }


        /// <summary>
        /// Fills in the latest values for the options screen menu text.
        /// </summary>
        void SetMenuEntryText()
        {
            levelNumberMenuEntry.Text = "Level Number: " + levelNumber; 
        }


        #endregion

        #region Handle Input



        /// <summary>
        /// Event handler for when the level number is increased.
        /// </summary>
        void increaseLevelNumber(object sender, EventArgs e)
        {
            levelNumber += 1;
            if (levelNumber > maxLevels-1)
                levelNumber = 0;
            SetMenuEntryText();
        }

        /// <summary>
        /// Event handler for when the level number is decreased.
        /// </summary>
        void decreaseLevelNumber(object sender, EventArgs e)
        {
            levelNumber -= 1;

            if (levelNumber < 0)
                levelNumber = maxLevels-1;
            SetMenuEntryText();
        }

        /// <summary>
        /// Event handler for when the level number is decreased.
        /// </summary>
        void levelNumberSelected(object sender, EventArgs e)
        {

            #region Find the file for the selected level
#if DEBUG
            /*
            string levelsfolder = "Levels";
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
             */
#else
            levelPath = String.Format("Levels/{0}.txt", levelNumber);
            levelPath = Path.Combine(StorageContainer.TitleLocation, "GameContent/" + levelPath);
            Trace.Write("Level Path: " + levelPath + "\n");
#endif
            #endregion

            #region Read the levels file for the metadata.

            /*
            if (File.Exists(levelPath))
            {
                int width;
                List<string> lines = new List<string>();

                using (StreamReader reader = new StreamReader(levelPath))
                {
                    string line = reader.ReadLine();
                    width = line.Length;
                    while (line != null)
                    {
                        if (line.Length != width)
                        {
                            if (line.StartsWith("@"))
                            {
                                //if (line.StartsWith("@tim."))  //*** Timer
                                //{
                                //    levelInfoTimerInt = Int32.Parse(line.Remove(0, 5).ToString());
                                //    levelInfoTimerString = levelInfoTimerInt + ":00";
                                //    if (!metaIsSet)
                                //        metaIsSet = true;
                                //    if (!metaTimerIsSet)
                                //        metaTimerIsSet = true;
                                //}
                                //else
                                    if (line.StartsWith("@tit."))
                                    {
                                        levelInfoTitle = line.Remove(0, 5).ToString();
                                        if (!metaIsSet)
                                            metaIsSet = true;
                                        if (!metaTitleIsSet)
                                            metaTitleIsSet = true;
                                    }
                                    else
                                        if (line.StartsWith("@des."))
                                        {
                                            levelInfoDescription = line.Remove(0, 5).ToString();
                                            if (!metaIsSet)
                                                metaIsSet = true;
                                            if (!metaDescriptionIsSet)
                                                metaDescriptionIsSet = true;

                                        }

                            }
                        }

                        line = reader.ReadLine();


                        //if (metaTimerIsSet && levelInfoTimerInt < 1)
                        //{
                        //    levelInfoTimerString = "2:00";
                        //}

                        //if (!metaTimerIsSet)
                        //{
                        //    levelInfoTimerString = "2:00";
                        //} // ***

                        if (levelInfoTitle == null)
                        {
                            levelInfoTitle = levelIndex.ToString();
                        }

                        if (levelInfoDescription == null)
                        {
                            levelInfoDescription = "Explore the level looking for the exit.";
                        }
                    }
                }
            }
             */
            #endregion

            
            string message_levelInfo = "  Level Title:\n" + levelInfoTitle + "\n  Level Description:\n" + levelInfoDescription;
            Trace.Write("levelPath: " + levelPath + "\n");
            
            MessageBoxScreen box_levelInfo = new MessageBoxScreen(message_levelInfo, true, true);
            box_levelInfo.Accepted += box_levelInfo_Accepted;
            box_levelInfo.Cancelled += box_levelInfo_Cancelled;
            ScreenManager.AddScreen(box_levelInfo);
        }

        /// <summary>
        /// Event handler for when the level info box is exited, we can clear the metadata.
        /// </summary>
        void box_levelInfo_Accepted(object sender, EventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, new GameplayScreen(levelNumber-1, screenManager, false));
            metaDescriptionIsSet = false;
            metaIsSet = false;
            //metaTimerIsSet = false;
            metaTitleIsSet = false;
            levelInfoDescription = "";
            // levelInfoTimerInt = 0;
            //*** levelInfoTimerString = "";
            levelInfoTitle = "";
            levelPath = "";
        }
        void box_levelInfo_Cancelled(object sender, EventArgs e)
        {
            metaDescriptionIsSet = false;
            metaIsSet = false;
            //metaTimerIsSet = false;
            metaTitleIsSet = false;
            levelInfoDescription = "";
            //levelInfoTimerInt = 0;
            //***levelInfoTimerString = "";
            levelInfoTitle = "";
            levelPath = "";
        }
        

        #endregion
    }
}
