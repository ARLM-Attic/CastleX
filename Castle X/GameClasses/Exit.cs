using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace CastleX
{
    /// <summary>
    /// The Exit point.
    /// </summary>
    public class Exit
    {
        ScreenManager screenManager;
        private Texture2D texture;
        private Vector2 origin;
        private int nextLevel;
        private int exitNumber;
        public int ExitNumber
        {
            get { return exitNumber; }
            set { exitNumber = value; }
        }

        public Level Level
        {
            get { return level; }
        }
        Level level;

        /// <summary>
        /// Gets the current position of this tester in world space.
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
        }
        private Vector2 position;

        /// <summary>
        /// Gets a rectangle which bounds this exit in world space.
        /// </summary>
        public Rectangle BoundingRectangle
        {
            get
            {
                return new Rectangle((int)Position.X - (Tile.Width / 2), (int)Position.Y - (Tile.Height / 2), (int)Tile.Width, (int)Tile.Height);
            }
        }

        /// <summary>
        /// Constructs a new exit.
        /// </summary>
        public Exit(ScreenManager ThisScreenManager, Level level, Vector2 position, int exitNumber, int nextLevel)
        {
            screenManager = ThisScreenManager;
            this.level = level;
            this.position = position;
            this.exitNumber = exitNumber;
            this.nextLevel = nextLevel;
            LoadContent();
        }

        /// <summary>
        /// Loads the exit texture 
        /// </summary>
        public void LoadContent()
        {          
            texture = Level.screenManager.ExitTexture; 
            origin = new Vector2(texture.Width / 2.0f, texture.Height / 2.0f);
        }

        /// <summary>
        /// Called when the player reached this exit
        /// </summary>
        /// <param name="collectedBy">
        /// The player who reached this exit. Although currently not used, this parameter would be
        /// useful for making the exit has any kind of effect on the player.
        /// </param>
        public void OnReached(Player reachedBy)
        {
            level.ReachedExit = true;
            level.ExitNumber = this.exitNumber;
            level.NextLevel = this.nextLevel;
        }
        
        /// <summary>
        /// Draws a exit in the appropriate color.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (level.CoinsRequired)
            {
                if (level.AllCoinsCollected)
                    spriteBatch.Draw(texture, Position, null, Color.White, 0.0f, origin, 1.0f, SpriteEffects.None, 0.0f);
                else
                    spriteBatch.Draw(texture, Position, null, Color.Gray, 0.0f, origin, 1.0f, SpriteEffects.None, 0.0f);
            } else
                spriteBatch.Draw(texture, Position, null, Color.White, 0.0f, origin, 1.0f, SpriteEffects.None, 0.0f);

            if (screenManager.Settings.DebugMode) //  Show bounding box if debugging
                spriteBatch.Draw(screenManager.BlankTexture, BoundingRectangle, new Color(255, 0, 0, 75)); 

        }
    }
}
