using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Xna.Framework.Input;

namespace CastleX
{
    /// <summary>
    /// A uniform grid of tiles with suport collections as items and enemies, for example.
    /// The level owns the player and controls the game's win and lose
    /// conditions as well as scoring.
    /// </summary>
    public class Level : IDisposable
    {
        // Physical structure of the level.
        private Tile[,] tiles;
        private Layer[] layers;
        private String[][] layerNames = new String[3][];
        // The layer which entities are drawn on top of.
        private const int EntityLayer = 2;

        // Entities in the level.

        public Boss Boss
        {
            get { return boss; }
            set { boss = value; }
        }
        Boss boss;

        public List<Item> items = new List<Item>();
        public List<AnimatedItem> animatedItems = new List<AnimatedItem>();
        public List<MultipleStateItem> multipleStateItems = new List<MultipleStateItem>();
        // Level exits and entrances
        public List<Exit> exits = new List<Exit>();
        public List<Entrance> entrances = new List<Entrance>();

        public List<FallingTile> FallingTiles = new List<FallingTile>();
        public List<Enemy> enemies = new List<Enemy>();
        public List<Boss> bosses = new List<Boss>();
        public List<Arrow> arrows = new List<Arrow>();
        public List<VanishingTile> VanishingTiles = new List<VanishingTile>();
        public List<HiddenTile> HiddenTiles = new List<HiddenTile>();
        public List<Spring> Springs = new List<Spring>();
        public List<DeathTile> DeathTiles = new List<DeathTile>();
        public List<BossJumpTile> BossJumpTiles = new List<BossJumpTile>();
        //  Lista das plataformas móveis
        public List<MovingItem> MovingItems = new List<MovingItem>();
        public Boolean MovingItemsAreActive = true;

        public ScreenManager screenManager;
        // Key locations in the level.        
        private Vector2 start;
        private Point exit = InvalidPosition;
        private static readonly Point InvalidPosition = new Point(-1, -1);
        // Level game state.
        private Random random = new Random(354668); // Arbitrary, but constant seed
        private Vector2 cameraPosition;

        bool IsBDown = false;

        //public TimeSpan TimeRemaining
        //{
        //    get { return timeRemaining; }
        //    set { timeRemaining = value; }
        //}
        //TimeSpan timeRemaining;
        int levelnumber = 0;

        //--- Exit to next level control
        public bool ReachedExit
        {
            get { return reachedExit; }
            set { reachedExit = value; }
        }
        bool reachedExit;
        public int NextLevel
        {
            get { return nextLevel; }
            set { nextLevel = value; }
        }
        int nextLevel;
        public int ExitNumber
        {
            get { return exitNumber; }
            set { exitNumber = value; }
        }
        int exitNumber;
        //---

        public bool AllCoinsCollected
        {
            get { return allCoinsCollected; }
            set { allCoinsCollected = value; }
        }
        bool allCoinsCollected;

        public bool CoinsRequired
        {
            get { return coinsRequired; }
            set { coinsRequired = value; }
        }
        bool coinsRequired;

        public int CoinsToCollect
        {
            get { return coinsToCollect; }
            set { coinsToCollect = value; }
        }
        int coinsToCollect;

        public int CoinsRemaining
        {
            get { return coinsRemaining; }
            set { coinsRemaining = value; }
        }
        int coinsRemaining;
        bool customCoinsRequired;
//        int timerline = 0;
        bool metaisset = false, metatitleisset = false, metadescriptionisset = false, metaCoinReqIsSet = false; //metatimerisset = false, *** Timer
        public bool MetaIsSet
        {
            get { return metaisset; }
        }
        public bool MetaDescriptionIsSet
        {
            get { return metadescriptionisset; }
        }
        public bool MetaTitleIsSet
        {
            get { return metatitleisset; }
        }
        //public bool MetaTimerIsSet
        //{
        //    get { return metatimerisset; }
        //}
        public bool MetaCoinReqIsSet
        {
            get { return metaCoinReqIsSet; }
        }
        
        //public TimeSpan LevelTimer
        //{
        //    get { return levelTimer; }
        //    set { levelTimer = value; }
        //}
        //TimeSpan levelTimer;

        public string LevelTitle
        {
            get { return levelTitle; }
            set { levelTitle = value; }
        }
        string levelTitle;

        public string LevelDescription
        {
            get { return levelDescription; }
            set { levelDescription = value; }
        }
        string levelDescription;

        private const int PointsPerSecond = 5;

        public Vector2 Checkpoint;
        public Vector2 bossStartPosition;

#region special items control
        // Magic mirror and the cloned player
        public Vector2 magicMirrorPosition;
        Player magicMirrorPlayerClone = null;
        // controls if the screen is under effect of upsidedown mirror
        public Boolean isUpsideDown = false; 
#endregion

        public bool StaticContentUpdated = false;
        public ContentManager content;
        private SoundEffect exitReachedSound;

        int StallFPSTime = 0;

        #region Loading

        /// <summary>
        /// Constructs a new level.
        /// </summary>
        /// <param name="serviceProvider">
        /// The service provider that will be used to construct a ContentManager.
        /// </param>
        /// <param name="path">
        /// The absolute path to the level file to be loaded.
        /// </param>
        public Level(IServiceProvider serviceProvider, string path, int levelNumber, int entranceNumber, ScreenManager myscreenmanager)
        {
            screenManager = myscreenmanager;

            // Create a new content manager to load content used just by this level.
            content = new ContentManager(serviceProvider, "GameContent");
            //timeRemaining = TimeSpan.FromMinutes(mytime);

            levelnumber = levelNumber;
            try
            {
                LoadTilesFromFile(path);
            }
            catch
            {
                screenManager.loadGameContent();
                LoadTilesFromFile(path);
            }

            Boolean playerPositionFound = false;
            for (int i = 0; i < entrances.Count; ++i)
            {
                //  Creates the player at the corresponding entrance number
                if (entrances[i].EntranceNumber == entranceNumber)
                {
                    // There is only one player object, created when the first level is loaded
                    if (screenManager.Player == null)
                        screenManager.Player = new Player(screenManager, this, entrances[i].Position, false);
                    else
                    {
                        screenManager.Player.Reset(entrances[i].Position, this);
                    }
                    start = entrances[i].Position;
                    playerPositionFound = true;
                }
            }
            if(!playerPositionFound)
                throw new Exception(String.Format("Invalid player entrance code {0} at level {1}.", entranceNumber, levelNumber));
           
            layers = new Layer[3];
            LoadLayers();
            //Load Sounds
            exitReachedSound = screenManager.ExitReachedSound;
        }

        public void LoadLayers()
        {
            try
            {
                //  Load level specific backgrounds, if any
                if(layerNames[0] != null)
                    layers[0] = new Layer(0, 0.2f, screenManager, content, layerNames[0]);
                else
                    layers[0] = new Layer(0, 0.2f, screenManager);
                if (layerNames[1] != null)
                    layers[1] = new Layer(1, 0.5f, screenManager, content, layerNames[1]);
                else
                    layers[1] = new Layer(1, 0.5f, screenManager);
                if (layerNames[2] != null)
                    layers[2] = new Layer(2, 0.8f, screenManager, content, layerNames[2]);
                else
                    layers[2] = new Layer(2, 0.8f, screenManager);
            }
            catch
            {
                // Fail gracefully
            }
        }

