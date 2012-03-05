using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace CastleX
{
    public enum ArrowDirection
    {
        Left = 1,
        Right = 2,
    }

    public class Arrow
    {
        private Texture2D texture;
        private Vector2 origin;
        public int Width;
        public int Height;
        ScreenManager screenManager;

        private SoundEffect hitSound;
        private Boolean played = false;

        float speed = 15.0f;

        public ArrowDirection Direction;

        public Level Level
        {
            get { return level; }
        }
        Level level;

        /// <summary>
        /// Gets the current position of this arrow in world space.
        /// </summary>
        public Vector2 Position;

        /// <summary>
        /// Gets a circle which bounds this arrow in world space.
        /// </summary>
        public Rectangle BoundingRectangle
        {
            get
            {
                  return new Rectangle((int)(Position.X - (Width / 2)), (int)(Position.Y - (Height / 2)), Width, Height);
            }
        }

        public Arrow(ScreenManager thisScreenManager, Level level, Vector2 position, ArrowDirection direction)
        {
            this.level = level;
            screenManager = thisScreenManager;
            this.Position = new Vector2(position.X, position.Y);
            this.Direction = direction;
            LoadContent();
        }


        /// <summary>
        /// Loads the arrow texture 
        /// </summary>
        public void LoadContent()
        {
            texture = Level.screenManager.ArrowTexture;
            origin = new Vector2(texture.Width / 2.0f, texture.Height / 2.0f);
            Width = texture.Width;
            Height = texture.Height;
            hitSound = Level.screenManager.ArrowHitSound;
        }

        /// <summary>
        /// Update the arrow position until it leaves the screen
        /// </summary>
        public void Update(GameTime gameTime)
        {

            if (Direction == ArrowDirection.Left)
                Position.X += speed;
            else
                Position.X -= speed;

            if (Position.X < -this.Width || Position.X > level.Width* Tile.Width )
                level.arrows.Remove(this);
        }

        public void onTouched(Enemy touchedBy)
        {
            level.arrows.Remove(this);
            touchedBy.OnKilled();
        }

        public void onTouched(Item touchedBy)
        {
            touchedBy.onTouched();
            // remove the arrow only when touching trunks
            if (touchedBy.ItemType == ItemType.Trunk)
                level.arrows.Remove(this);
        }

        public void onTouched(AnimatedItem touchedBy)
        {
            touchedBy.onTouched();
            // ***TODO 
            // Example: remove the arrow when touching someting
            //          Or maybe transform it (touching fire makes it a flaming arrow?)
            //if(collectedby.ItemType == AnimatedItemType.Candle)
            //    level.arrows.Remove(this);
        }

        public void onTouched(MultipleStateItem touchedBy)
        {
            touchedBy.onTouched();
        }

        public void onTouched(Tile touchedBy)
        {
            if (touchedBy.Type == TileType.Wood)
            {
                if (!played)
                {
                    hitSound.Play(screenManager.Settings.SoundVolumeAmount, 0, 0);
                    played = true;
                }
                this.speed = 0f;
            }
            else
                level.arrows.Remove(this);
        }

        public void onTouched(MovingItem touchedBy)
        {
            level.arrows.Remove(this);
        }
        /// <summary>
        /// Draws the arrow
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Direction == ArrowDirection.Left)
                spriteBatch.Draw(texture, Position, null, Color.White, 0.0f, origin, 1.0f, SpriteEffects.None, 0.0f);
            else
                spriteBatch.Draw(texture, Position, null, Color.White, 0.0f, origin, 1.0f, SpriteEffects.FlipHorizontally, 0.0f);

            if (screenManager.Settings.DebugMode) //  Show bounding box if debugging
                spriteBatch.Draw(screenManager.BlankTexture, BoundingRectangle, new Color(255, 0, 0, 75)); 

        }
    }
}
