using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CastleX
{
    /// <summary>
    /// A valuable item the player can collect.
    /// </summary>
    public class DeathTile
    {
        ScreenManager screenManager;

        private Texture2D texture;
        private Vector2 origin;
        private Random random = new Random(354668); // Arbitrary, but constant seed

        public bool AnimateColors
        {
            get { return animateColors; }
            set { animateColors = value; }
        }
        bool animateColors = true;

        Color[] DeathTileColors = {
                               Color.Black,
                               Color.Maroon,
                               Color.Red,
                               Color.Maroon,
                                               };
        
        // The death tile is animated from a base position along the Y axis.
        private Vector2 basePosition;
        
        public Level Level
        {
            get { return level; }
        }
        Level level;

        /// <summary>
        /// Gets the current position of this item in world space.
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
                return new Rectangle(((int)Position.X - (Tile.Width / 2)) - 2, ((int)Position.Y - (Tile.Height / 2)) - 2, (int)Tile.Width + 4, (int)Tile.Height + 4);
            }
        }

        public DeathTile(ScreenManager ThisScreenManager, Level level, Vector2 position, string baseName)
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
        /// Draws a death tile in the appropriate color.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, bool AnimateColors)
        {
            Color color;
            if (screenManager.isRunningSlow)
            {
                color = Color.Red;
            }
            else
            {
                float t = ((float)gameTime.TotalGameTime.TotalSeconds) * 5.0f;
                int colorIndex = (int)t % DeathTileColors.Length;
                color = DeathTileColors[colorIndex];
            }
                spriteBatch.Draw(texture, Position, null, color, 0.0f, origin, 1.0f, SpriteEffects.None, 0.0f);
        }
    }
}
