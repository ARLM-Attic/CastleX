using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System.Diagnostics;

namespace CastleX
{
    /// <summary>
    /// A Spring
    /// </summary>
    public class Spring
    {
        ScreenManager screenManager;
        private AnimationPlayer sprite;
        private Animation movingSpring;
        private Animation stoppedSpring;
        private SoundEffect springSound;

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
                return basePosition;
            }
        }

        /// <summary>   
        /// Gets whether or not the player's feet are on Spring  
        /// </summary>   
        public bool PlayerIsOn { get; set; }   

        public Rectangle BoundingRectangle
        {
            get
            {
                //  Adjusted to only collide with the lower part of the spring (not fired)
                return new Rectangle((int)Position.X - (Tile.Width / 2), (int)Position.Y - Tile.Height , (int)Tile.Width, (int)Tile.Height / 2);
                //return new Rectangle((int)basePosition.X, (int)basePosition.Y, (int)Tile.Width, (int)Tile.Height / 2);
            }
        }

        public Spring(ScreenManager ThisScreenManager, Level level, Vector2 position)
        {
            screenManager = ThisScreenManager;
            this.level = level;
            this.basePosition = new Vector2(position.X, position.Y + Tile.Height / 2) ;

            springSound = level.screenManager.SpringSound;  
            movingSpring = new Animation(Level.screenManager.SpringTexture, 0.03f, false); 
            stoppedSpring = new Animation(Level.screenManager.StoppedSpringTexture, 0.1f, false); 
            sprite.PlayAnimation(stoppedSpring);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Update(GameTime gameTime)
        {
            if (sprite.FrameIndex == 4) // *** TODO é 4?
                sprite.PlayAnimation(stoppedSpring);
        }

        /// <summary>
        /// Called when this spring has been hit by a player 
        /// </summary>
        /// <param name="collectedBy">
        /// The player who collided with the spring
        /// </param>
        public void OnColide(Player collectedBy)
        {
            sprite.PlayAnimation(movingSpring);
            PlaySound();
        }

        private void PlaySound()
        {
            try
            {
                springSound.Play(screenManager.Settings.SoundVolumeAmount, 0, 0);
            }
            catch { }
        }
        /// <summary>
        /// Draws a spring in the appropriate color.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            sprite.Draw(gameTime, spriteBatch, Position, SpriteEffects.None);
            if (screenManager.Settings.DebugMode) //  Show bounding box if debugging
                spriteBatch.Draw(screenManager.BlankTexture, this.BoundingRectangle, new Color (Color.Red, .5f));
        }
    }
}
