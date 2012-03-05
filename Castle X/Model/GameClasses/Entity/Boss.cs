using System;
using CastleX.Model.GameClasses.Entity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using Microsoft.Xna.Framework.Media;
using System.IO;

namespace CastleX
{
    /// <summary>
    ///  A Boss who moves, jumps, etc.
    /// </summary>
    public class Boss : Entity
    {
        // Animations
        private Animation idleAnimation;
        private Animation runAnimation;
        private Animation jumpAnimation;
        private Animation celebrateAnimation;
        private Animation dieAnimation;
        private SpriteEffects flip = SpriteEffects.None;
        private AnimationPlayer sprite;

        public Stream StatusString { get; set; }

        ScreenManager screenManager;
        // Sounds
        private SoundEffect killedSound;
        private SoundEffect jumpSound;
        private SoundEffect fallSound;
        public Level Level
        {
            get { return level; }
        }
        Level level;

        bool demogoleft = false;
        public int Lives
        {
            get { return lives; }
            set { lives = value; }
        }
        int lives;

        public bool IsAlive;
        // Powerup state
        private const float MaxPowerUpTime = 6.0f;
        private float powerUpTime;
        

        public bool IsPoweredUp
        {
            get { return powerUpTime > 0.0f; }
        }


        private readonly Color[] poweredUpColors = {
                               Color.Red,
                               Color.Blue,
                               Color.Orange,
                               Color.Yellow,
                                               };
        private SoundEffect powerUpSound;


        // Physics state
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        Vector2 position;

        private float previousBottom;
        private float previousLeft;
        private float previousRight;
        //private float previousUp;
        //private float previousDown;

        int numJumps = 0;
        int MaxDoubleJumps = 5;

        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
        Vector2 velocity;


        // Constants for controling horizontal movement
        private float MoveAcceleration = 7000.0f;
        private float MaxMoveSpeed = 1000.0f;
        private float GroundDragFactor = 0.38f;
        private float AirDragFactor = 0.48f;
        //private float ClimbAcceleration = 7000.0f;
        //private float MaxClimbSpeed = 1000.0f;

        // Constants for controlling vertical movement
        private float MaxJumpTime = 0.35f;
        private float JumpLaunchVelocity = -2000.0f;
        private float GravityAcceleration = 1700.0f;
        private float MaxFallSpeed = 400.0f;
        private float JumpControlPower = 0.45f;

        // Input configuration
        private int LadderAlignment = 12;

        /// <summary>
        /// Gets whether or not the player's feet are on the ground.
        /// </summary>
        public bool IsOnGround
        {
            get { return isOnGround; }
            set { isOnGround = false; }
        }
        bool isOnGround;

        public bool isTouchingLadder = false;


        /// <summary>
        /// Gets whether the player was jumping on the last update cycle.
        /// </summary>
        public bool wasJumping {get; set;}

        /// <summary>
        /// Flag set when the player has pressed the jump button. This doesn't
        /// necessarily mean they are jumping - check the jumping flag for that.
        /// </summary>
        //private bool jumpRequested;

        private float jumpTime;

        /// <summary>
        /// Gets whether or not the player is ascending or descending a ladder.
        /// </summary>
        private bool isClimbing;
        public bool IsClimbing
        {
            get { return isClimbing; }
        }

        /// <summary>
        /// Gets whether the player was climbing on the last update cycle.
        /// </summary>
        private bool wasClimbing;

        /*/// <summary>
        /// Current user movement input.
        /// </summary>
        private float movementx;

        private float movementy;
        public float MovementX
        {
            get { return movementx; }
            set { movementx = value; }
        }
        public float MovementY
        {
            get { return movementy; }
            set { movementy = value; }
        }*/
    private Vector2 movement;
        // Jumping state
        public bool isJumping;
        //private bool wasJumping;
        //private float jumpTime;
        public bool isClimbingUp;
        public bool isClimbingDown;

        private Rectangle localBounds;
        /// <summary>
        /// Gets a rectangle which bounds this player in world space.
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

        /// <summary>
        /// Constructors a new player.
        /// </summary>
        public Boss(ScreenManager ThisScreenManager, Level level, Vector2 position)
        {
            screenManager = ThisScreenManager;
            this.level = level;

            LoadContent();
            Reset(position);
        }