        /// <summary>
        /// Iterates over every tile in the structure file and loads its
        /// appearance and behavior. This method also validates that the
        /// file is well-formed with a player start point, exit, etc.
        /// </summary>
        /// <param name="path">
        /// The absolute path to the level file to be loaded.
        /// </param>
        private void LoadTilesFromFile(string path)
        {
            // Load the level and ensure all of the lines are the same length.
            int width;
            int numboflines = 0;
            List<string> lines = new List<string>();
            Boolean hasEntrance = false;
            Boolean hasExit = false;
            using (StreamReader reader = new StreamReader(path))
            {
                string line = reader.ReadLine();
                width = line.Length;
                String tempLine = line;
                // Calculates line lenght, desconsidering item codes
                for (int x = 0; x < width; ++x)
                 {
                     try
                     {
                         String tileType = tempLine.Substring(x, 1);
                         // if it's an special tile, passes control codes to LoadTile
                         switch (tileType)
                         {
                             case "X": //Exits passes the exit number and next level (4 extra chars) 
                                 tempLine = tempLine.Substring(0, x) + tempLine.Substring(x + 5);
                                 break;
                             case "x":  // Entrances passes the entrance number (1 extra chars) 
                                 tempLine = tempLine.Substring(0, x) + tempLine.Substring(x + 2);
                                 break;
                             case "E":
                                 tempLine = tempLine.Substring(0, x) + tempLine.Substring(x + 6);
                                 break;
                         }
                     }
                     catch { }
                }
                 width = tempLine.Length;
                //----------------------------------
                while (line != null)
                {
                    lines.Add(line);
                    if (line.Length != width)
                    {
                        if (line.StartsWith("@"))
                        {
                            //if (line.StartsWith("@tim.")) // *** Timer
                            //{
                            //    string mytimerstring = line.Remove(0, 5).ToString();
                            //    int thistimer = Int32.Parse(mytimerstring);
                            //    //timeRemaining = TimeSpan.FromMinutes(thistimer);
                            //    LevelTimer = TimeSpan.FromMinutes(thistimer);
                            //    timerline = numboflines;
                            //    if (!metaisset)
                            //        metaisset = true;
                            //    if (!metatimerisset)
                            //        metatimerisset = true;
                            //}
                            //else
                            if (line.StartsWith("@tit."))
                            {
                                string mytitlestring = line.Remove(0, 5).ToString();
                                LevelTitle = mytitlestring;
                                if (!metaisset)
                                    metaisset = true;
                                if (!metatitleisset)
                                    metatitleisset = true;
                            }
                            else
                                if (line.StartsWith("@des."))
                                {
                                    string mydescriptionstring = line.Remove(0, 5).ToString();
                                    LevelDescription = mydescriptionstring;
                                    if (!metaisset)
                                        metaisset = true;
                                    if (!metadescriptionisset)
                                        metadescriptionisset = true;
                                }
                                else
                                    if (line.StartsWith("@Layer0."))
                                    {
                                        string levelLayers = line.Remove(0, 8).ToString();
                                        layerNames[0] = levelLayers.Split('.');
                                    }
                                    else
                                        if (line.StartsWith("@Layer1."))
                                        {
                                            string levelLayers = line.Remove(0, 8).ToString();
                                            layerNames[1] = levelLayers.Split('.');
                                        }
                                        else
                                            if (line.StartsWith("@Layer2."))
                                            {
                                                string levelLayers = line.Remove(0, 8).ToString();
                                                layerNames[2] = levelLayers.Split('.');
                                            }
                                            else
                                                if (line.StartsWith("@remcoin."))
                                                {
                                                    if (!metaisset)
                                                        metaisset = true;
                                                    if (!metaCoinReqIsSet)
                                                        metaCoinReqIsSet = true;
                                                    if (line.StartsWith("@remcoin.0"))
                                                    {
                                                        coinsRequired = false;
                                                        customCoinsRequired = false;
                                                    }
                                                    else if (line.StartsWith("@remcoin.-1"))
                                                    {
                                                        coinsRequired = true;
                                                        customCoinsRequired = false;
                                                    }
                                                    else
                                                    {
                                                        coinsRequired = true;
                                                        customCoinsRequired = true;
                                                        string myCoinString = line.Remove(0, 8).ToString();
                                                        coinsToCollect = Int32.Parse(myCoinString);
                                                    }
                                                }
                        }
                        else
                        {
                            // Allow lines with different lenght due to Exit Codes - 4 more chars per exit ('X') on the line
                            // And to allow nullable tiles which will not ocupy a position on the tile array but will create items
                            // in the game, such as exits, player, collectable items, etc.
                         //   if(!line.Contains("X"))
                         //       throw new Exception(String.Format("The length of line {0} is different from all preceeding lines.", lines.Count));
                        }
                    }
                    line = reader.ReadLine();
                    numboflines++;
                }
            }

            // Allocate the tile grid.
            int metanumberset = 0;
            if (metadescriptionisset)
                metanumberset += 1;
  //          if (metatimerisset)
    //            metanumberset += 1;
            if (metatitleisset)
                metanumberset += 1;
            if (metaCoinReqIsSet)
                metanumberset += 1;
            tiles = new Tile[width, lines.Count - metanumberset];

            // Loop over every tile position,
            for (int y = 0; y < Height; ++y)
            {
                if (lines[y][0] != '@') // If it's not a control line
                {
                    for (int x = 0; x < Width; ++x)
                    {
                        Tile? newTile;
                        // to load each tile.
                        char tileType = lines[y][x];
                        // if it's an special tile, passes control codes to LoadTile
                        switch(tileType)
                        {
                            case 'X': //Exits passes the exit number and next level (4 extra chars) 
                                newTile = LoadTile(tileType, x, y, lines[y].Substring(x + 1, 4));
                                // Remove the exit code from the line (we don't want to process it like a tile code...)
                                lines[y] = lines[y].Substring(0, x) + lines[y].Substring(x + 5);
                                x--; //  since we did not create a tile, read the next one at the same array position
                                hasExit = true;
                                break;
                            case 'x':  // Entrances passes the entrance number (1 extra chars) 
                                newTile = LoadTile(tileType, x, y, lines[y].Substring(x + 1, 1));
                                // Remove the exit code from the line (we don't want to process it like a tile code...)
                                lines[y] = lines[y].Substring(0, x) + lines[y].Substring(x + 2);
                                x--; //  since we did not create a tile, read the next one at the same array position
                                hasEntrance = true;
                                break;
                            case 'E':   // Enemies passes the enemy type ("G"host, "M"onster or "F"lying),
                                        //  number (2 digits) and contact damage (2 digits)(total 5 extra chars)
                                newTile = LoadTile(tileType, x, y, lines[y].Substring(x + 1, 5));
                                // Remove the exit code from the line (we don't want to process it like a tile code...)
                                lines[y] = lines[y].Substring(0, x) + lines[y].Substring(x + 6);
                                x--; //  since we did not create a tile, read the next one at the same array position
                                break;
                            default:
                                newTile = LoadTile(tileType, x, y, "");
                                if (newTile.HasValue)
                                    tiles[x, y] = (Tile)newTile;
                                else
                                {
                                    lines[y] = lines[y].Substring(0, x) + lines[y].Substring(x + 1);
                                    x--; //  If we did not create a tile, read the next one at the same array position
                                }
                                break;
                        }
                    }
                // If after removing all codes and extras in line it still is greater than height,
                //  there's an error in the level file 
                if(lines[y].Length > Width)
                    throw new Exception(String.Format("The length ({1}) of line {0} is different from the lenght of all preceeding lines ({2}). \n Line content: ({3})", y+1, lines[y].Length, Width, lines[y] ));
                }
            }
            if (coinsRequired && !customCoinsRequired)
                coinsRemaining = coinsToCollect;
            if (coinsRequired && customCoinsRequired)
                coinsRemaining = coinsToCollect;

            if (coinsToCollect > items.Count)
                throw new Exception("There are not enough coins to fit the number required to pass the level.");

            //if (metatimerisset && levelTimer < TimeSpan.FromSeconds(10))
            //{
            //    //TimeRemaining = TimeSpan.FromMinutes(2.0);
            //    levelTimer = TimeSpan.FromMinutes(2.0);
            //} // *** Timer

            //if (!metatimerisset)
            //{
            //    //TimeRemaining = TimeSpan.FromMinutes(2.0);
            //    levelTimer = TimeSpan.FromMinutes(2.0);
            //}  // *** Timer

            if (levelTitle == null)
            {
                levelTitle = levelnumber.ToString();
            }

            if (levelDescription == null)
            {
                levelDescription = "Explore the level looking for the exit.";
            }


            // Verify that the level has a beginning and an end.
            if (!hasEntrance)
                throw new NotSupportedException("A level must have a starting point.");
            if (!hasExit)
                if(!screenManager.IsDemo)
                    throw new NotSupportedException("A level must have an exit.");   
   
        }


