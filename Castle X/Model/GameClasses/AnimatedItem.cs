using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System.Diagnostics;

namespace CastleX
{
    public enum AnimatedItemType
    {
        Candle = 1,
        Torch = 2,
        WaterSurface = 3,
        UpsideDownSpell = 4
    }

    
    /// <summary>
    /// An animated item 
    /// </summary>
    public class AnimatedItem
    {
        ScreenManager screenManager;

        private AnimationPlayer sprite;
        private Animation spriteSheet;

        private SoundEffect collectedSound;

        private int height;
        public readonly Color Color;

        public AnimatedItemType ItemType
        {
            get;
            set;
        }

        // The item may be animated from a base position along the Y axis.
        private Vector2 basePosition;
        private float bounce;
        public Boolean isCollectable;
        public Boolean isBouncing;

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
                return basePosition + new Vector2(0.0f, bounce);
            }
        }

        /// <summary>
        /// Gets a circle which bounds this item in world space.
        /// </summary>
        public Circle BoundingCircle
        {
            get
            {
                return new Circle(Position, Tile.Width / 3.0f);
            }
        }

        private Rectangle localBounds;
        /// <summary>
        /// Gets a rectangle which bounds this item in world space.
        /// </summary>
        public Rectangle BoundingRectangle
        {
            get
            {
                int left = (int)Math.Round(Position.X - sprite.Origin.X) + localBounds.X;
                int top = (int)Math.Round(Position.Y - sprite.Origin.Y) + localBounds.Y;

                return new Rectangle(left, top, localBounds.Width, localBounds.Height);
            }
        }

        public AnimatedItem(ScreenManager ThisScreenManager, Level level, Vector2 position, AnimatedItemType spriteName, Boolean collectable)
        {
            screenManager = ThisScreenManager;
            this.level = level;
            this.basePosition = position;
            this.ItemType = spriteName;
            this.isCollectable = collectable;

            LoadContent(spriteName);

            collectedSound = level.screenManager.CoinCollectedSound;

            switch (ItemType)
            {
                case AnimatedItemType.Candle:
                    Color = Color.White;
                    isBouncing = false;
                    break;
                case AnimatedItemType.Torch:
                    Color = Color.White;
                    isBouncing = false;
                    break;
                case AnimatedItemType.WaterSurface:
                    Color = Color.White;
                    isBouncing = false;
                    this.basePosition = new Vector2(position.X + Tile.Width*1.5f, position.Y+Tile.Height*1.5f);
                    break;
                case AnimatedItemType.UpsideDownSpell:
                    Color = Color.White;
                    isBouncing = true;
                    break;
            }
        }


        /// <summary>
        /// Loads the item texture and collected sound.
        /// </summary>
        public void LoadContent(AnimatedItemType spriteName)
        {
            switch (spriteName)
            {
                case AnimatedItemType.Candle:
                    spriteSheet = new Animation(Level.screenManager.Candle, 0.1f, true);
                    bounce = 0 - height / 4;
                    break;
                case AnimatedItemType.Torch:
                    spriteSheet = new Animation(Level.screenManager.Torch, 0.1f, true);
                    bounce = 0 - height / 4;
                    break;
                case AnimatedItemType.WaterSurface:
                    spriteSheet = new Animation(Level.screenManager.WaterSurface, 0.3f, true, Tile.Width*4);
                    bounce = 0 ;
                    break;
                case AnimatedItemType.UpsideDownSpell:
                    spriteSheet = new Animation(Level.screenManager.UpsideDownSpell, 0.1f, true);
                    bounce = 0 - height / 4;
                    isCollectable = true;
                    break;
            }

            // Calculate bounds within texture size.
            int width = (int)(spriteSheet.FrameWidth * 0.35);
            int left = (spriteSheet.FrameWidth - width) / 2;
            height = (int)(spriteSheet.FrameWidth * 0.7);
            int top = spriteSheet.FrameHeight - height;
            localBounds = new Rectangle(left, top, width, height);

            sprite.PlayAnimation(spriteSheet);

            collectedSound = level.screenManager.CoinCollectedSound;
        }

        /// <summary>
        /// Bounces up and down in the air to entice players to collect them.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            // Bounce control constants
            const float BounceHeight = 0.18f;
            const float BounceRate = 3.0f;
            const float BounceSync = -0.75f;

            // Bounce along a sine curve over time.
            // Include the X coordinate so that neighboring items bounce in a nice wave pattern.            
            double t = gameTime.TotalGameTime.TotalSeconds * BounceRate + Position.X * BounceSync;
            if (isBouncing)
                bounce = (float)Math.Sin(t) * BounceHeight * height;

        }

        /// <summary>
        /// Called when this item has been collected by a player and removed from the level.
        /// </summary>
        /// <param name="collectedBy">
        /// The player who collected this item.
        /// </param>
        public void OnCollected(Player collectedBy, int itemnumber)
        {

            switch (ItemType)
            {
                case AnimatedItemType.Candle:
                    break;

                case AnimatedItemType.Torch:
                   // level.animatedItems.RemoveAt(itemnumber--);
                  //  level.Score += this.PointValue;
                    //collectedBy.PowerUp();
                    //collectedBy.Lives += 1;
                    //collectedBy.CurrentHealth += 1;
                 //   PlaySound();
                    break;

                case AnimatedItemType.WaterSurface:
                    PlaySound();
                    break;

                case AnimatedItemType.UpsideDownSpell:
                    PlaySound();
                    level.isUpsideDown = !level.isUpsideDown;
                    break;
            }

            // Remove item if it is collectable
            if (isCollectable)
                level.animatedItems.Remove(this);

        }

        //  Do any action needed when object is touched
        public void onTouched()
        {
            switch (ItemType)
            {
                case AnimatedItemType.Candle:
                    //  ***TODO: apagar a vela se tocada 
                    break;

                case AnimatedItemType.Torch:
                    break;

                case AnimatedItemType.WaterSurface:
                    break;
            }
        }

        private void PlaySound()
        {
            try
            {
                collectedSound.Play(screenManager.Settings.SoundVolumeAmount, 0, 0);
            }
            catch { }
        }

        /// <summary>
        /// Draws the item in the appropriate color.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            sprite.PlayAnimation(spriteSheet);
            sprite.Draw(gameTime, spriteBatch, Position, SpriteEffects.None, Color);
        }


    }
}
