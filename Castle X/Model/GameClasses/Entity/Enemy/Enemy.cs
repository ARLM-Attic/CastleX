﻿
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
    public class Enemy : Entity
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
        /// Default empty constructor for a new Enemy.
        /// </summary>
        public Enemy() {}

        /// <summary>
        /// Constructs a new Enemy.
        /// </summary>
        public Enemy(ScreenManager thisScreenManager, Level level, Vector2 position, EnemyType type, int enemyNumber, int contactDamage)
        {
            this.level = level;
            Type = type;
            screenManager = thisScreenManager;
            this.position = position;
            this.contactDamage = contactDamage;
            IsAlive = true;
            MoveSpeed = 64.0f;

            LoadContent(type, enemyNumber);

            // pick starting direction by chance
            if (rnd.NextDouble() < 0.7)
                direction = (FaceDirection)(-(int)direction);

            // Only used by flying enemies
            if (type == EnemyType.Flying)
            {
                turnVerticalDirectionTime = MaxWaitTime * 2; // Flying enemies are always turning around vertical movement
                //pick starting vertical direction by chance
                if (rnd.NextDouble() < 0.6)
                    verticalDirection = (VerticalDirection)(-(int)verticalDirection);
            }
        }

        #region LoadContent
        /// <summary>
        /// Loads a particular enemy sprite sheet and sounds.
        /// </summary>
        public virtual void LoadContent(EnemyType type, int enemyNumber)
        {
            switch (type)
            {
                case EnemyType.Monster:
                    runAnimation = new Animation(Level.screenManager.MonsterRunTexture[enemyNumber - 1], 0.1f, true, level.screenManager.SkinSettings.FrameWidth_Monster_Run[enemyNumber - 1]);
                    idleAnimation = new Animation(Level.screenManager.MonsterIdleTexture[enemyNumber - 1], 0.15f, true, level.screenManager.SkinSettings.FrameWidth_Monster_Idle[enemyNumber - 1]);
                    dieAnimation = new Animation(Level.screenManager.MonsterDieTexture[enemyNumber - 1], 0.07f, false, level.screenManager.SkinSettings.FrameWidth_Monster_Die[enemyNumber - 1]);
                    break;
                case EnemyType.Ghost:
                    runAnimation = new Animation(Level.screenManager.GhostRunTexture[enemyNumber - 1], 0.1f, true, level.screenManager.SkinSettings.FrameWidth_Ghost_Run[enemyNumber - 1]);
                    idleAnimation = new Animation(Level.screenManager.GhostIdleTexture[enemyNumber - 1], 0.15f, true, level.screenManager.SkinSettings.FrameWidth_Ghost_Idle[enemyNumber - 1]);
                    dieAnimation = new Animation(Level.screenManager.GhostDieTexture[enemyNumber - 1], 0.07f, false, level.screenManager.SkinSettings.FrameWidth_Ghost_Die[enemyNumber - 1]);
                    break;
                case EnemyType.Flying:
                    runAnimation = new Animation(Level.screenManager.FlyingRunTexture[enemyNumber - 1], 0.1f, true, level.screenManager.SkinSettings.FrameWidth_Flying_Run[enemyNumber - 1]);
                    idleAnimation = new Animation(Level.screenManager.FlyingIdleTexture[enemyNumber - 1], 0.15f, true, level.screenManager.SkinSettings.FrameWidth_Flying_Idle[enemyNumber - 1]);
                    dieAnimation = new Animation(Level.screenManager.FlyingDieTexture[enemyNumber - 1], 0.07f, false, level.screenManager.SkinSettings.FrameWidth_Flying_Die[enemyNumber - 1]);
                    break;
            }

            sprite.PlayAnimation(idleAnimation);

            // Load sounds.
            monsterKilledSound = Level.screenManager.MonsterKilledSound;
            ghostKilledSound = Level.screenManager.GhostKilledSound;
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
        public virtual void OnKilled()
        {
            IsAlive = false;
            switch (this.Type)
            {
                case EnemyType.Monster:
                     monsterKilledSound.Play(screenManager.Settings.SoundVolumeAmount, 0, 0);
                     break;
                case EnemyType.Ghost:
                     ghostKilledSound.Play(screenManager.Settings.SoundVolumeAmount, 0, 0);
                     break;
                case EnemyType.Flying:
                     flyingEnemyKilledSound.Play(screenManager.Settings.SoundVolumeAmount, 0, 0);
                     break;
                case EnemyType.Swimming:
                     //swimmingEnemyKilledSound.Play(screenManager.Settings.SoundVolumeAmount, 0, 0);
                     break;
            }
             
        }

        #region Update
        /// <summary>
        /// Paces back and forth along a platform, waiting at either end.
        /// </summary>
        public virtual void Update(GameTime gameTime)
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
                // ---------------------- Move walking enemies
                if (this.Type != EnemyType.Flying)  
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
                // ---------------------- Move flying enemies
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


        }
        #endregion

        #region Draw
        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            throw new Exception("Enemy Draw must be overridden");
        }
        #endregion

    }
}