        public void ChangeTileCollision(int x, int y, TileCollision collision)
        {
            tiles[x, y].Collision = collision;
        }
        /// <summary>
        /// Loads an individual tile's appearance and behavior.
        /// </summary>
        /// <param name="tileType">
        /// The character loaded from the structure file which
        /// indicates what should be loaded.
        /// </param>
        /// <param name="x">
        /// The X location of this tile in tile space.
        /// </param>
        /// <param name="y">
        /// The Y location of this tile in tile space.
        /// </param>
        ///  <param name="code">
        /// Extra info used on tile creation (such as in exits) 
        /// </param>
        /// <returns>The loaded tile.</returns>
        private Tile? LoadTile(char tileType, int x, int y, String code)
        {
            switch (tileType)
            {
                //  Terrain - Mud
                case '2':
                    return LoadVarietyTile("Mud", 5, TileCollision.Impassable, TileType.Brick);
                //  Terrain - Mud Surface
                case '3':
                    return LoadVarietyTile("MudSurface", 3, TileCollision.Impassable, TileType.Brick);
                //  Terrain - Mud Ceiling
                case '4':
                    return LoadVarietyTile("MudCeiling", 3, TileCollision.Impassable, TileType.Transparent);
                //  Terrain - Clay
                case '5':
                    return LoadVarietyTile("Clay", 5, TileCollision.Impassable, TileType.Brick);
                //  Terrain - Clay Surface
                case '6':
                    return LoadVarietyTile("ClaySurface", 3, TileCollision.Impassable, TileType.Brick);
                //  Terrain - Clay Ceiling
                case '7':
                    return LoadVarietyTile("ClayCeiling", 3, TileCollision.Impassable, TileType.Transparent);
                //  Terrain - Rock
                case '8':
                    return LoadVarietyTile("Rock", 5, TileCollision.Impassable, TileType.Brick);
                //  Terrain - Rock Surface
                case '9':
                    return LoadVarietyTile("RockSurface", 3, TileCollision.Impassable, TileType.Brick);
                //  Terrain - Rock Ceiling
                case '0':
                    return LoadVarietyTile("RockCeiling", 3, TileCollision.Impassable, TileType.Transparent);
                case 'B':
                    return LoadMultipleStateItem(x, y, MultipleStateItemType.BlueDoor);
                case 'b':
                    return LoadItem(x, y, ItemType.BlueKey);
                case 'C':
                    return LoadItem(x, y, ItemType.Coin);
                case 'd':
                    return LoadItem(x, y, ItemType.MagicMirror);  // magic duplicating mirror
                case 'D':
                    return LoadMagicMirrorSpot(x, y);  // magic duplicating mirror cloning spot
                // Various enemies
                case 'E':
                    return LoadEnemy(x, y, code);
                case 'F':
                    return LoadFallingTile(x, y);
                case 'G':
                    return LoadMultipleStateItem(x, y, MultipleStateItemType.GreenDoor);
                case 'g':
                    return LoadItem(x, y, ItemType.GreenKey);
                case 'H':
                    return LoadHiddenTile(x, y, TileCollision.Impassable);
                // Rope
                case 'I':
                    return LoadTile(screenManager.RopeTexture, TileCollision.Ladder, TileType.Other);
                case 'i':
                    return LoadMultipleStateItem(x, y, MultipleStateItemType.Lever);
                case 'h':
                    return LoadHiddenTile(x, y, TileCollision.Platform);
                case 'J':
                    return LoadBossJumpTile(x, y);
                // Ladder 
                case 'L':
                    return LoadTile(screenManager.LadderTexture, TileCollision.Ladder, TileType.Brick);
                case 'l':
                    return LoadItem(x, y, ItemType.Life);
                // Moveable Tile
                case 'M':
                    return LoadMovingItem(screenManager.MoveablePlatformTexture, x, y, TileCollision.Platform);
                case 'm':
                    return LoadItem(x, y, ItemType.Map);
                case 'N':
                    return LoadAnimatedItem(x, y, AnimatedItemType.Candle);
                case 'O':
                    return LoadAnimatedItem(x, y, AnimatedItemType.Torch);
                case 'o':
                    return LoadItem(x, y, ItemType.Oxygen);
                case 'P':
                    return LoadItem(x, y, ItemType.Powerup);
                case 'R':
                    return LoadMultipleStateItem(x, y, MultipleStateItemType.RedDoor);
                case 'r':
                    return LoadItem(x, y, ItemType.RedKey);
                case 's':
                    return LoadSpring(x, y);
                case 'S':
                    return LoadBossTile(x, y);
                case 'T':
                    return LoadItem(x, y, ItemType.Trunk);
                case 'u':
                    return LoadAnimatedItem(x, y, AnimatedItemType.UpsideDownSpell);
                case 'V':
                    return LoadVanishingTile(x, y);
                case 'W':
                    return LoadAnimatedItem(x, y, AnimatedItemType.WaterSurface);
                case 'w':
                    return LoadTile(screenManager.Water, TileCollision.Passable, TileType.Water);
                case 'X':   // Exit
                    return LoadExitTile(x, y, code);
                case 'x':   // Entrance
                    return LoadEntranceTile(x, y, code);
                case 'Y':
                    return LoadMultipleStateItem(x, y, MultipleStateItemType.YellowDoor);
                case 'y':
                    return LoadItem(x, y, ItemType.YellowKey);
                case '+':
                    return LoadMultipleStateItem(x, y, MultipleStateItemType.HealthPotion);
                // Floating platform
                case '-':
                    return LoadVarietyTile("Platform", 1, TileCollision.Platform, TileType.Brick);
                case '*':
                    return LoadDeathTile("BlockA", x, y);
                // Blank space
                case '.':
                    return new Tile(null, TileCollision.Passable, TileType.Other, screenManager);
                // Brick (background)
                case ',':
                    return new Tile(screenManager.Brick, TileCollision.Passable, TileType.Other, screenManager);
                // Brick (Impassable)
                case ';':
                    return new Tile(screenManager.Brick, TileCollision.Impassable, TileType.Other, screenManager);
                // Impassable Blank space
                case '!':
                    return new Tile(null, TileCollision.Impassable, TileType.Brick, screenManager);
                // Platform block
                case '~':
                    return LoadVarietyTile("BlockB", 2, TileCollision.Platform, TileType.Brick);
                // Passable block
                case ':':
                    return LoadVarietyTile("BlockB", 2, TileCollision.Passable, TileType.Brick);
                // Impassable block
                case '#':
                    return LoadVarietyTile("BlockA", 7, TileCollision.Impassable, TileType.Brick);
                // Wood block
                case '=':
                    return LoadTile(screenManager.WoodBlockTexture, TileCollision.Impassable, TileType.Wood);

                // Unknown tile type character
                default:
                    throw new NotSupportedException(String.Format("Unsupported tile type character '{0}' at position {1}, {2}.", tileType, x, y));

            }

        }

        private Tile LoadBossTurnTile(int x, int y)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Creates a new tile. The other tile loading methods typically chain to this
        /// method after performing their special logic.
        /// </summary>
        /// <param name="name">
        /// Path to a tile texture relative to the Content/Tiles directory.
        /// </param>
        /// <param name="collision">
        /// The tile collision type for the new tile.
        /// </param>
        /// <returns>The new tile.</returns>
        private Tile LoadTile(Texture2D name, TileCollision collision, TileType type)
        {
            try
            {
                if (screenManager.SkinSettings.hasTiles)
                    return new Tile(name, collision, type,  screenManager);
                else
                    return new Tile(name, collision, type, screenManager);
            }
            catch
            {
                return new Tile(name, collision, type, screenManager);
            }
        }


        /// <summary>
        /// Loads a tile with a random appearance.
        /// </summary>
        /// <param name="baseName">
        /// The content name prefix for this group of tile variations. Tile groups are
        /// name LikeThis0.png and LikeThis1.png and LikeThis2.png.
        /// </param>
        /// <param name="variationCount">
        /// The number of variations in this group.
        /// </param>
        private Tile LoadVarietyTile(string baseName, int variationCount, TileCollision collision, TileType type)
        {
            int index = random.Next(variationCount);

            switch (baseName)
            {
                // Bricks and platforms
                case "BlockA":
                     return LoadTile(screenManager.BlockATexture[index], collision, type);
                case "BlockB":
                    return LoadTile(screenManager.BlockBTexture[index], collision, type);
                case "Platform":
                    return LoadTile(screenManager.PlatformTexture, collision, type);
                // Terrain
                case "Mud":
                    return LoadTile(screenManager.MudTerrain[index], collision, type);
                case "MudSurface":
                    return LoadTile(screenManager.MudSurface[index], collision, type);
                case "MudCeiling":
                    return LoadTile(screenManager.MudCeiling[index], collision, type);
                case "Clay":
                    return LoadTile(screenManager.ClayTerrain[index], collision, type);
                case "ClaySurface":
                    return LoadTile(screenManager.ClaySurface[index], collision, type);
                case "ClayCeiling":
                    return LoadTile(screenManager.ClayCeiling[index], collision, type);
                case "Rock":
                    return LoadTile(screenManager.RockTerrain[index], collision, type);
                case "RockSurface":
                    return LoadTile(screenManager.RockSurface[index], collision, type);
                case "RockCeiling":
                    return LoadTile(screenManager.RockCeiling[index], collision, type);
                default:
                    return LoadTile(screenManager.PlatformTexture, collision, type);
            }
        }

        /// <summary>
        /// Instantiates an exit and puts it in the level.
        /// </summary>
        private Tile? LoadExitTile(int x, int y, String code)
        {
            Point position = GetBounds(x, y).Center;
            int nextLevel = Convert.ToInt32(code.Substring(1));
            int exitNumber = Convert.ToInt32(code.Substring(0, 1));
            exits.Add(new Exit(screenManager, this, new Vector2(position.X, position.Y), exitNumber, nextLevel));
            return null;
        }

