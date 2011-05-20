using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System.Diagnostics;

namespace CastleX
{
    /// <summary>
    /// A valuable item the player can collect.
    /// </summary>
    class VanishingTile
    {
        ScreenManager screenManager;

        private Texture2D texture;
        private Vector2 origin;
        private Vector2 basePosition;

        public Level Level
        {
            get { return level; }
        }
        Level level;

        /// <summary>
        /// Gets the current position of this tile in world space.
        /// </summary>
        public Vector2 Position
        {
            get
            {
                return basePosition;
            }
        }

        public Rectangle BoundingRectangle
        {
            get
            {
                return new Rectangle((int)Position.X - (Tile.Width / 2), (int)Position.Y - (Tile.Height / 2), (int)Tile.Width, (int)Tile.Height);
            }
        }

        /// <summary>
        /// Constructs a new tile.
        /// </summary>

        public VanishingTile(ScreenManager ThisScreenManager, Level level, Vector2 position)
        {
            screenManager = ThisScreenManager;
            this.level = level;
            this.basePosition = position;

            texture = screenManager.BlockATexture[0];
            origin = new Vector2(texture.Width / 2.0f, texture.Height / 2.0f);
        }

        /// <summary>
        /// Bounces up and down in the air to entice players to collect them.
        /// </summary>
        public void Update(GameTime gameTime)
        {
        }

        /// <summary>
        /// Called when this tile has been collected by a player and removed from the level.
        /// </summary>
        /// <param name="collectedBy">
        /// The player who collected this item. 
        /// </param>
        public void OnCollected(Player collectedBy)
        {   
        }

        /// <summary>
        /// Draws a tile in the appropriate color.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, null, Color.White, 0.0f, origin, 1.0f, SpriteEffects.None, 0.0f);
        }
    }
}
