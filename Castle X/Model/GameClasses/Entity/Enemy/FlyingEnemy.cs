
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
    public sealed class FlyingEnemy : Enemy
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
        public FlyingEnemy(ScreenManager thisScreenManager, Level level, Vector2 position, EnemyType type, int enemyNumber, int contactDamage)
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

            turnVerticalDirectionTime = MaxWaitTime * 2; // Flying enemies are always turning around vertical movement
            //pick starting vertical direction by chance
            if (rnd.NextDouble() < 0.6)
                verticalDirection = (VerticalDirection)(-(int)verticalDirection);
        }

        #region LoadContent
        /// <summary>
        /// Loads a particular enemy sprite sheet and sounds.
        /// </summary>
        public void LoadContent(int enemyNumber)
        {
            runAnimation = new Animation(Level.screenManager.FlyingRunTexture[enemyNumber - 1], 0.1f, true, level.screenManager.SkinSettings.FrameWidth_Flying_Run[enemyNumber - 1]);
            idleAnimation = new Animation(Level.screenManager.FlyingIdleTexture[enemyNumber - 1], 0.15f, true, level.screenManager.SkinSettings.FrameWidth_Flying_Idle[enemyNumber - 1]);
            dieAnimation = new Animation(Level.screenManager.FlyingDieTexture[enemyNumber - 1], 0.07f, false, level.screenManager.SkinSettings.FrameWidth_Flying_Die[enemyNumber - 1]);

            sprite.PlayAnimation(idleAnimation);

            // Load sounds.
            flyingEnemyKilledSound = Level.screenManager.FlyingEnemyKilledSound;

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
            flyingEnemyKilledSound.Play(screenManager.Settings.SoundVolumeAmount, 0, 0);
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
                // If we are about to run into a wall, start waiting to turn around horizontal movement.
                if (Level.GetCollision(tileX + (int)direction, tileY) == TileCollision.Impassable ||
                    Level.GetCollision(tileX + (int)direction, tileY + (int)verticalDirection) == TileCollision.Impassable)
                {
                    waitTime = MaxWaitTime / 2;
                    //// If we are about to run into the roof or floor, also turn around vertical movement.
                    if (Level.GetCollision(tileX + (int)direction, tileY + (int)verticalDirection) == TileCollision.Impassable)
                        verticalDirection = (VerticalDirection)(-(int)verticalDirection);
                }
                else
                {
                    // Move in the current direction.
                    // Turn around vertical movement only time to time
                    if (turnVerticalDirectionTime > 0)
                    {
                        // Wait for some amount of time.
                        turnVerticalDirectionTime = Math.Max(0.0f, turnVerticalDirectionTime - (float)gameTime.ElapsedGameTime.TotalSeconds);
                        if (turnVerticalDirectionTime <= 0.0f)
                        {
                            // add some randomicity in the movement
                            if (rnd.NextDouble() > 0.7)
                            {
                                // Then turn around.
                                verticalDirection = (VerticalDirection)(-(int)verticalDirection);
                            }
                            turnVerticalDirectionTime = MaxWaitTime * 2;
                            //// If we are about to run into the roof or floor, turn around vertical movement.
                            if (Level.GetCollision(tileX, tileY + (int)verticalDirection) == TileCollision.Impassable ||
                                Level.GetCollision(tileX + (int)direction, tileY + (int)verticalDirection) == TileCollision.Impassable)
                                verticalDirection = (VerticalDirection)(-(int)verticalDirection);
                        }
                    }
                    Vector2 velocity = new Vector2((int)direction * MoveSpeed * elapsed, (float)rnd.NextDouble() * (int)verticalDirection);

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