        /// <summary>
        /// Instantiates an entrance and puts it in the level.
        /// </summary>
        private Tile? LoadEntranceTile(int x, int y, String code)
        {
            Point position = GetBounds(x, y).Center;
            int entranceNumber = Convert.ToInt32(code.Substring(0, 1));
            entrances.Add(new Entrance(new Vector2(position.X, position.Y), entranceNumber));
            return null;
        }
        /// <summary>
        /// Instantiates a vanishing tile and puts it in the level.
        /// </summary>
        private Tile LoadVanishingTile(int x, int y)
        {
            Point position = GetBounds(x, y).Center;
            VanishingTiles.Add(new VanishingTile(screenManager, this, new Vector2(position.X, position.Y)));

            return new Tile(null, TileCollision.Passable, TileType.Brick, screenManager);
        }

        /// <summary>
        /// Instantiates a jumping tile and puts it in the level.
        /// </summary>
        private Tile? LoadBossJumpTile(int x, int y)
        {
            Point position = GetBounds(x, y).Center;
            BossJumpTiles.Add(new BossJumpTile(this, new Vector2(position.X, position.Y)));

            return null;
        }

        /// <summary>
        /// Instantiates a hidden tile and puts it in the level.
        /// </summary>
        private Tile LoadHiddenTile(int x, int y, TileCollision Collision)
        {
            Point position = GetBounds(x, y).Center;
            HiddenTiles.Add(new HiddenTile(screenManager, this, new Vector2(position.X, position.Y), "BlockA"));

            return new Tile(null, Collision, TileType.Brick, screenManager);
        }

        /// <summary>
        /// Instantiates a spring and puts it in the level.
        /// </summary>
        private Tile LoadSpring(int x, int y)
        {
            Point position = GetBounds(x, y).Center;
            Springs.Add(new Spring(screenManager, this, new Vector2(position.X, position.Y)));

            return new Tile(null, TileCollision.Impassable, TileType.Brick, screenManager);  
        }

        /// <summary>
        /// Instantiates an enemy and puts him in the level.
        /// </summary>
        private Tile? LoadEnemy(int x, int y, string code)
        {
            char enemyType = Convert.ToChar(code.Substring(0, 1));
            int enemyNumber;
            int contactDamage;
            try
            {
                enemyNumber = Convert.ToInt32(code.Substring(1, 2));
            }
            catch
            {
                throw new Exception(String.Format("Invalid enemy number: {0}.", code.Substring(1, 2)));
            }
            try
            {
                contactDamage = Convert.ToInt32(code.Substring(3, 2));
            }
            catch
            {
                throw new Exception(String.Format("Invalid enemy contact damage: {0}.", code.Substring(3, 2)));
            }

           Vector2 position = RectangleExtensions.GetBottomCenter(GetBounds(x, y));
           switch (enemyType) // Each enemy has its own code...
            {
                case 'G':
                    enemies.Add(new Enemy(screenManager, this, position, EnemyType.Ghost, enemyNumber, contactDamage));
                    break;
                case 'M':
                    enemies.Add(new Enemy(screenManager, this, position, EnemyType.Monster, enemyNumber, contactDamage));
                    break;
                case 'F':
                    enemies.Add(new Enemy(screenManager, this, position, EnemyType.Flying, enemyNumber, contactDamage));
                    break;
                default:
                    throw new Exception(String.Format("Invalid enemy type {0}.", enemyType));
            }
            return null;
        }

        /// <summary>
        /// Instantiates a boss and puts him in the level - ONLY ONE PER LEVEL
        /// </summary>
        private Tile LoadBossTile(int x, int y)
        {
            Vector2 position = RectangleExtensions.GetBottomCenter(GetBounds(x, y));
            if (bossStartPosition == null)
                bossStartPosition = position;
            bosses.Add(new Boss(screenManager, this, position));
            boss = new Boss(screenManager, this, position);

            return new Tile(null, TileCollision.Passable, TileType.Other, screenManager);
        }

        /// <summary>
        /// Instantiates a magic mirror destination (cloning) spot - ONLY ONE PER LEVEL
        /// </summary>
        private Tile? LoadMagicMirrorSpot(int x, int y)
        {
            Point position = GetBounds(x, y).Center;
            magicMirrorPosition = new Vector2(position.X, position.Y);

            return null;
        }
        private Tile LoadDeathTile(string baseName, int x, int y)
        {
            Point position = GetBounds(x, y).Center;
            DeathTiles.Add(new DeathTile(screenManager, this, new Vector2(position.X, position.Y), baseName));
            return new Tile(null, TileCollision.Impassable, TileType.Brick, screenManager);
        }

        public Tile? LoadMovingItem(Texture2D texture, int x, int y, TileCollision collision)
        {
            Point position = GetBounds(x, y).Center;//new Vector2(texture.Width / 2.0f, texture.Height / 2.0f);
            MovingItems.Add(new MovingItem(texture, screenManager, this, new Vector2(position.X, position.Y)));
            return null;
        }

        private Tile? LoadFallingTile(int x, int y)
        {
            Point position = GetBounds(x, y).Center;
            FallingTiles.Add(new FallingTile(screenManager, this, new Vector2(position.X, position.Y)));
            return null;
        }

        /// <summary>
        /// Instantiates a item and puts it in the level.
        /// </summary>
        private Tile? LoadItem(int x, int y, ItemType itemObject)
        {
            Point position = GetBounds(x, y).Center;
            items.Add(new Item(screenManager, this, new Vector2(position.X, position.Y), itemObject));
            if (itemObject == ItemType.Coin)
            {
                if (coinsRequired && !customCoinsRequired)
                    coinsToCollect += 1;
            }
            return null;
        }

        private Tile? LoadAnimatedItem(int x, int y, AnimatedItemType itemObject)
        {
            Point position = GetBounds(x, y).Center;
            animatedItems.Add(new AnimatedItem(screenManager, this, new Vector2(position.X, position.Y), itemObject, false));
            return null;
        }

        private Tile LoadMultipleStateItem(int x, int y, MultipleStateItemType itemObject)
        {
            multipleStateItems.Add(new MultipleStateItem(screenManager, this, x, y, itemObject));

            switch (itemObject)
            {
                case MultipleStateItemType.YellowDoor:
                case MultipleStateItemType.RedDoor:
                case MultipleStateItemType.GreenDoor:
                case MultipleStateItemType.BlueDoor:
                    return new Tile(null, TileCollision.Impassable, TileType.Other, screenManager);
                case MultipleStateItemType.Lever:
                default:
                    return new Tile(null, TileCollision.Passable, TileType.Other, screenManager);
            }
            
        }

        /// <summary>
        /// Unloads the level content.
        /// </summary>
        public void Dispose()
        {
            content.Unload();
        }

        #endregion

        #region Bounds and collision

        /// <summary>
        /// Gets the collision mode of the tile at a particular location.
        /// This method handles tiles outside of the levels boundries by making it
        /// impossible to escape past the left or right edges, but allowing things
        /// to jump beyond the top of the level and fall off the bottom.
        /// </summary>
        public TileCollision GetCollision(int x, int y)
        {
            // Prevent escaping past the level ends.
            if (x < 0 || x >= Width)
                return TileCollision.Impassable;
            // Allow jumping past the level top and falling through the bottom.
            if (y < 0 || y >= Height)
                return TileCollision.Passable;

            return tiles[x, y].Collision;
        }

        /// <summary>
        /// Gets the collision mode of the tile behind the player.
        /// </summary>
        public TileCollision GetTileCollisionBehindPlayer(Vector2 playerPosition)
        {
            int x = (int)playerPosition.X / Tile.Width;
            int y = (int)(playerPosition.Y - 1) / Tile.Height;

            // prevent out of bounds array error
            if (x < 0 || x >= Width)
                return TileCollision.Impassable;
            if (y < 0 || y >= Height)
                return TileCollision.Passable;

            return tiles[x, y].Collision;
        }

        /// <summary>
        /// Gets the collision mode of the tile at a particular location.
        /// </summary>
        public TileCollision GetTileCollisionAtPosition(Vector2 Position)
        {
            int x = (int)Position.X / Tile.Width;
            int y = (int)(Position.Y) / Tile.Height;

            // prevent out of bounds array error
            if (x < 0 || x >= Width)
                return TileCollision.Impassable;
            if (y < 0 || y >= Height)
                return TileCollision.Passable;

            return tiles[x, y].Collision;
        }

