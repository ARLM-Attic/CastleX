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

namespace CastleX
{
    /// <summary>
    /// A uniform grid of tiles with collections of suport items.
    /// The level owns the player and controls the game's win and lose
    /// conditions as well as scoring.
    /// </summary>
    class LevelEditor : IDisposable
    {
        // Physical structure of the level.
        public LevelEditorTile[,] tiles;
        public Vector2 tilesAmount = Vector2.Zero;
        // The layer which entities are drawn on top of.
        private const int EntityLayer = 2;

        public ScreenManager screenManager;
        // Key locations in the level.       
        private static readonly Point InvalidPosition = new Point(-1, -1);

        // Level game state.
       // private float cameraPosition;

        private Random random = new Random(354668); // Arbitrary, but constant seed

        public Vector2 CursorPosition = Vector2.Zero;
        public Vector2 CursorLocation = Vector2.Zero;

        LevelEditorScreen levelEditor;
        public Vector2 MapPosition = Vector2.Zero;
        public ContentManager content;

        public Vector2 MapSize = Vector2.Zero;

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
        public LevelEditor(IServiceProvider serviceProvider, string path, ScreenManager myscreenmanager, LevelEditorScreen levelEditor)
        {
            this.levelEditor = levelEditor;
            screenManager = myscreenmanager;
            //loadGameContent();

            // Create a new content manager to load content used just by this level.
            content = new ContentManager(serviceProvider, "GameContent");
            //timeRemaining = TimeSpan.FromMinutes(mytime);

            try
            {
                LoadTiles(path);
            }
            catch
            {
                try
                {
                    screenManager.loadGameContent();
                    LoadTiles(path);
                }
                catch
                {
                    LoadTiles(path);
                }
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
        private void LoadTiles(string path)
        {
            // Load the level and ensure all of the lines are the same length.
            int width;
            List<string> lines = new List<string>();
            using (StreamReader reader = new StreamReader(path))
            {
                string line = reader.ReadLine();
                width = line.Length;
                while (line != null)
                {
                    lines.Add(line);
                    if (line.Length != width)
                    {
                        
                            throw new Exception(String.Format("The length of line {0} is different from all preceeding lines.", lines.Count));
                    
                    }

                    line = reader.ReadLine();
                }
            }

            tilesAmount.X = width;
            Trace.Write("Width: " + width.ToString() + "\n\n");

            // Allocate the tile grid.

            tiles = new LevelEditorTile[width, lines.Count];
            int numberofitems = 0;
            // Loop over every tile position,
            for (int y = 0; y < Height; ++y)
            {
                    for (int x = 0; x < Width; ++x)
                    {

                        // to load each tile.
                        char tileType = lines[y][x];
                        tiles[x, y] = LoadTile(tileType, x, y, numberofitems);
                        numberofitems++;
                    }
            }
            Trace.Write("NumberOfItems: " + numberofitems.ToString() + "\n\n");
        }

        private LevelEditorTile LoadTile(char tileType, int x, int y, int index)
        {

            if (tilesAmount.Y < y)
                tilesAmount.Y += 1;

            return new LevelEditorTile(screenManager, tileType, index, x, y);

        }


        /// <summary>
        /// Unloads the level content.
        /// </summary>
        public void Dispose()
        {
            content.Unload();
        }

        #endregion

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

            
        /// <summary>
        /// Width of level measured in tiles.
        /// </summary>
        public int Width_1
        {
            get; set;
        }

        /// <summary>
        /// Height of the level measured in tiles.
        /// </summary>
        public int Height_1
        {
            get; set;
        }

        /// <summary>
        /// Updates all objects in the world, performs collision between them,
        /// and handles the time limit with scoring.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            // Calculate the visible range of tiles.
            screenleft = (int)CursorLocation.X - 25;
            if (screenleft < 0)
                screenleft = 0;
            screenup = (int)CursorLocation.Y - 25;
            if (screenup < 0)
                screenup = 0;

            screenright = (int)CursorLocation.X + 100;
            if (screenright > tilesAmount.X)
                screenright = (int)tilesAmount.X;
            screendown = (int)CursorLocation.Y + 25;
            if (screendown > tilesAmount.Y)
                screendown = (int)tilesAmount.Y;

            MapPosition = -(CursorLocation * LevelEditorTile.Width);
            //MapPosition = new Vector2(-((CursorLocation.X - 5) * LevelEditorTile.Width), -((CursorLocation.Y - 6) * LevelEditorTile.Height));
        }


        #region Draw
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, bool isactive)
        {
                DrawTiles(spriteBatch, isactive);
        }

        int screenleft;
        int screenright;
        int screenup;
        int screendown;

        /// <summary>
        /// Draws each tile in the level.
        /// </summary>
        private void DrawTiles(SpriteBatch spriteBatch, bool isactive)
        {    
            // For each tile position
            for (int y = screenup; y <= screendown; ++y)
            {
                for (int x = screenleft; x < screenright; ++x)
                {

                    LevelEditorTile tile;
                    // If there is a visible tile in that position
                    try
                    {
                        tile = tiles[x, y];
                    }
                    catch
                    {
                        throw new Exception("X: " + x + "; Y: " + y);

                    }
                    if (tile.texture != null)
                    {
                        // Draw it in screen space.
                        Vector2 position = MapPosition + new Vector2(x, y) * new Vector2(LevelEditorTile.Width, LevelEditorTile.Height);
                        Rectangle thisrectangle = new Rectangle(int.Parse(position.X.ToString()), int.Parse(position.Y.ToString()), LevelEditorTile.Width, LevelEditorTile.Height);

                        if (isactive && tile.isSelected(CursorLocation))
                        {
                            spriteBatch.Draw(screenManager.BlankTexture, thisrectangle, Color.White);
                            spriteBatch.Draw(tile.texture, thisrectangle, tile.Color);
                        }
                        else
                        {
                            spriteBatch.Draw(screenManager.BlankTexture, thisrectangle, Color.White);
                            spriteBatch.Draw(tile.texture, thisrectangle, tile.Color);
                            spriteBatch.Draw(screenManager.SpacerTexture, thisrectangle, Color.Gray);
                        }
                    }

                        if (x == Width - 1 && y == Height - 1)
                        {
                            if (Width_1 != x)
                                Width_1 = x;
                            if (Height_1 != y)
                                Height_1 = y;
                        }
                }
            }
        }

        #endregion
    }
}
