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
    class HiddenTile
    {
        ScreenManager screenManager;

        private Texture2D texture;
        private Vector2 origin;
        private Random random = new Random(354668); // Arbitrary, but constant seed

        public bool hasTouched { get; set; }

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
        int variationcount;
        int index;

        public Rectangle BoundingRectangle
        {
            get
            {
                return new Rectangle(((int)Position.X - (Tile.Width / 2)) - 5, ((int)Position.Y - (Tile.Height / 2)) - 5, (int)Tile.Width + 10, (int)Tile.Height + 10);
            }
        }

        public HiddenTile(ScreenManager ThisScreenManager, Level level, Vector2 position, String baseName)
        {
            screenManager = ThisScreenManager;
            this.level = level;
            this.basePosition = position;
            if (baseName.Contains("BlockA")){
                variationcount = 7;
            } else if (baseName.Contains("BlockB")){
                variationcount = 2;
            }
            index = random.Next(variationcount);

            if (baseName.Equals("BlockA"))
            {
                if (index <= 0)
                    texture = screenManager.BlockATexture[index];
                else
                    texture = screenManager.BlockATexture[index - 1];
            }
            else if (baseName.Equals("BlockB"))
            {
                if (index <= 0)
                    texture = screenManager.BlockBTexture[index];
                else
                    texture = screenManager.BlockBTexture[index - 1];
            }
            else if (baseName.Equals("Platform"))
                texture = screenManager.PlatformTexture;
            else
            {
                texture = screenManager.PlatformTexture;
            }

            
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
        /// The player who collected this tile. 
        /// </param>
        public void OnCollected()
        {
            hasTouched = true;
        }

        /// <summary>
        /// Draws a tile in the appropriate color.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (hasTouched)
            {
                spriteBatch.Draw(texture, Position, null, Color.White, 0.0f, origin, 1.0f, SpriteEffects.None, 0.0f);
            }
        }
    }
}