        /// <summary>
        /// Gets the type of the tile at a particular location.
        /// </summary>
        public TileType GetTileTypeAtPosition(Vector2 tilePosition)
        {
            int x = (int)tilePosition.X / Tile.Width;
            int y = (int)(tilePosition.Y) / Tile.Height;

            // prevent out of bounds array error
            if (x < 0 || x >= Width)
                return TileType.Other;
            if (y < 1 || y >= Height)
                return TileType.Other;

            return tiles[x, y-1].Type;
        }

        /// <summary>
        /// Gets the bounding rectangle of a tile in world space.
        /// </summary>        
        public Rectangle GetBounds(int x, int y)
        {
            return new Rectangle(x * Tile.Width, y * Tile.Height, Tile.Width, Tile.Height);
        }

        /// <summary>
        /// Width of level measured in tiles.
        /// </summary>
        public int Width
        {
            get { return tiles.GetLength(0); }
        }

        /// <summary>
        /// Height of the level measured in tiles.
        /// </summary>
        public int Height
        {
            get { return tiles.GetLength(1); }
        }
        #endregion

        #region Update

        /// <summary>
        /// Updates all objects in the world, performs collision between them,
        /// and handles the time limit with scoring.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            // Pause while the player is dead or time is expired.
            if (!screenManager.Player.IsAlive)//|| TimeRemaining == TimeSpan.Zero)
            {
                // Still want to perform physics on the player.
                screenManager.Player.ApplyPhysics(gameTime);
            }
            //else if (ReachedExit)
            //{
            //    // Animate the time being converted into points.
            //        int seconds = (int)Math.Round(gameTime.ElapsedGameTime.TotalSeconds * 100.0f);
            //        seconds = Math.Min(seconds, (int)Math.Ceiling(TimeRemaining.TotalSeconds));
            //        timeRemaining -= TimeSpan.FromSeconds(seconds);
            //        score += seconds * PointsPerSecond;

            //}
            else
            {
            //    timeRemaining -= gameTime.ElapsedGameTime;
                screenManager.Player.Update(gameTime);
                if (magicMirrorPlayerClone != null)
                    magicMirrorPlayerClone.Update(gameTime);

                UpdateItems(gameTime);
                UpdateFallingTiles(gameTime);

                //By default it tries to update every item on the field every second.
                //Sometimes it is not required to necessarily update all items
                //As some of them are mere static items that stay still.
                //We have a bool that gets assigned once per level to tell if the items
                //Have already been updated, and if they have already had its first update
                //It will no longer update throughout the level, unless the updater
                //Has a function that is interactive, say if a block has to update
                //When touched by the player, it can still update if needed.
                if (!StaticContentUpdated)
                {
                    UpdateVanishingTiles(gameTime, true);
                    UpdateHiddenTiles(gameTime, true);
                    UpdateBossJumpTiles(gameTime, true);
                    UpdateExits(gameTime, true);
                    UpdateDeathTiles(gameTime, true);
                    UpdateSprings(gameTime, true);
                    UpdateMovingItems(gameTime);
                    StaticContentUpdated = true;
                }
                else
                {
                    UpdateVanishingTiles(gameTime, false);
                    UpdateHiddenTiles(gameTime, false);
                    UpdateBossJumpTiles(gameTime, false);
                    UpdateExits(gameTime, false);
                    UpdateDeathTiles(gameTime, true);
                    UpdateSprings(gameTime, true);
                    UpdateMovingItems(gameTime);
                    StaticContentUpdated = true;
                }

                UpdateArrows(gameTime);
                UpdateAnimatedItems(gameTime);
                UpdateMultipleStateItems(gameTime);
                UpdateBosses(gameTime);
                UpdateEnemies(gameTime);

                if (Keyboard.GetState().IsKeyDown(Keys.F) && !IsBDown)
                {
                    IsBDown = true;
                    IncreaseFPS();
                }
                if (!Keyboard.GetState().IsKeyDown(Keys.F) && IsBDown)
                    IsBDown = false;

                if (screenManager.FPS < 5)
                {
                    StallFPSTime += 1;
                    if (StallFPSTime >= 20)
                    {
                        IncreaseFPS();
                        StallFPSTime = 0;
                    }
                }
                // Falling off the bottom of the level kills the player.
                if (screenManager.Player.BoundingRectangle.Top >= Height * Tile.Height)
                    OnPlayerKilled(false);

                if (!allCoinsCollected && coinsRemaining == 0)
                    allCoinsCollected = true;
            }

            // Clamp the time remaining at zero.
            //if (timeRemaining < TimeSpan.Zero)
            //    timeRemaining = TimeSpan.Zero;
        }

        private void UpdateMovingItems(GameTime gameTime)
        {
            if (MovingItemsAreActive)
            {
                for (int i = 0; i < MovingItems.Count; ++i)
                {
                    MovingItem movingItem = MovingItems[i];
                    movingItem.Update(gameTime);

                    if (movingItem.PlayerIsOn)
                    {
                        //Make player move with tile if the player is on top of tile   
                        screenManager.Player.Position += movingItem.Velocity;
                    }
                }
            }
        }  

        public void IncreaseFPS()
        {
            int NumberRemoved = 0;
            int ResetTime = 0;

            while (true)
            {
                if (items.Count > 10)
                {
                    Random myrandom = new Random();
                    int mynumber = myrandom.Next(0, this.items.Count);

                    if (this.items[mynumber].ItemType == ItemType.Coin)
                    {
                        Trace.Write("Removed Coin #" + mynumber + " at Position: {" + this.items[mynumber].Position.X.ToString() + ", " + this.items[mynumber].Position.Y.ToString() + "} \n");
                        this.items.RemoveAt(mynumber);
                        if (this.coinsRequired)
                        {
                            if (this.coinsRemaining > 0)
                            {
                                coinsRemaining -= 1;
                            }

                        }
                        NumberRemoved += 1;
                        Trace.Write("Removed\n");
                    }
                    Trace.Write("Time To Remove Another One\n");
                }
                else
                {
                    if (NumberRemoved >= 1 || items.Count > 1)
                    {
                        ResetTime = 0;
                        break;
                    }
                }
                ResetTime += 1;
                if (ResetTime > 10)
                    break;
            }
        }

        /// <summary>
        /// Animates each item and checks to allows the player to collect them.
        /// </summary>
        private void UpdateItems(GameTime gameTime)
        {
            for (int i = 0; i < items.Count; ++i)
            {
                Item item = items[i];

                item.Update(gameTime);

                if (item.BoundingCircle.Intersects(screenManager.Player.BoundingRectangle))
                {
                    OnItemCollected(item, screenManager.Player, i);
                }
            }
        }

        /// <summary>
        /// Animates each item and checks to allows the player to collect them.
        /// </summary>
        private void UpdateAnimatedItems(GameTime gameTime)
        {
            for (int i = 0; i < animatedItems.Count; ++i)
            {
                AnimatedItem item = animatedItems[i];

                item.Update(gameTime);

                if (item.BoundingCircle.Intersects(screenManager.Player.BoundingRectangle))
                {
                    if(item.isCollectable)
                        OnAnimatedItemCollected(item, screenManager.Player, i);
                }
            }
        }

        /// <summary>
        /// Updates the multiple state items
        /// </summary>
        private void UpdateMultipleStateItems(GameTime gameTime)
        {
            for (int i = 0; i < multipleStateItems.Count; ++i)
            {
                MultipleStateItem item = multipleStateItems[i];

                if (magicMirrorPlayerClone != null)
                {
                    // also test collision with player clone (created by Magic Mirror)
                    if (item.BoundingRectangle.Intersects(screenManager.Player.BoundingRectangle) ||
                        item.BoundingRectangle.Intersects(this.magicMirrorPlayerClone.BoundingRectangle))
                        item.OnCollected(screenManager.Player, i);
                    else
                        item.OnNotCollected();
                }
                else
                {
                    if (item.BoundingRectangle.Intersects(screenManager.Player.BoundingRectangle))
                        item.OnCollected(screenManager.Player, i);
                    else
                        item.OnNotCollected();
                }
            }
        }
        /// <summary>
        /// Animates each arrow 
        /// </summary>
        private void UpdateArrows(GameTime gameTime)
        {
            for (int i = 0; i < arrows.Count; ++i)
            {
                Arrow arrow = arrows[i];

                arrow.Update(gameTime);

                try
                {                
                    int x, y; // arrow position converted to tiles array indexes
                   // Test collision with tiles (includes any impassable tiles such as springs, death tiles or hidden impasable tiles)
                    y = (int)arrow.Position.Y / Tile.Height;              
                    if(arrow.Direction == ArrowDirection.Left)
                        x = (int)(arrow.Position.X + arrow.Width*0.5f) / Tile.Width;
                    else
                        x = (int)(arrow.Position.X - arrow.Width*0.5f) / Tile.Width;
                    if (tiles[x, y].Collision == TileCollision.Impassable)
                        arrow.onTouched(tiles[x, y]);
                }
                catch
                {
                    // Ignore out of bounds array error (when arrow goes outside the screen but was not yet removed)
                }

                //  Test collision with animated itens 
                for (int j = 0; j < animatedItems.Count; ++j)
                {
                    if (arrow.BoundingRectangle.Intersects(animatedItems[j].BoundingRectangle))
                        arrow.onTouched(animatedItems[j]);
                }

                //  Test collision with multiple state itens 
                for (int j = 0; j < multipleStateItems.Count; ++j)
                {
                    if (arrow.BoundingRectangle.Intersects(multipleStateItems[j].BoundingRectangle))
                        arrow.onTouched(multipleStateItems[j]);
                    else
                        multipleStateItems[j].OnNotTouched();
                }

                // Test collision with itens 
                for (int j = 0; j < items.Count; ++j)
                {
                    if (arrow.BoundingRectangle.Intersects(items[j].BoundingRectangle))
                        arrow.onTouched(items[j]);
                }

                // Remove arrows when touching moveable tiles
                for (int j = 0; j < MovingItems.Count; ++j)
                {
                    if (arrow.BoundingRectangle.Intersects(MovingItems[j].BoundingRectangle))
                        arrow.onTouched(MovingItems[j]);
                }
            }
        }