        /// <summary>
        /// Loads the player sprite sheet and sounds.
        /// </summary>
        public void LoadContent()
        {
            idleAnimation = new Animation(Level.screenManager.PlayerIdleTexture, 0.1f, true, screenManager.SkinSettings.FrameWidth_Player_Idle);
            runAnimation = new Animation(Level.screenManager.PlayerRunTexture, 0.1f, true, screenManager.SkinSettings.FrameWidth_Player_Run);
            jumpAnimation = new Animation(Level.screenManager.PlayerJumpTexture, 0.1f, false, screenManager.SkinSettings.FrameWidth_Player_Jump);
            celebrateAnimation = new Animation(Level.screenManager.PlayerCelebrateTexture, 0.1f, false, screenManager.SkinSettings.FrameWidth_Player_Celebrate);
            dieAnimation = new Animation(Level.screenManager.PlayerDieTexture, 0.1f, false, screenManager.SkinSettings.FrameWidth_Player_Die);

            powerUpSound = Level.screenManager.PlayerPowerUpSound;

            // Calculate bounds within texture size.            
            int width = (int)(idleAnimation.FrameWidth * 0.4);
            int left = (idleAnimation.FrameWidth - width) / 2;
            int height = (int)(idleAnimation.FrameWidth * 0.8);
            int top = idleAnimation.FrameHeight - height;
            localBounds = new Rectangle(left, top, width, height);

            // Load sounds.            
            killedSound = Level.screenManager.PlayerKilledSound;
            jumpSound = Level.screenManager.PlayerJumpSound;
            fallSound = Level.screenManager.PlayerFallSound;
        }

        /// <summary>
        /// Resets the player to life.
        /// </summary>
        /// <param name="position">The position to come to life at.</param>
        public void Reset(Vector2 position)
        {
            Position = position;
            Velocity = Vector2.Zero;
            IsAlive = true;
            sprite.PlayAnimation(idleAnimation);
        }


        public void Update(GameTime gameTime)
        {
            GetInput();

            ApplyPhysics(gameTime);
            if (IsPoweredUp)
                powerUpTime = Math.Max(0.0f, powerUpTime - (float)gameTime.ElapsedGameTime.TotalSeconds);
    

            if (IsAlive)
            {
                if (isOnGround)
                {
                    if (Math.Abs(Velocity.X) - 0.02f > 0)
                    {
                        sprite.PlayAnimation(runAnimation);
                    }
                    else
                    {
                        sprite.PlayAnimation(idleAnimation);
                    }
                }
                else if (isClimbing)
                {
                    if (Math.Abs(Velocity.Y) - 0.02f > 0)
                    {
                        sprite.PlayAnimation(runAnimation);
                    }
                    else
                    {
                        sprite.PlayAnimation(idleAnimation);
                    }
                }
            }

            // Clear input.
            movement.X = 0.0f;
            movement.Y = 0.0f;
            isJumping = false;
            isClimbingUp = false;
            isClimbingDown = false;
            wasClimbing = isClimbing;
            wasJumping = isJumping;

            if (isOnGround)
                numJumps = 0; 
            
        }

        /// <summary>
        /// Gets player horizontal movement and jump commands from input.
        /// </summary>
        private void GetInput()
        {
           
                if (!demogoleft)
                    movement.X = 1.0f;
                else
                    movement.X = -1.0f;

                for (int i = 0; i < level.BossJumpTiles.Count; ++i)
                {
                    BossJumpTile bossJumpTile = level.BossJumpTiles[i];
                    if (BoundingRectangle.Intersects(bossJumpTile.BoundingRectangle))
                    {
                        isJumping = true;
                        //movement.Y += 1.0f;
                    }
                }
                for (int i = 0; i < level.exits.Count; ++i)
                {
                    Exit exit = level.exits[i];
                    if (!screenManager.IsDemo)
                    {
                        if (BoundingRectangle.Intersects(exit.BoundingRectangle))
                        {
                            screenManager.Player.Kill();
                        }
                    }
                }
        }


