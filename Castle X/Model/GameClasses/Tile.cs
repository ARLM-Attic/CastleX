using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CastleX
{
    /// <summary>
    /// Controls the collision detection and response behavior of a tile.
    /// </summary>
    public enum TileCollision
    {
        /// <summary>
        /// A passable tile is one which does not hinder player motion at all.
        /// </summary>
        Passable = 0,

        /// <summary>
        /// An impassable tile is one which does not allow the player to move through
        /// it at all. It is completely solid.
        /// </summary>
        Impassable = 1,

        /// <summary>
        /// A platform tile is one which behaves like a passable tile except when the
        /// player is above it. A player can jump up through a platform as well as move
        /// past it to the left and right, but can not fall down through the top of it.
        /// </summary>
        Platform = 2,

        /// <summary>
        /// A ladder tile is one which behaves like - well, a ladder!
        /// </summary>
        Ladder = 3,
    }

    /// <summary>
    /// Controls special collision detection or response behavior of a tile.
    /// </summary>
    public enum TileType
    {
        Other = 1,  // Control tiles, exit tiles, etc.
        Brick = 2,  // Break arrows
        Wood = 3,   // Arrows stick to it
        Water = 4,  // slows down the player
        Transparent = 5  // shows through texture from the tile below it
    }
    /// <summary>
    /// Stores the appearance and collision behavior of a tile.
    /// </summary>
    public struct Tile
    {
        
        public Texture2D Texture;
        public TileCollision Collision;
        public TileType Type;
        public static Vector2 Size;
        public static int Width
        {
            get { return width; }
            set { width = value; }
        }

        static int width;
        public static int Height
        {
            get { return height; }
            set { height = value; }
        }

        static int height;

        public static int Center
        {
            get { return center; }
            set { center = value; }
        }

    static int center;


        /// <summary>
        /// Constructs a new tile.
        /// </summary>
        public Tile(Texture2D texture, TileCollision collision, TileType type, ScreenManager screenManager)
        {
            Texture = texture;
            Collision = collision;
            Type = type;
            if (texture != null)
                Size = new Vector2(texture.Width, texture.Height);
            else
                Size = new Vector2(Width, Height);

            width =  32;
            height =  16; 
            center = width / 2;
        }
    }
}