        /// <summary>
        /// Checks if the player reached the exit
        /// </summary>
        private void UpdateExits(GameTime gameTime, bool updateEntireObject)
        {
            for (int i = 0; i < exits.Count; ++i)
            {
                if (exits[i].BoundingRectangle.Intersects(screenManager.Player.BoundingRectangle))
                {
                    OnExitReached(screenManager.Player, exits[i]);
                }
            }
        }

        /// <summary>
        /// Animates each boss 
        /// </summary>
        private void UpdateBosses(GameTime gameTime)
        {
            for (int i = 0; i < bosses.Count; ++i)
            {
                Boss boss = bosses[i];

                boss.Update(gameTime);

                if (boss.BoundingRectangle.Intersects(screenManager.Player.BoundingRectangle))
                {
                    OnPlayerKilled(true);
                }
            }
        }

        /// <summary>
        /// Animates each boss
        /// </summary>
        private void UpdateBossJumpTiles(GameTime gameTime, bool updateEntireObject)
        {
            for (int i = 0; i < BossJumpTiles.Count; ++i)
            {
                BossJumpTile bossJumpTile = BossJumpTiles[i];
                if (updateEntireObject)
                bossJumpTile.Update(gameTime);
            }
        }

        /// <summary>
        /// Update tiles that vanish when player touches them
        /// </summary>
        private void UpdateVanishingTiles(GameTime gameTime, bool updateEntireObject)
        {
            for (int i = 0; i < VanishingTiles.Count; ++i)
            {
                VanishingTile vanishingTile = VanishingTiles[i];
                if (updateEntireObject)
                vanishingTile.Update(gameTime);

                if (vanishingTile.BoundingRectangle.Intersects(screenManager.Player.BoundingRectangle))
                {
                    VanishingTiles.RemoveAt(i--);
                }
            }
        }

        /// <summary>
        /// Updates the hidden tiles
        /// </summary>
        private void UpdateHiddenTiles(GameTime gameTime, bool updateEntireObject)
        {
            for (int i = 0; i < HiddenTiles.Count; ++i)
            {
                HiddenTile hiddenTile = HiddenTiles[i];
                if (updateEntireObject)
                hiddenTile.Update(gameTime);

                if (hiddenTile.BoundingRectangle.Intersects(screenManager.Player.BoundingRectangle))
                {
                    hiddenTile.OnCollected();
                }
            }
        }

        /// <summary>
        /// Updates the springs.
        /// </summary>
        private void UpdateSprings(GameTime gameTime, bool updateEntireObject)
        {
             for (int i = 0; i < Springs.Count; ++i)
            {
                Spring spring = Springs[i];
                if (updateEntireObject)
                    spring.Update(gameTime);

                if (spring.PlayerIsOn)
                {
                    //Make player move with tile if the player is on top of tile   
                    screenManager.Player.DoSpringJump(gameTime);
                    spring.OnColide(screenManager.Player);
                }
            }
        }

        /// <summary>
        /// Updates  tiles that kill player when touched
        /// </summary>
        private void UpdateDeathTiles(GameTime gameTime, bool updateEntireObject)
        {
            for (int i = 0; i < DeathTiles.Count; ++i)
            {
                DeathTile deathTile = DeathTiles[i];
                if (updateEntireObject)
                deathTile.Update(gameTime);

                if (deathTile.BoundingRectangle.Intersects(screenManager.Player.BoundingRectangle))
                {
                    OnPlayerKilled(true);
                }
            }
        }

        private void UpdateFallingTiles(GameTime gameTime)
        {
            for (int i = 0; i < FallingTiles.Count; ++i)
            {
                FallingTile FallingTile = FallingTiles[i];
                FallingTile.Update(gameTime);
                if (FallingTile.BoundingCircle.Intersects(screenManager.Player.BoundingRectangle))
                {
                    if (!screenManager.Player.IsPoweredUp)
                        OnPlayerHurt(true, 1);
                }
                else if (FallingTile.TriggerRectangle.Intersects(screenManager.Player.BoundingRectangle))
                {
                    OnFallingTileFalling(FallingTile);
                }
                for (int e = 0; e < enemies.Count; ++e)
                {
                    if (FallingTile.BoundingCircle.Intersects(enemies[e].BoundingRectangle))
                    {
                        enemies[e].OnKilled();
                    }
                    else if (FallingTile.TriggerRectangle.Intersects(enemies[e].BoundingRectangle))
                    {
                        OnFallingTileFalling(FallingTile);
                    }
                }
            }
        }

        Enemy enemytodie;
        float enemydeathtime;
        /// <summary>
        /// Animates each enemy and allow them to kill the player.
        /// </summary>
        private void UpdateEnemies(GameTime gameTime)
        {
            try
            {
                foreach (Enemy enemy in enemies)
                {

                    if (!enemy.IsAlive)
                    {
                        if (enemytodie == null)
                            enemytodie = enemy;
                        else
                            if (enemytodie != enemy)
                                enemies.Remove(enemy);
                    }
                    else
                        enemy.Update(gameTime);
                    // Touching an enemy hurts the player. The Health is lower down according to the enemy's strenght defined on the level file
                    if (enemy.IsAlive && (enemy.BoundingRectangle.Intersects(screenManager.Player.BoundingRectangle)
                        || enemy.BoundingRectangle.Intersects(magicMirrorPlayerClone.BoundingRectangle)))
                    {
                        if (screenManager.Player.IsPoweredUp)
                        {
                            OnEnemyKilled(enemy);
                        }
                        else
                        {
                            OnPlayerHurt(enemy, true);
                        }
                    }
                    if (enemy.IsAlive && (enemy.BoundingRectangle.Intersects(screenManager.Player.MeleeRectangle)
                        || enemy.BoundingRectangle.Intersects(magicMirrorPlayerClone.MeleeRectangle)))
                    {
                        if (screenManager.Player.isAttacking)
                            OnEnemyKilled(enemy);
                    }

                    for (int i = 0; i < arrows.Count; ++i)
                    {
                        Arrow arrow = arrows[i];
                        if (arrow.BoundingRectangle.Intersects(enemy.BoundingRectangle))
                            onEnemyHitByArrow(enemy, i);
                    }
                }
            }
            catch { }

            if (enemytodie != null)
            {
                enemydeathtime++;
                if (enemydeathtime > 20.0f)
                {
                    enemies.Remove(enemytodie);
                    enemydeathtime = 0;
                    enemytodie = null;
                }
            }
        }
        

#endregion

        #region OnActions

        void onEnemyHitByArrow(Enemy enemy, int arrowNumber)
        {
            // Ghosts are not affected by arrows
            if(enemy.Type != EnemyType.Ghost)
                arrows[arrowNumber].onTouched(enemy);
        }


        void OnEnemyKilled(Enemy enemy)
        {
            enemy.OnKilled();
        }


        /// <summary>
        /// Called when a item is collected.
        /// </summary>
        /// <param name="item">The item that was collected.</param>
        /// <param name="collectedBy">The player who collected this item.</param>
        private void OnAnimatedItemCollected(AnimatedItem item, Player collectedBy, int number)
        {
            item.OnCollected(collectedBy, number);
        }

        /// <summary>
        /// Called when a item is collected.
        /// </summary>
        /// <param name="item">The item that was collected.</param>
        /// <param name="collectedBy">The player who collected this item.</param>
        private void OnItemCollected(Item item, Player collectedBy, int number)
        {
            item.OnCollected(collectedBy, number);
        }

