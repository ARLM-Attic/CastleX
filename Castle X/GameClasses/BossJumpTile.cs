using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System.Diagnostics;

namespace CastleX
{
    /// <summary>
    /// A item that makes the boss jump.
    /// </summary>
    class BossJumpTile
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
        public Vector2 Position
        {
            get
            {
                return basePosition + new Vector2(0.0f, 0.0f);
            }
        }
        public Rectangle BoundingRectangle
        {
            get
            {
                return new Rectangle((int)Position.X - (Tile.Width / 2), (int)Position.Y - (Tile.Height / 2), (int)Tile.Width, (int)Tile.Height);
            }
        }
        public BossJumpTile(Level level, Vector2 position)
        {
            screenManager = level.screenManager;
            this.level = level;
            this.basePosition = position;

            texture = level.screenManager.BlockATexture[1];
            origin = new Vector2(texture.Width / 2.0f, texture.Height / 2.0f);
          


        }
        public void Update(GameTime gameTime)
        {
        }
        public void OnCollected(Boss collectedBy)
        {
            collectedBy.isJumping = true;
        }

        /// <summary>
        /// Draws the boss 
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (screenManager.Settings.DebugMode) //  Show bounding box if debugging
                spriteBatch.Draw(screenManager.BlankTexture, BoundingRectangle, new Color(200, 200, 200, 155));
        }
    }
}
