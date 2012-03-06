
#region Using Statements
using System;
using CastleX.Model.GameClasses.Entity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace CastleX
{

    /// <summary>
    /// An evil monster hell-bent on impeding the progress of our fearless adventurer.
    /// </summary>
    public sealed class GhostEnemy : Enemy
    {

        #region Fields

        public EnemyType Type;

        private Rectangle localBounds;

        // Sounds
        private SoundEffect monsterKilledSound;
        private SoundEffect ghostKilledSound;
        private SoundEffect flyingEnemyKilledSound;

        /// <summary>
        /// The direction this enemy is facing and moving along the X axis.
        /// </summary>
        private FaceDirection direction = FaceDirection.Left;
        private VerticalDirection verticalDirection = VerticalDirection.Up;

        /// <summary>
        /// How long this enemy has been waiting before turning around.
        /// </summary>
        private float waitTime;
        private float turnVerticalDirectionTime;

        /// <summary>
        /// How long to wait before turning around.
        /// </summary>
        private float MaxWaitTime = 0.5f;

        /// <summary>
        /// The speed at which this enemy moves along the X axis.
        /// </summary>
        private float MoveSpeed = 64.0f;

        // Used for include variations on enemy movement
        Random rnd = new Random();

        #endregion

        #region Properties

        public Level Level
        {
            get { return level; }
        }
        Level level;

        /// <summary>
        /// Position in world space of the bottom center of this enemy.
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
        }
        Vector2 position;

        /// <summary>
        /// Gets a rectangle which bounds this enemy in world space.
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

        public int ContactDamage
        {
            get { return contactDamage; }
            set { contactDamage = value; }
        }
        int contactDamage;

        #endregion

        /// <summary>
        /// Constructs a new Enemy.
        /// </summary>
        public GhostEnemy(ScreenManager thisScreenManager, Level level, Vector2 position, EnemyType type, int enemyNumber, int contactDamage)
        {
            this.level = level;
            Type = type;
            screenManager = thisScreenManager;
            this.position = position;
            this.contactDamage = contactDamage;
            IsAlive = true;
            MoveSpeed = 64.0f;

            LoadContent(enemyNumber);

            // pick starting direction by chance
            if (rnd.NextDouble() < 0.7)
                direction = (FaceDirection)(-(int)direction);
        }

        #region LoadContent
        /// <summary>
        /// Loads a particular enemy sprite sheet and sounds.
        /// </summary>
        public void LoadContent(int enemyNumber)
        {
            runAnimation = new Animation(Level.screenManager.GhostRunTexture[enemyNumber - 1], 0.1f, true, level.screenManager.SkinSettings.FrameWidth_Ghost_Run[enemyNumber - 1]);
            idleAnimation = new Animation(Level.screenManager.GhostIdleTexture[enemyNumber - 1], 0.15f, true, level.screenManager.SkinSettings.FrameWidth_Ghost_Idle[enemyNumber - 1]);
            dieAnimation = new Animation(Level.screenManager.GhostDieTexture[enemyNumber - 1], 0.07f, false, level.screenManager.SkinSettings.FrameWidth_Ghost_Die[enemyNumber - 1]);

            sprite.PlayAnimation(idleAnimation);

            // Load sounds.
            ghostKilledSound = Level.screenManager.GhostKilledSound;

            // Calculate bounds within texture size.
            int width = (int)(idleAnimation.FrameWidth * 0.35);
            int left = (idleAnimation.FrameWidth - width) / 2;
            int height = (int)(idleAnimation.FrameWidth * 0.7);
            int top = idleAnimation.FrameHeight - height;
            localBounds = new Rectangle(left, top, width, height);
        }
        #endregion

        /// <summary>
        /// Called when the enemy has been killed.
        /// </summary>
        public override void OnKilled()
        {
            IsAlive = false;
            ghostKilledSound.Play(screenManager.Settings.SoundVolumeAmount, 0, 0);
        }

        #region Update
        /// <summary>
        /// Paces back and forth along a platform, waiting at either end.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (!IsAlive)
                return;

            // Calculate tile position based on the side we are walking towards.
            float posX = Position.X + localBounds.Width / 2 * (int)direction;
            int tileX = (int)Math.Floor(posX / Tile.Width) - (int)direction;
            int tileY = (int)Math.Floor(Position.Y / Tile.Height);

            if (waitTime > 0)
            {
                // Wait for some amount of time.
                waitTime = Math.Max(0.0f, waitTime - (float)gameTime.ElapsedGameTime.TotalSeconds);
                if (waitTime <= 0.0f)
                {
                    // Then turn around.
                    direction = (FaceDirection)(-(int)direction);
                }
            }
            else
            {
                // If we are about to run into a wall or off a cliff, start waiting.
                if (Level.GetCollision(tileX + (int)direction, tileY - 1) == TileCollision.Impassable ||
                    Level.GetCollision(tileX + (int)direction, tileY) == TileCollision.Passable)
                {
                    waitTime = MaxWaitTime;
                }
                else
                {
                    // Move in the current direction.
                    Vector2 velocity = new Vector2((int)direction * MoveSpeed * elapsed, 0.0f);
                    position = position + velocity;
                }
            }
        }
        #endregion

        #region Draw
        /// <summary>
        /// Draws the animated enemy.
        /// </summary>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!IsAlive)
                sprite.PlayAnimation(dieAnimation);
            else if (!screenManager.Player.IsAlive || Level.ReachedExit || waitTime > 0)
                sprite.PlayAnimation(idleAnimation);
            else
                sprite.PlayAnimation(runAnimation);

            // Draw facing the way the enemy is moving.
            SpriteEffects flip = direction > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            sprite.Draw(gameTime, spriteBatch, Position, flip);
        }
        #endregion

    }
}