        private void OnFallingTileFalling(FallingTile FallingTile)
        {
            FallingTile.OnFallingTileFalling();
        }
        
        /// <summary>
        /// Called when the player is killed.
        /// </summary>
        /// <param name="killedBy">
        /// The enemy who killed the player. This is null if the player was not killed by an
        /// enemy, such as when a player falls into a hole.
        /// </param>
        /// <param name="playKillSound">
        /// This tells weather to play the sound of being killed or the sound of falling. if
        /// this is true, the killed sound will play, if this is false, the fall sound will play.
        /// </param>
        private void OnPlayerHurt(Enemy hurtBy, bool playHurtSound)
        {
            screenManager.Player.Hurt(playHurtSound, hurtBy.ContactDamage);
        }

        /// <summary>
        /// Called when the player is instantly killed.
        /// </summary>
        /// <param name="playKillSound">
        /// This tells weather to play the sound of being killed or the sound of falling. if
        /// this is true, the killed sound will play, if this is false, the fall sound will play.
        /// </param>
        private void OnPlayerKilled(bool playKilledSound)
        {
            screenManager.Player.Kill(playKilledSound);
        }

        /// <summary>
        /// Called when the player is hurt.
        /// </summary>
        /// <param name="playKilledSound">
        /// This tells weather to play the sound of being killed or the sound of falling. if
        /// this is true, the killed sound will play, if this is false, the fall sound will play.
        /// </param>
        /// <param name="contactdamage">Amaount of damage to inflict upon Players Health.</param>
        private void OnPlayerHurt(bool playKilledSound, int contactdamage)
        {
            screenManager.Player.Hurt(playKilledSound, contactdamage);
        }

         public void onMagicMirrorPlayerCloned()
         {
             magicMirrorPlayerClone = new Player(screenManager, this, magicMirrorPosition, true);
         }
             
        /// <summary>
        /// Called when the player reaches the level's exit.
        /// </summary>
        private void OnExitReached(Player reachedBy, Exit exit)
        {
            if (reachedBy.IsAlive && (reachedBy.IsOnGround || reachedBy.IsClimbing))
            {
                if (coinsRequired)
                {
                    if (allCoinsCollected)
                    {
                        exit.OnReached(reachedBy); // informs the exit it was reached
                        reachedBy.OnReachedExit(); // informs the player it reached the exit
                        //exitReachedSound.Play(screenManager.Settings.SoundVolumeAmount, 0, 0);
                        reachedExit = true;
                    }
                }
                else
                {
                    exit.OnReached(reachedBy); // informs the exit it was reached
                    reachedBy.OnReachedExit(); // informs the player it reached the exit
                    //exitReachedSound.Play(screenManager.Settings.SoundVolumeAmount, 0, 0);
                    reachedExit = true;
                }
            }
        }

        /// <summary>
        /// Restores the player to the starting point to try the level again.
        /// </summary>
        public void StartNewLife()
        {
            if (screenManager.Player.Lives <= 0)
            {
                /////////////////////GO TO MAIN MENU///////////////////
                BackgroundScreen backgroundScreen = new BackgroundScreen();
                LoadingScreen.Load(screenManager, true, backgroundScreen,
                                                         new MainMenuScreen(screenManager));
            }
            else
            {
                if (Boss != null)
                    Boss.Position = bossStartPosition;
                if (Checkpoint != Vector2.Zero)
                    screenManager.Player.Reset(Checkpoint, this);
                else
                    screenManager.Player.Reset(start, this);
                screenManager.Player.Lives--;
            }
        }
        #endregion

        #region Draw
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Matrix cameraTransform = Matrix.Identity;

            // Draw layers
            for (int i = 0; i <= EntityLayer; ++i)
               layers[i].Draw(spriteBatch, cameraPosition.X);
                
            spriteBatch.End();

            ScrollCamera(spriteBatch.GraphicsDevice.Viewport);

            if (this.isUpsideDown)
            {
                cameraTransform *= Matrix.CreateRotationZ((float)Math.PI);
                cameraTransform *= Matrix.CreateTranslation(this.Width * Tile.Width , this.Height * Tile.Height, 0.0f);
                cameraTransform *= Matrix.CreateTranslation(- cameraPosition.X, -cameraPosition.Y, 0.0f);
                //  Matrix.CreateTranslation((this.Width * Tile.Width) , (this.Height * Tile.Height), 0.0f);
            } 
            else
                cameraTransform = Matrix.CreateTranslation(-cameraPosition.X, -cameraPosition.Y, 0.0f); 