        private bool IsAlignedToLadder()
        {
            int playerOffset = ((int)position.X % Tile.Width) - Tile.Center;

            if (Math.Abs(playerOffset) <= LadderAlignment)
            {
                // Align the player with the middle of the tile
                position.X -= playerOffset;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Updates the player's velocity and position based on input, gravity, etc.
        /// </summary>
        public void ApplyPhysics(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 previousPosition = Position;

            // Base velocity is a combination of horizontal movement control and
            // acceleration downward due to gravity.
            if (!isClimbing)
            {
                velocity.X += movement.X * MoveAcceleration * elapsed;

                if (wasClimbing)
                {
                    // If we've just finished climbing, stop at the top of the ladder.
                    velocity.Y = 0;
                }
                else
                {
                    // Apply gravity as normal
                    velocity.Y = MathHelper.Clamp(velocity.Y + GravityAcceleration * elapsed, -MaxFallSpeed, MaxFallSpeed);
                }
            }
            else
            {
                // When player is climbing ladder
                velocity.Y = movement.Y * MoveAcceleration * elapsed;
            }

            velocity.Y = DoJump(velocity.Y, gameTime);

            // Apply pseudo-drag horizontally.
            if (isOnGround)
                velocity.X *= GroundDragFactor;
            else
                velocity.X *= AirDragFactor;

            // Prevent the player from running faster than his top speed.            
            velocity.X = MathHelper.Clamp(velocity.X, -MaxMoveSpeed, MaxMoveSpeed);

            // Apply velocity.
            Position += velocity * elapsed;
            Position = new Vector2((float)Math.Round(Position.X), (float)Math.Round(Position.Y));

            // If the player is now colliding with the level, separate them.
            HandleCollisions(gameTime);

            // If the collision stopped us from moving, reset the velocity to zero.
            if (Position.X == previousPosition.X)
                velocity.X = 0;

            if (Position.Y == previousPosition.Y)
                velocity.Y = 0;
        }

        /// <summary>
        /// Calculates the Y velocity accounting for jumping and
        /// animates accordingly.
        /// </summary>
        /// <remarks>
        /// During the accent of a jump, the Y velocity is completely
        /// overridden by a power curve. During the decent, gravity takes
        /// over. The jump velocity is controlled by the jumpTime field
        /// which measures time into the accent of the current jump.
        /// </remarks>
        /// <param name="velocityY">
        /// The player's current velocity along the Y axis.
        /// </param>
        /// <returns>
        /// A new Y velocity if beginning or continuing a jump.
        /// Otherwise, the existing Y velocity.
        /// </returns>
        public float DoJump(float velocityY, GameTime gameTime)
        {  
            // If the player wants to jump
            if (isJumping && !isClimbing)
            {
                // Begin or continue a jump
                if ((!wasJumping && IsOnGround) || jumpTime > 0.0f)
                {
                    if (jumpTime == 0.0f)
                        jumpSound.Play(screenManager.Settings.SoundVolumeAmount, 0, 0);
                        jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    sprite.PlayAnimation(jumpAnimation);
                }

                if (0.0f < jumpTime && jumpTime <= MaxJumpTime)
                {
                    // Fully override the vertical velocity with a power curve that gives players more control over the top of the jump
                    velocityY = JumpLaunchVelocity * (1.0f - (float)Math.Pow(jumpTime / MaxJumpTime, JumpControlPower));
                }
                else
                {
                    // Reached the apex of the jump and has double jumps
                    //if velocityY <> 0 to double jump on descent
                    if (velocityY <= -MaxFallSpeed * 1.0f && !wasJumping && numJumps < MaxDoubleJumps)
                    {
                        jumpSound.Play(screenManager.Settings.SoundVolumeAmount, 0, 0);
                        velocityY = JumpLaunchVelocity * (1.0f - (float)Math.Pow(jumpTime / MaxJumpTime, JumpControlPower));
                        jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds; sprite.PlayAnimation(jumpAnimation); numJumps++;
                    }
                    else
                    {
                        jumpTime = 0.0f;
                    }
                }
            }
            else
            {
                // Continues not jumping or cancels a jump in progress
                jumpTime = 0.0f;
            }
            wasJumping = isJumping;
            return velocityY;
        }

        /// <summary>
        /// Detects and resolves all collisions between the player and his neighboring
        /// tiles. When a collision is detected, the player is pushed away along one
        /// axis to prevent overlapping. There is some special logic for the Y axis to
        /// handle platforms which behave differently depending on direction of movement.
        /// </summary>
        private void HandleCollisions(GameTime gameTime)
        {
            // Get the player's bounding rectangle and find neighboring tiles.
            Rectangle bounds = BoundingRectangle;
            int leftTile = (int)Math.Floor((float)bounds.Left / Tile.Width/2);
            int rightTile = (int)Math.Ceiling(((float)bounds.Right / Tile.Width)) - 1;
            int topTile = (int)Math.Floor((float)bounds.Top / Tile.Height);
            int bottomTile = (int)Math.Ceiling(((float)bounds.Bottom / Tile.Height)) - 1;

            // Reset flag to search for ground collision.
            isOnGround = false;

            // For each potentially colliding tile,
            for (int y = topTile; y <= bottomTile; ++y)
            {
                for (int x = leftTile; x <= rightTile; ++x)
                {
                    // If this tile is collidable,
                    TileCollision collision = Level.GetCollision(x, y);
                    if (collision != TileCollision.Passable)
                    {
                        // Determine collision depth (with direction) and magnitude.
                        Rectangle tileBounds = Level.GetBounds(x, y);
                        Vector2 depth = RectangleExtensions.GetIntersectionDepth(bounds, tileBounds);
                        if (depth != Vector2.Zero)
                        {
                            float absDepthX = Math.Abs(depth.X);
                            float absDepthY = Math.Abs(depth.Y);

                            // Resolve the collision along the shallow axis.
                            if (absDepthY < absDepthX || collision == TileCollision.Platform)
                            {
                                // If we crossed the top of a tile, we are on the ground.
                                if (previousBottom <= tileBounds.Top)
                                {
                                    if (collision == TileCollision.Ladder)
                                    {
                                        if (!isClimbing && !isJumping)
                                        {
                                            // When walking over a ladder
                                            isOnGround = true;
                                        }
                                    }
                                    else
                                    {
                                        //In the demo that plays in the background of the menu,
                                        // we want the person to go left if he hits a wall on the right.
                                        if (tileBounds.Right == previousRight)
                                            demogoleft = true;

                                        //In the demo that plays in the background of the menu,
                                        // we want the person to go right if he hits a wall on the left.
                                        if (tileBounds.Left == previousLeft)
                                            demogoleft = false;
                                            isOnGround = true;
                                            isClimbing = false;
                                            isJumping = false;
                                    }
                                }

                                // Ignore platforms, unless we are on the ground.
                                if (collision == TileCollision.Impassable || isOnGround)
                                {
                                    // Resolve the collision along the Y axis.
                                    Position = new Vector2(Position.X, Position.Y + depth.Y);

                                    // Perform further collisions with the new bounds.
                                    bounds = BoundingRectangle;
                                }
                            }
                            else if (collision == TileCollision.Impassable) // Ignore platforms.
                            {
                                // When walking over an impassable tile i.e.
                                // a normal platform

                                // Resolve the collision along the X axis.
                                Position = new Vector2(Position.X + depth.X, Position.Y);

                                // Perform further collisions with the new bounds.
                                bounds = BoundingRectangle;
                            }
                            else if (collision == TileCollision.Ladder && !isClimbing)
                            {
                                // When walking in front of a ladder, falling off a ladder
                                // but not climbing

                                // Resolve the collision along the Y axis.
                                Position = new Vector2(Position.X, Position.Y);

                                // Perform further collisions with the new bounds.
                                bounds = BoundingRectangle;
                            }
                        }
                    }
                }
            }

            // Save the new bounds bottom.
            previousBottom = bounds.Bottom;
            previousLeft = bounds.Left;
            previousRight = bounds.Right;
        }

        /// <summary>
        /// Called when the player has been killed.
        /// </summary>
        /// <param name="killedBy">
        /// The enemy who killed the player. This parameter is null if the player was
        /// not killed by an enemy (fell into a hole).
        /// </param>
        /// <param name="playKillSound">
        /// This tells weather to play the sound of being killed or the sound of falling. if
        /// this is true, the killed sound will play, if this is false, the fall sound will play.
        /// </param>
        public void OnKilled(Enemy killedBy, bool playKillSound)
        {
            IsAlive = false;

            if (playKillSound)
                killedSound.Play(screenManager.Settings.SoundVolumeAmount, 0, 0);
                else
                fallSound.Play(screenManager.Settings.SoundVolumeAmount, 0, 0);

            sprite.PlayAnimation(dieAnimation);
        }

        /// <summary>
        /// Called when this player reaches the level's exit.
        /// </summary>
        public void OnReachedExit()
        {
            sprite.PlayAnimation(celebrateAnimation);
        }

        /// <summary>
        /// Draws the animated player.
        /// </summary>
        /*public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Flip the sprite to face the way we are moving.
            if (Velocity.X > 0)
                flip = SpriteEffects.FlipHorizontally;
            else if (Velocity.X < 0)
                flip = SpriteEffects.None;

            // Draw that sprite.
            sprite.Draw(gameTime, spriteBatch, Position, flip);
        }*/
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Flip the sprite to face the way we are moving.
            if (Velocity.X > 0)
                flip = SpriteEffects.FlipHorizontally;
            else if (Velocity.X < 0)
                flip = SpriteEffects.None;

            // Calculate a tint color based on power up state.

            // Draw that sprite. if this is the front page demo, we draw it the normal color.
            // if its in game, then its a boss, so we draw it red.
            if (screenManager.IsDemo)
                sprite.Draw(gameTime, spriteBatch, Position, flip, Color.White);
            else
                sprite.Draw(gameTime, spriteBatch, Position, flip, new Color(255, 0, 0, 150));
        }
        public void PowerUp()
        {
            powerUpTime = MaxPowerUpTime;

            powerUpSound.Play(screenManager.Settings.SoundVolumeAmount, 0, 0);
        }
    


    }
}
