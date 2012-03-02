using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System.Diagnostics;

namespace CastleX
{
    enum BulletDirection
    {
        Left = 1,

        Right = 2,

    }

    class Bullet
    {
        private Texture2D texture;
        private Vector2 origin;

        ScreenManager screenManager;

        float maxtime = 10000.0f;
        float timer = 0.0f;

#if ZUNE
        float speed = 10.0f;
#else
        float speed = 30.0f;
#endif

        public BulletDirection direction;

        public Level Level
        {
            get { return level; }
        }
        Level level;

        /// <summary>
        /// Gets the current position of this gem in world space.
        /// </summary>
        public Vector2 Position;


        /// <summary>
        /// Gets a circle which bounds this gem in world space.
        /// </summary>
        public Circle BoundingCircle
        {
            get
            {
                return new Circle(Position, Tile.Width / 3.0f);
            }
        }
        public Bullet(ScreenManager thisScreenManager, Level level, Vector2 position, BulletDirection direction)
        {
            this.level = level;
            screenManager = thisScreenManager;
            this.Position = new Vector2(position.X, position.Y);
            this.direction = direction;
            LoadContent();
        }


        /// <summary>
        /// Loads the gem texture and collected sound.
        /// </summary>
        public void LoadContent()
        {
            texture = Level.screenManager.ArrowTexture;
            origin = new Vector2(texture.Width / 2.0f, texture.Height / 2.0f);
        }

        /// <summary>
        /// Bounces up and down in the air to entice players to collect them.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            timer++;
            if (direction == BulletDirection.Left)
                Position.X += speed;
            else
                Position.X -= speed;

            if (timer > maxtime)
                level.bullets.Remove(this);
        }

        public void onTouched(Player collectedby)
        {
            if (timer > maxtime/4*1)
            {
                level.bullets.Remove(this);
                collectedby.Hurt();
            }
        }

        public void onTouched(Enemy collectedby)
        {

                level.bullets.Remove(this);
                collectedby.OnKilled();
        }

        /// <summary>
        /// Draws a gem in the appropriate color.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, null, Color.White, 0.0f, origin, 1.0f, SpriteEffects.None, 0.0f);
        }
    }
}