            // If the player is under the upside down spell, modifies the screen accordingly
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None, cameraTransform);

            DrawTiles(spriteBatch);
            foreach (Item item in items)
                item.Draw(gameTime, spriteBatch);
            foreach (MultipleStateItem item in multipleStateItems)
                item.Draw(gameTime, spriteBatch);
            foreach (AnimatedItem item in animatedItems)
                item.Draw(gameTime, spriteBatch);
            foreach (FallingTile FallingTile in FallingTiles)
                FallingTile.Draw(gameTime, spriteBatch);
            foreach (Exit exit in exits)
                exit.Draw(gameTime, spriteBatch);
            foreach (Boss boss in bosses)
                boss.Draw(gameTime, spriteBatch);
            foreach (Arrow arrow in arrows)
                arrow.Draw(gameTime, spriteBatch);
            foreach (VanishingTile vanishingTile in VanishingTiles)
                vanishingTile.Draw(gameTime, spriteBatch);
            foreach (Spring spring in Springs)
                spring.Draw(gameTime, spriteBatch);
            foreach (HiddenTile hiddenTile in HiddenTiles)
                hiddenTile.Draw(gameTime, spriteBatch);
            foreach (BossJumpTile bossJumpTile in BossJumpTiles)
                bossJumpTile.Draw(gameTime, spriteBatch);
            foreach (DeathTile deathTile in DeathTiles)
               deathTile.Draw(gameTime, spriteBatch, !screenManager.isRunningSlow);
            foreach (MovingItem movingItem in MovingItems)
                movingItem.Draw(gameTime, spriteBatch);
            foreach (Enemy enemy in enemies)
                enemy.Draw(gameTime, spriteBatch);

            // Draw the Player on top of everything
            screenManager.Player.Draw(gameTime, spriteBatch, Color.White);
            // If there is a player clone (created with magic mirror), draw it
            if (magicMirrorPlayerClone != null)
                magicMirrorPlayerClone.Draw(gameTime, spriteBatch, new Color (Color.WhiteSmoke, 125));

            spriteBatch.End();

            //  Draw HUD
            spriteBatch.Begin();
                spriteBatch.Draw(screenManager.HUDTexture, Vector2.Zero, Color.White);
                // Show debug info, if in debug mode
                if (screenManager.Settings.DebugMode)
                {
                    Viewport viewport = spriteBatch.GraphicsDevice.Viewport;
                    // Write the camera position
                    string debugCameraPos = "Camera Pos: " + cameraPosition.ToString();
                    float lineHeight = screenManager.Font.MeasureString(debugCameraPos).Y;
                    spriteBatch.DrawString(screenManager.Font, debugCameraPos, 
                            new Vector2((viewport.Width - screenManager.Font.MeasureString(debugCameraPos).X) - 10, 
                                         viewport.Height - lineHeight * 2 - 10), Color.White);
                    string maxCameraX = "Max Camera X: " + (Tile.Width * Width - viewport.Width).ToString();
                    string maxCameraY = "Max Camera Y: " + (Tile.Height * (Height - 6) - (viewport.Height - ScreenManager.HUDHeight)).ToString();
                    spriteBatch.DrawString(screenManager.Font, maxCameraX,
                        new Vector2((viewport.Width - screenManager.Font.MeasureString(maxCameraX).X) - 10, 
                                         viewport.Height - lineHeight * 3 - 10), Color.White);
                    spriteBatch.DrawString(screenManager.Font, maxCameraY,
                        new Vector2((viewport.Width - screenManager.Font.MeasureString(maxCameraY).X) - 10,
                                         viewport.Height - lineHeight * 4 - 10), Color.White);

                    string playerPos = "Player Pos: " + screenManager.Player.Position.ToString();
                    spriteBatch.DrawString(screenManager.Font, playerPos,
                        new Vector2((viewport.Width - screenManager.Font.MeasureString(playerPos).X) - 10,
                                         viewport.Height - lineHeight * 5 - 10), Color.White);

                    
                    // Draw the edges of the vieweing part of the screen -> CODE REPEATED FROM ScrollCamera method!
                    //const float ViewMargin = 0.35f;
                    //float marginWidth = viewport.Width * ViewMargin;
                    //float marginHeight = viewport.Height * ViewMargin;

                    //float marginLeft = cameraPosition.X + marginWidth;
                    //float marginRight = cameraPosition.X + viewport.Width - marginWidth;
                    //float marginTop = cameraPosition.Y + marginHeight + ScreenManager.HUDHeight;
                    //float marginBottom = cameraPosition.Y + viewport.Height - marginHeight + ScreenManager.HUDHeight;

                    spriteBatch.Draw(screenManager.CrossHairTexture, cameraPosition, Color.White);
                    //spriteBatch.Draw(screenManager.CrossHairTexture, new Vector2(marginLeft, marginTop), Color.White);
                    //spriteBatch.Draw(screenManager.CrossHairTexture, new Vector2(marginRight, marginTop), Color.White);
                    //spriteBatch.Draw(screenManager.CrossHairTexture, new Vector2(marginLeft, marginBottom), Color.White);
                    //spriteBatch.Draw(screenManager.CrossHairTexture, new Vector2(marginRight, marginBottom), Color.White); 


                }
            spriteBatch.End();

            // Draw background layers
            spriteBatch.Begin();
            for (int i = EntityLayer + 1; i < layers.Length; ++i)
                layers[i].Draw(spriteBatch, cameraPosition.X);
            //spriteBatch.End();   //*** Don't blame me, the original code was already commented... And fails if the comment is removed!
        }

        /// <summary>
        /// Draws the camera and makes it scroll.
        /// </summary>
        private void ScrollCamera(Viewport viewport)
        {
            const float ViewMargin = 0.35f;
            Vector2 cameraMovement = Vector2.Zero;
            Vector2 maxCameraPosition = Vector2.Zero;
            Vector2 minCameraPosition = Vector2.Zero;

            // Calculate the edges of the screen.
            float marginWidth = viewport.Width * ViewMargin;
            float marginHeight = viewport.Height * ViewMargin;  
            float marginLeft = cameraPosition.X + marginWidth;
            float marginRight = cameraPosition.X + viewport.Width - marginWidth;
            float marginTop = cameraPosition.Y + marginHeight + ScreenManager.HUDHeight;  
            float marginBottom =(cameraPosition.Y + viewport.Height - marginHeight) + ScreenManager.HUDHeight ;

            //if (this.isUpsideDown)
            //{
            //    marginTop = cameraPosition.Y + marginHeight ;
            //    marginBottom= (cameraPosition.Y + viewport.Height - marginHeight) ;
            //}

            // Calculate how far to scroll when the player is near the edges of the screen.
            // X axis (horizontal)
            //if (this.isUpsideDown)
            //{
            //    if (screenManager.Player.Position.X < marginLeft)
            //        cameraMovement.X = screenManager.Player.Position.X - marginLeft;
            //    else if (screenManager.Player.Position.X > marginRight)
            //        cameraMovement.X = screenManager.Player.Position.X - marginRight;
            //}
            //else
            {
                if (screenManager.Player.Position.X < marginLeft)
                    cameraMovement.X = screenManager.Player.Position.X - marginLeft;
                else if (screenManager.Player.Position.X > marginRight)
                    cameraMovement.X = screenManager.Player.Position.X - marginRight;
            }
            // Y Axis (Vertical)
            //if (this.isUpsideDown)
            //{
            //    if (screenManager.Player.Position.Y < marginTop)// //above the top margin  
            //        cameraMovement.Y = screenManager.Player.Position.Y - marginTop;//(marginTop - ScreenManager.HUDHeight);
            //    else if (screenManager.Player.Position.Y > marginBottom) //below the bottom margin  
            //        cameraMovement.Y = screenManager.Player.Position.Y - marginBottom;
            //}
            //else
            {
                if (screenManager.Player.Position.Y < marginTop)// //above the top margin  
                    cameraMovement.Y = screenManager.Player.Position.Y - marginTop ;//
                else if (screenManager.Player.Position.Y > marginBottom) //below the bottom margin  
                    cameraMovement.Y = screenManager.Player.Position.Y - marginBottom ;
            }
            // Update the camera position, but prevent scrolling off the ends of the level.
            // X Axis (Horizontal)
           maxCameraPosition.X = Tile.Width * Width - viewport.Width;
           minCameraPosition.X = 0;
           if (this.isUpsideDown)
            {
            //    maxCameraPosition.X = Tile.Width * Width;
            //    minCameraPosition.X = viewport.Width;
                cameraPosition.X = cameraPosition.X - cameraMovement.X;
            }
            else
            {
                cameraPosition.X = cameraPosition.X + cameraMovement.X;
            }
            cameraPosition.X = MathHelper.Clamp(cameraPosition.X, minCameraPosition.X, maxCameraPosition.X);

            // Y Axis (Vertical)
            //if (this.isUpsideDown)
            //{
            //    maxCameraPosition.Y = Tile.Height * (Height - 6) - (viewport.Height - ScreenManager.HUDHeight) ;
            //    minCameraPosition.Y = -ScreenManager.HUDHeight + Tile.Height;
            //}
            //else
            {
                maxCameraPosition.Y = Tile.Height * (Height - 6) - (viewport.Height - ScreenManager.HUDHeight);
                minCameraPosition.Y = -ScreenManager.HUDHeight;
            }

            if (this.isUpsideDown)
            {
                 minCameraPosition.Y =  -ScreenManager.HUDHeight + Tile.Height;
                cameraPosition.Y = cameraPosition.Y - cameraMovement.Y;
            }
            else
            {
                cameraPosition.Y = cameraPosition.Y + cameraMovement.Y;
            }
            cameraPosition.Y = MathHelper.Clamp(cameraPosition.Y, minCameraPosition.Y, maxCameraPosition.Y);

        }
    



        /// <summary>
        /// Draws each tile in the level.
        /// </summary>
        private void DrawTiles(SpriteBatch spriteBatch)
        {
            int left = 0;
            int right = 0;

            // Calculate the visible range of tiles.
            if (isUpsideDown)
            {
                
                right = Width - (int)Math.Floor(cameraPosition.X / Tile.Width)+2 ;
                right = Math.Min(right, Width - 1);
                left = Math.Max(0,right - spriteBatch.GraphicsDevice.Viewport.Width / Tile.Width - 2);
              //  
            }
            else
            {
                left = Math.Max(0,(int)Math.Floor(cameraPosition.X / Tile.Width)-2);
                right = left + spriteBatch.GraphicsDevice.Viewport.Width / Tile.Width+2;
                right = Math.Min(right, Width - 1);
            }
            // For each tile position
            for (int y = 0; y < Height; ++y)
            {
                //for (int x = 0; x < Width; ++x)
                for (int x = left; x <= right; ++x)
                {
                    // If there is a visible tile in that position
                    Texture2D texture = tiles[x, y].Texture;
                    if (texture != null)
                    {
                        Vector2 position = new Vector2(x, y) * Tile.Size;
                        // When drawing ladders, repeat the next texture under the ladder
                        if(tiles[x, y].Collision == TileCollision.Ladder)
                            if (tiles[x + 1, y].Texture != null)
                                spriteBatch.Draw(tiles[x+1, y].Texture, position, Color.White);
                        // When drawing transparent tiles (such as the terrain bottom tiles = stalactites), repeat the texture bellow under the tile
                        //  used with any tile with transparent parts
                        if (tiles[x, y].Type == TileType.Transparent)
                        {
                            if (tiles[x, y + 1].Texture != null)
                            {
                                int numTiles = tiles[x, y].Texture.Width / Tile.Width;
                                for (int i = 0; i < numTiles; i++)
                                {
                                    spriteBatch.Draw(tiles[x, y + 1].Texture, new Vector2(position.X + i*Tile.Width, position.Y), Color.White);
                                }
                            }
                        }
                        // Draw it in screen space.
                        spriteBatch.Draw(texture, position, Color.White);
                        // If debugging, show bounding boxes for impassable tiles
                        if (screenManager.Settings.DebugMode)
                            if(tiles[x, y].Collision == TileCollision.Impassable)
                                spriteBatch.Draw(screenManager.BlankTexture, new Rectangle(x * Tile.Width, y * Tile.Height, Tile.Width, Tile.Height), new Color(255, 155, 0, 155));
                            else
                                spriteBatch.Draw(screenManager.BlankTexture, new Rectangle(x* Tile.Width, y * Tile.Height, Tile.Width, Tile.Height), new Color(0, 255, 0, 155)); 

                        //
                    }
                }
            }
        }

        #endregion

        internal void addArrow(Vector2 vector2, bool isfacingleft)
        {
            if (isfacingleft)
                arrows.Add(new Arrow(screenManager, this, vector2, ArrowDirection.Left));
            else
                arrows.Add(new Arrow(screenManager, this, vector2, ArrowDirection.Right));
        }
    }
}
