
#region Using Statements
using System;
using System.Diagnostics;
using CastleX.Model.GameClasses.Entity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#endregion

namespace CastleX
{
    /// <summary>
    /// Our fearless adventurer!
    /// </summary>
    public class Player : Entity
    {

        #region Fields

        // Animations
        //private Animation idleAnimation;
        //private Animation runAnimation;
        private Animation climbingAnimation;
        private Animation jumpAnimation;
        private Animation celebrateAnimation;
        //private Animation dieAnimation;
        private Animation attackAnimation;
        private Animation attack_swordAnimation;
        private SpriteEffects flip = SpriteEffects.None;
        //private AnimationPlayer sprite;
        private AnimationPlayer sprite_sword;

        ScreenManager screenManager;

        // Sounds
        private SoundEffect killedSound;
        private SoundEffect jumpSound;
        private SoundEffect fallSound;
        private SoundEffect hitSound;
        private SoundEffect swordUnderwaterSound; 
        private SoundEffect swordSound;
        private SoundEffect arrowSound;
        private SoundEffect arrowUnderwaterSound;
        private SoundEffect powerUpSound;
        private SoundEffect DiveSound;

        // Inventory
        public int YellowKeys = 0;
        public int GreenKeys = 0;
        public int RedKeys = 0;
        public int BlueKeys = 0;
        public bool hasMap = false;
        public bool hasOxygen = false;
        public bool hasCandle = false;
        public bool IsAlive;
        public int Score = 0;

        // Powerup state
        private const float MaxPowerUpTime = 6.0f;
        private float powerUpTime;

        // dive / underwater control
        private const float MaxUnderwaterTime = 1.0f;
        private float UnderwaterTime;
        private Boolean isUnderWater = false;        // set to true when player is under water
        private Boolean wasUnderWater = false;        // set to true when player is under water

        bool demogoleft = false;

        private float previousBottom;
        private float previousLeft;
        private float previousRight;

        int numJumps = 0;
        int MaxDoubleJumps = 5;

        GamePadState gamePadState;
        KeyboardState keyboardState;
        GamePadState previousGamePadState;
        KeyboardState previousKeyboardState;

        #endregion

        #region Properties

        Level level;
        public Level Level
        {
            get { return level; }
        }

        int lives = 4;
        public int Lives
        {
            get { return lives; }
            set { lives = value; }
        }

        public bool IsUnderwater
        {
            get { return isUnderWater; }
        }

        public Vector2 StartPosition { get; set; }

        public bool IsPoweredUp
        {
            get { return powerUpTime > 0.0f; }
            set
            {
                if (!value)
                    powerUpTime = 0;
            }
        }

        public bool IsOutOfOxygen
        {
            get { return UnderwaterTime <= 0.0f; }
        }

        Boolean isGhost = false;
        public bool IsGhost
        {
            get { return isGhost; }
        }

        // Physics state
        Vector2 position;
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        Vector2 velocity;
        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        #endregion

        private readonly Color[] poweredUpColors = {
                               Color.Red,
                               Color.Blue,
                               Color.Orange,
                               Color.Yellow,
                                               };

        // Constants for controling horizontal movement
        private float MoveAcceleration
        {
            get
            {
                if (isUnderWater)
                    return 5000.0f;
                else
                    return 7000.0f;
            }
        }

        private float MaxMoveSpeed
        {
            get
            {
                if (isUnderWater)
                    return 700.0f;
                else
                    return 1000.0f;
            }
        }

        private float GroundDragFactor
        {
            get
            {
                if (isUnderWater)
                    return 0.28f;
                else
                    return 0.38f;
            }
        }

        private float AirDragFactor
        {
            get { 
                if(isUnderWater)
                    return 0.42f;
                else
                    return 0.48f;
                }
        }

        //private float airDragFactor = 0.48f;
        //private float ClimbAcceleration = 7000.0f;
        //private float MaxClimbSpeed = 1000.0f;

        // Constants for controlling vertical movement
        private float MaxJumpTime
        {
            get
            {
                if (isUnderWater)
                    return 0.25f;
                else
                    return 0.35f;
            }
        }
        private float JumpLaunchVelocity
        {
            get
            {
                if (isUnderWater)
                    return -1500.0f;
                else
                    return -2000.0f;
            }
        }

        private float GravityAcceleration
        {
            get
            {
                if (isUnderWater)
                    return 1000.0f;
                else
                    return 1700.0f;
            }
        }

        private float MaxFallSpeed
        {
            get
            {
                if (isUnderWater)
                    return 200.0f;
                else
                    return 450.0f;
            }
        }

        private float JumpControlPower = 0.13f;

        bool isfacingleft = false;

        // Input configuration
        private float MoveStickScale = 0.0f;
        private Buttons JumpButton = Buttons.B;
        private int LadderAlignment = 12;
        
        // Attacking state
        public bool isAttacking;
        public bool wasAttacking;
        const float MaxAttackTime = 0.33f;
        public float AttackTime;

        /// <summary>
        /// Gets whether or not the player's feet are on the ground.
        /// </summary>
        bool isOnGround;
        public bool IsOnGround
        {
            get { return isOnGround; }
        }

        public bool isTouchingLadder = false;
        private bool wasTouchingLadder = false;

        public TimeSpan InvulnerableTime
        {
            get { return invulnerableTime; }
            set { invulnerableTime = value; }
        }
        TimeSpan invulnerableTime = TimeSpan.Zero;

        public bool Invulnerable
        {
            get { return invulnerableTime != TimeSpan.Zero; }
        }

        /// <summary>
        /// Gets whether the player was jumping on the last update cycle.
        /// </summary>
        private bool wasJumping;
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

        private Vector2 movement;
        // Jumping state
        public bool isJumping;
        public bool isSpringJumping; // If the jump was triggered by a spring
        public bool isClimbingUp;
        public bool isClimbingDown;

        public int CurrentHealth
        {
            get { return currentHealth; }
            set { currentHealth = value; }
        }
        int currentHealth;
        private const int MaxHealth = 5;

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
        public Rectangle MeleeRectangle
        {
            get
            {
                int left =
                     (int)Math.Round(Position.X - sprite.Origin.X) + localBounds.X;
                int top =
                     (int)Math.Round(Position.Y - sprite.Origin.Y) + localBounds.Y;
                if (flip == SpriteEffects.FlipHorizontally)
                    return new Rectangle(
                         (left + localBounds.Width),
                         top,
                         localBounds.Width,
                         localBounds.Height);
                else
                    return new Rectangle(
                         (left - localBounds.Width),
                         top,
                         localBounds.Width,
                         localBounds.Height);
            }
        }

        /// <summary>
        /// Constructor for a new player.
        /// </summary>
        public Player(ScreenManager ThisScreenManager, Level level, Vector2 position, Boolean isGhost)
        {
            screenManager = ThisScreenManager;
            this.level = level;
            this.isGhost = isGhost;

            LoadContent();

            Reset(position, level);

        }

        #region LoadContent
        /// <summary>
        /// Loads the player sprite sheet and sounds.
        /// </summary>
        public void LoadContent()
        {
            idleAnimation = new Animation(Level.screenManager.PlayerIdleTexture, 0.1f, true, screenManager.SkinSettings.FrameWidth_Player_Idle);
            runAnimation = new Animation(Level.screenManager.PlayerRunTexture, 0.1f, true, screenManager.SkinSettings.FrameWidth_Player_Run);
            climbingAnimation = new Animation(Level.screenManager.PlayerClimbingTexture, 0.1f, true, screenManager.SkinSettings.FrameWidth_Player_Climbing);
            jumpAnimation = new Animation(Level.screenManager.PlayerJumpTexture, 0.1f, false, screenManager.SkinSettings.FrameWidth_Player_Jump);
            celebrateAnimation = new Animation(Level.screenManager.PlayerCelebrateTexture, 0.1f, false, screenManager.SkinSettings.FrameWidth_Player_Celebrate);
            dieAnimation = new Animation(Level.screenManager.PlayerDieTexture, 0.1f, false, screenManager.SkinSettings.FrameWidth_Player_Die);
            attackAnimation = new Animation(Level.screenManager.PlayerAttackTexture, 0.1f, false);
            attack_swordAnimation = new Animation(Level.screenManager.PlayerAttack_swordAnimation, 0.1f, false);

            // Load sounds
            powerUpSound = Level.screenManager.PlayerPowerUpSound;
            DiveSound = Level.screenManager.PlayerDiveSound;
            swordSound = Level.screenManager.PlayerSwordSound;
            swordUnderwaterSound = Level.screenManager.PlayerSwordUnderwaterSound;
            arrowSound = Level.screenManager.PlayerArrowSound;
            arrowUnderwaterSound = Level.screenManager.PlayerArrowUnderwaterSound;
            killedSound = Level.screenManager.PlayerKilledSound;
            jumpSound = Level.screenManager.PlayerJumpSound;
            fallSound = Level.screenManager.PlayerFallSound;
            hitSound = Level.screenManager.PlayerHitSound;

            // Calculate bounds within texture size.            
            int width = (int)(idleAnimation.FrameWidth * 0.79);
            //int width = (int)(idleAnimation.FrameWidth * 0.4);
            int left = (idleAnimation.FrameWidth - width) / 2;
            int height = (int)(idleAnimation.FrameWidth * 0.8);

            int top = idleAnimation.FrameHeight - height;
            localBounds = new Rectangle(left, top, width, height);         
        }
        #endregion

        /// <summary>
        /// Resets the player to life.
        /// </summary>
        /// <param name="position">The position to come to life at.</param>
        public void Reset(Vector2 position, Level level)
        {
            Position = position;
            Velocity = Vector2.Zero;
            IsAlive = true;
            CurrentHealth = MaxHealth;
            if (level.Boss != null)
                level.Boss.Position = level.bossStartPosition;
            sprite.PlayAnimation(idleAnimation);
            this.level = level;
        }

        #region Kill and Hurt methods
        /// <summary>
        /// Called when the player gets killed, reguardless of health.
        /// </summary>
        /// <param name="playerKilledSound">If true, player will play killed sound. If false, player will play fall sound.</param>
        public void Kill(bool playerKilledSound)
        {
            Kill(playerKilledSound, true, 0);
        }
        /// <summary>
        /// Called when the player gets killed, reguardless of health.
        /// </summary>
        public void Kill()
        {
            Kill(true, true, 0);
        }
        /// <summary>
        /// Called when the player gets damage inflicted upon himself.
        /// </summary>
        /// <param name="playerKilledSound">If true, player will play killed sound. If false, player will play fall sound.</param>
        /// <param name="Damage">Amount of damage that is inflicted upon the players health.</param>
        public void Hurt(bool playerKilledSound, int Damage)
        {
            Kill(playerKilledSound, false, Damage);
            if (screenManager.IsDemo)
            {
                this.isAttacking = true;
                AttackTime = MaxAttackTime;
                sprite.PlayAnimation(attackAnimation);
                sprite_sword.PlayAnimation(attack_swordAnimation);
            }
        }
        /// <summary>
        /// Called when the player gets damage inflicted upon himself.
        /// </summary>
        /// <param name="Damage">Amount of damage that is inflicted upon the players health.</param>
        public void Hurt(int Damage)
        {
            Kill(true, false, Damage);
        }
        /// <summary>
        /// Called when a player gets damage inflicted upon himself. This will default the amount of damage inflicted to 1.
        /// </summary>
        public void Hurt()
        {
            Kill(true, false, 1);
        }
        /// <summary>
        /// Called when the player is killed.
        /// </summary>
        /// <param name="playKilledSound">If true, player will play killed sound. If false, player will play fall sound.</param>
        /// <param name="dieFully">If true, player will lose an life, reguardless of health. If false, player will only lose health and jump back.</param>
        /// <param name="Damage">Amount of health the player loses.</param>
        public void Kill(bool playKilledSound, bool dieFully, int Damage)
        {
            if (dieFully)
            {
                IsAlive = false;
                if (playKilledSound)
                    killedSound.Play(screenManager.Settings.SoundVolumeAmount, 0, 0);
                else
                    fallSound.Play(screenManager.Settings.SoundVolumeAmount, 0, 0);
                sprite.PlayAnimation(dieAnimation);
            }
            else
            {
                if (InvulnerableTime.Seconds == 0.0)
                {
                    CurrentHealth -= Damage;
                    if (CurrentHealth <= 0)
                    {
                        IsAlive = false;
                        if (playKilledSound)
                            killedSound.Play(screenManager.Settings.SoundVolumeAmount, 0, 0);
                        else
                            fallSound.Play(screenManager.Settings.SoundVolumeAmount, 0, 0);
                        sprite.PlayAnimation(dieAnimation);
                    }
                    else
                    {
                        InvulnerableTime = TimeSpan.FromSeconds(2.0);
                        if (Velocity.Length() > 0)
                        {
                            Velocity = -10 * Velocity;
                        }
                        else
                        {
                            //Velocity = 50 * (Position -);
                        }
                        hitSound.Play(screenManager.Settings.SoundVolumeAmount,0,0);
                    }
                    Trace.Write("\nPlayer Lives: " + CurrentHealth.ToString() + "\nDamage: " + Damage.ToString() + "\n");
                }
            }
        }
        #endregion

        /// <summary>
        /// Gets player horizontal movement and jump commands from input.
        /// </summary>
        private void GetInput()
        {
            if (!screenManager.IsDemo)
            {
                // Get input state.
                gamePadState = GamePad.GetState(PlayerIndex.One);
                keyboardState = Keyboard.GetState();
                // Get analog horizontal movement.
                movement.X = gamePadState.ThumbSticks.Left.X * MoveStickScale;
                movement.Y = gamePadState.ThumbSticks.Left.Y * MoveStickScale;
                
                // Ignore small movements to prevent running in place.
                if (Math.Abs(movement.X) < 0.5f)
                    movement.X = 0.0f;
                if (Math.Abs(movement.Y) < 0.5f)
                    movement.Y = 0.0f;

                // If any digital horizontal movement input is found, override the analog movement.
                if (gamePadState.IsButtonDown(Buttons.DPadLeft) ||
                    keyboardState.IsKeyDown(Keys.Left) ||
                    keyboardState.IsKeyDown(Keys.A))
                {
                    if (isClimbing)
                    {
                        isClimbing = false;
                        wasClimbing = true;
                    }
                    movement.X = -1.0f;
                }
                else if (gamePadState.IsButtonDown(Buttons.DPadRight) ||
                         keyboardState.IsKeyDown(Keys.Right) ||
                         keyboardState.IsKeyDown(Keys.D))
                {
                    if (isClimbing)
                    {
                        isClimbing = false;
                        wasClimbing = true;
                    }
                    movement.X = 1.0f;
                }


                if (gamePadState.IsButtonDown(Buttons.DPadUp) ||
                    keyboardState.IsKeyDown(Keys.Up) ||
                    keyboardState.IsKeyDown(Keys.W))
                {
                    isClimbing = false;
                    if (IsAlignedToLadder())
                    {
                        // We need to check the tile behind the player, not what he is
                        // standing on
                        if (level.GetTileCollisionBehindPlayer(screenManager.Player.Position) == TileCollision.Ladder)
                        {
                            isClimbing = true;
                            isJumping = false;
                            isOnGround = false;
                            movement.Y = -1.0f;
                        }
                    }
                }
                else if (gamePadState.IsButtonDown(Buttons.DPadDown) ||
                   keyboardState.IsKeyDown(Keys.Down) ||
                   keyboardState.IsKeyDown(Keys.S))
                {
                    isClimbing = false;

                    if (IsAlignedToLadder())
                    {
                        // Check the tile the player is standing on
                        if (level.GetTileCollisionAtPosition(screenManager.Player.Position) == TileCollision.Ladder)
                        {
                            isClimbing = true;
                            isJumping = false;
                            isOnGround = false;
                            movement.Y = 1.0f;
                        }
                    }
                }

                isJumping =
                    gamePadState.IsButtonDown(JumpButton) ||
                    keyboardState.IsKeyDown(Keys.Space);

                // Deal with non-user (in-game generated) input ---------------------- ***
                // If the player is on a spring, he "jumps"
                if (isSpringJumping)
                    isJumping = true;
                //--------------------------------------------------------------------

                if ((gamePadState.IsButtonDown(Buttons.RightTrigger) || keyboardState.IsKeyDown(Keys.X)) && (previousGamePadState.IsButtonUp(Buttons.RightTrigger) && (previousKeyboardState.IsKeyUp(Keys.X))))
                {
                    if (AttackTime != MaxAttackTime)
                    {
                        isAttacking = true;
                        AttackTime = MaxAttackTime;
                    }
                }

                if ((gamePadState.IsButtonDown(Buttons.LeftTrigger) || keyboardState.IsKeyDown(Keys.C)) && (previousGamePadState.IsButtonUp(Buttons.LeftTrigger) && (previousKeyboardState.IsKeyUp(Keys.C))))
                {
                    level.addArrow(new Vector2(this.position.X, this.position.Y - screenManager.PlayerIdleTexture.Height / 2), isfacingleft);
                    if (isUnderWater)
                        arrowUnderwaterSound.Play(screenManager.Settings.SoundVolumeAmount, 0, 0);
                    else
                        arrowSound.Play(screenManager.Settings.SoundVolumeAmount, 0, 0);

                }
                
                previousGamePadState = GamePad.GetState(PlayerIndex.One);
                previousKeyboardState = Keyboard.GetState(PlayerIndex.One);
            }
            else  // => screenManager.IsDemo
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
                previousGamePadState = GamePad.GetState(PlayerIndex.One);
                previousKeyboardState = Keyboard.GetState(PlayerIndex.One);
            }
            if (isClimbing)
                wasTouchingLadder = isTouchingLadder;

            if (level.isUpsideDown)
                movement.X *= -1;
        }


        private bool IsAlignedToLadder()
        {
            if (gamePadState.IsButtonDown(Buttons.DPadRight) ||
                         keyboardState.IsKeyDown(Keys.Right) ||
                         keyboardState.IsKeyDown(Keys.D) ||
                         gamePadState.IsButtonDown(Buttons.DPadLeft) ||
                         keyboardState.IsKeyDown(Keys.Left) ||
                         keyboardState.IsKeyDown(Keys.A))
            {
                return false;

            }
            else
            {
                float PrevPosition = position.X;
                int playerOffset = ((int)position.X % Tile.Width) - Tile.Center;

                if (Math.Abs(playerOffset) <= LadderAlignment)
                {
                    // Align the player with the middle of the tile
                    position.X -= playerOffset;
                    return true;
                }
                else
                {
                    position.X = PrevPosition;
                    return false;
                }
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

        public void DoSpringJump(GameTime gameTime)
        {
            isJumping = true;
            isSpringJumping = true;
            DoJump(JumpLaunchVelocity, gameTime);

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
              //  if (isJumping) /// && !isClimbing) Allows jumping while climbing
                {
               
                // Begin or continue a jump
                if ((!wasJumping && IsOnGround ) || jumpTime > 0.0f)
                    //if ((!wasJumping && (IsOnGround || isClimbing)) || jumpTime > 0.0f)
                    {
                        if (jumpTime == 0.0f)
                        {
                            if(!isSpringJumping)
                                jumpSound.Play(screenManager.Settings.SoundVolumeAmount, 0, 0);
                        }
                    jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    sprite.PlayAnimation(jumpAnimation);
                }

                if (0.0f < jumpTime && jumpTime <= MaxJumpTime)
                {
                    // Fully override the vertical velocity with a power curve that gives players more control over the top of the jump
                    if(!isSpringJumping)
                        velocityY = JumpLaunchVelocity * (1.0f - (float)Math.Pow(jumpTime / MaxJumpTime, JumpControlPower));
                    else // Spring jumps have double velocity
                        velocityY = (JumpLaunchVelocity *  2.5f) * (1.0f - (float)Math.Pow(jumpTime / MaxJumpTime, JumpControlPower));
                }
                else
                {
                    // Reached the apex of the jump and has double jumps
                    //if velocityY <> 0 to double jump on descent
                    if (velocityY <= -MaxFallSpeed * 1.0f && !wasJumping && numJumps < MaxDoubleJumps)
                    {
                        if (!isSpringJumping)
                            jumpSound.Play(screenManager.Settings.SoundVolumeAmount, 0, 0);

                        velocityY = JumpLaunchVelocity * (1.0f - (float)Math.Pow(jumpTime / MaxJumpTime, JumpControlPower));
                        jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds; 
                        sprite.PlayAnimation(jumpAnimation); 
                        numJumps++;
                    }
                    else
                    {
                        jumpTime = 0.0f;
                        isSpringJumping = false;
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

        private void DoAttack(GameTime gameTime)
        {
            // If the player wants to attack
            if (isAttacking)
            {
                // Begin or continue an attack
                if (AttackTime > 0.0f)
                {
                    AttackTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    sprite.PlayAnimation(attackAnimation);
                    sprite_sword.PlayAnimation(attack_swordAnimation);
                    if (!wasAttacking)
                    {
                        if (isUnderWater)
                            swordUnderwaterSound.Play(screenManager.Settings.SoundVolumeAmount, 0, 0);
                        else
                            swordSound.Play(screenManager.Settings.SoundVolumeAmount, 0, 0);
                        wasAttacking = true;
                    }
                }
                else
                {
                    isAttacking = false;
                    sprite_sword.ResetAnimation(attack_swordAnimation);
                }
            }
            else
            {
                AttackTime = 0.0f;
                wasAttacking = false;
            }
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
            int leftTile = (int)Math.Floor((float)bounds.Left / Tile.Width);
            int rightTile = (int)Math.Ceiling(((float)bounds.Right / Tile.Width)) - 1;
            int topTile = (int)Math.Floor((float)bounds.Top / Tile.Height);
            int bottomTile = (int)Math.Ceiling(((float)BoundingRectangle.Bottom / Tile.Height)) - 1;

            //if(screenManager.IsDemo)
            //    bottomTile = (int)Math.Ceiling(((float)(bounds.Bottom) / Tile.Height) -0.125) - 1; // XXX DEBUG Bacalhau - apagar

            // Reset flag to search for ground collision.
            isOnGround = false;

            //For each potentially colliding arrow.   
            foreach (Arrow arrow in level.arrows)
                bounds = HandleCollision(bounds, TileCollision.Platform, arrow.BoundingRectangle);

            // Only the real player uses moving platforms and springs
            // "Cloned" ghost do not
            if (!IsGhost)
            {
                //For each potentially colliding moving item 
                foreach (MovingItem movingItem in level.MovingItems)
                {
                    // Reset flag to search for moving item  collision.   
                    movingItem.PlayerIsOn = false;

                    //check to see if player is on moving item     
                    if ((BoundingRectangle.Bottom == movingItem.BoundingRectangle.Top + 2) &&
                        (BoundingRectangle.Left >= movingItem.BoundingRectangle.Left - (BoundingRectangle.Width / 2) &&
                        BoundingRectangle.Right <= movingItem.BoundingRectangle.Right + (BoundingRectangle.Width / 2)))
                    {
                        movingItem.PlayerIsOn = true;
                    }
                    bounds = HandleCollision(bounds, TileCollision.Impassable, movingItem.BoundingRectangle);
                }

                //For each potentially colliding spring.   
                foreach (Spring spring in level.Springs)
                {
                    // Reset flag to search for spring collision.   
                    spring.PlayerIsOn = false;

                    //check to see if player is on tile.    
                    if ((BoundingRectangle.Bottom == spring.BoundingRectangle.Top + 2) &&
                        (BoundingRectangle.Left >= spring.BoundingRectangle.Left - (BoundingRectangle.Width / 2) &&
                        BoundingRectangle.Right <= spring.BoundingRectangle.Right + (BoundingRectangle.Width / 2)))
                    {
                        spring.PlayerIsOn = true;
                    }
                    bounds = HandleCollision(bounds, TileCollision.Impassable, spring.BoundingRectangle);
                }
            }


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
                                        if (screenManager.IsDemo)
                                            isClimbing = true;
                                    }
                                    else
                                    {
                                        //In the demo that plays in the background of the menu,
                                        // we want the person to go left if he hits a wall on the right.
                                    //    if (Math.Abs(tileBounds.Right - previousRight) >= Tile.Width /4 )
                                     //   if (tileBounds.Right == previousRight)
                                        if (tileBounds.Right == previousRight)
                                            demogoleft = true;

                                        //In the demo that plays in the background of the menu,
                                        // we want the person to go right if he hits a wall on the left.
                                       // if (Math.Abs(tileBounds.Left - previousLeft) >= Tile.Width / 4 )
                          //              if (tileBounds.Left == previousLeft) 
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

        private Rectangle HandleCollision(Rectangle bounds, TileCollision collision, Rectangle tileBounds)
        {
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
                        isOnGround = true;

                    // Ignore platforms, unless we are on the ground.   
                    if (collision == TileCollision.Impassable || IsOnGround)
                    {
                        // Resolve the collision along the Y axis.   
                        Position = new Vector2(Position.X, Position.Y + depth.Y);

                        // Perform further collisions with the new bounds.   
                        bounds = BoundingRectangle;
                    }
                }
                else if (collision == TileCollision.Impassable) // Ignore platforms.   
                {
                    // Resolve the collision along the X axis.   
                    Position = new Vector2(Position.X + depth.X, Position.Y);

                    // Perform further collisions with the new bounds.   
                    bounds = BoundingRectangle;
                }
            }
            return bounds;
        }  


        /// <summary>
        /// Called when the player has lost
        /// </summary>
        /// <param name="killedBy">
        /// The enemy who killed the player. This parameter is null if the player was
        /// not killed by an enemy (fell into a hole).
        /// </param>
        /// <param name="playKillSound">
        /// This tells weather to play the sound of being killed or the sound of falling. if
        /// this is true, the killed sound will play, if this is false, the fall sound will play.
        /// </param>
        public void OnLose(Enemy killedBy, bool playKillSound)
        {
        //    level.TimeRemaining = TimeSpan.Zero;
            IsAlive = false;
        }

        /// <summary>
        /// Called when this player reaches the level's exit.
        /// </summary>
        public void OnReachedExit()
        {
            if (IsPoweredUp)
            {
                //powerUpSound.Dispose();
                IsPoweredUp = false;
            }
            //sprite.PlayAnimation(celebrateAnimation);  // Do NOT celebrate in the end of each level!
        }

        public void PowerUp()
        {
            powerUpTime = MaxPowerUpTime;

            powerUpSound.Play(screenManager.Settings.SoundVolumeAmount, 0, 0);
        }

        public void Submerge()
        {
            if (hasOxygen)
                UnderwaterTime = MaxUnderwaterTime * 45;
            else
                UnderwaterTime = MaxUnderwaterTime;

            DiveSound.Play(screenManager.Settings.SoundVolumeAmount, 0, 0);
        }

        #region Update

        public void Update(GameTime gameTime)
        {
            GetInput();
            DoAttack(gameTime);

            // Checks if the player is underwater and checks if he is still alive
            if (level.GetTileTypeAtPosition(this.Position) == TileType.Water)
            {
                isUnderWater = true;
                if (!wasUnderWater)
                {
                    Submerge();
                    wasUnderWater = true;
                }
            }
            else
            {
                isUnderWater = false;
                wasUnderWater = false;
            }
            if (isUnderWater && IsOutOfOxygen)
                Kill();


            if (currentHealth > MaxHealth)
            {
                lives += 1;
                currentHealth = 1;
            }

            ApplyPhysics(gameTime);
            if (IsPoweredUp)
                powerUpTime = Math.Max(0.0f, powerUpTime - (float)gameTime.ElapsedGameTime.TotalSeconds);

            if (isUnderWater)
                UnderwaterTime = Math.Max(0.0f, UnderwaterTime - (float)gameTime.ElapsedGameTime.TotalSeconds);

            if (IsAlive)
            {
                if (isOnGround)
                {
                    if (isAttacking)
                    {
                        sprite.PlayAnimation(attackAnimation);
                        sprite_sword.PlayAnimation(attack_swordAnimation);
                    }
                    else
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
                }
                else if (isClimbing)
                {
                    //   if (Math.Abs(Velocity.Y) - 0.02f > 0)
                    //   {
                    sprite.PlayAnimation(climbingAnimation);
                    //   }
                    //   else
                    //   {
                    //  sprite.PlayAnimation(idleAnimation);
                    //   }
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

            if (invulnerableTime.Seconds > 0)
                invulnerableTime -= gameTime.ElapsedGameTime;
            else
                invulnerableTime = TimeSpan.FromSeconds(0.0);
        }

        #endregion

        #region Draw

        /// <summary>
        /// Draws the animated player.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Color color)
        {
            // Flip the sprite to face the way we are moving.
            if (Velocity.X > 0)
            {
                flip = SpriteEffects.FlipHorizontally;
                isfacingleft = true;
            }
            else if (Velocity.X < 0)
            {
                flip = SpriteEffects.None;
                isfacingleft = false;
            }
            // Calculate a tint color based on power up state.
            if (IsPoweredUp)
            {
                float t = ((float)gameTime.TotalGameTime.TotalSeconds + powerUpTime / MaxPowerUpTime) * 20.0f;
                int colorIndex = (int)t % poweredUpColors.Length;
                color = poweredUpColors[colorIndex];
            }
            else
            {
                if (Invulnerable)
                    color = Color.Gray;
                else if (isUnderWater)
                {
                    color = Color.Aqua;
                }
            }

            if (isAttacking)
            {
                if (flip == SpriteEffects.FlipHorizontally)
                    sprite_sword.Draw(gameTime, spriteBatch, new Vector2 (Position.X + Tile.Width, Position.Y), flip, color);
                else
                    sprite_sword.Draw(gameTime, spriteBatch, new Vector2(Position.X - Tile.Width, Position.Y), flip, color);
            }            
            // Draw that sprite.
            sprite.Draw(gameTime, spriteBatch, Position, flip, color);

            // if the player is underwater, draw remaining oxygen bar
            if (isUnderWater)
            {
                int remainingOxygen = 0;
                if(hasOxygen)
                    remainingOxygen = (int) (UnderwaterTime * BoundingRectangle.Width / (MaxUnderwaterTime* 45)); 
                else
                    remainingOxygen = (int) (UnderwaterTime * BoundingRectangle.Width / MaxUnderwaterTime);

                Rectangle oxygenBar = new Rectangle((int)BoundingRectangle.X, (int)BoundingRectangle.Y - Tile.Height, remainingOxygen, 4);
                spriteBatch.Draw(screenManager.BlankTexture, oxygenBar, Color.Cyan);
            }
            // Draw the sword if attacking
            if (screenManager.Settings.DebugMode) //  Show bounding box if debugging
            {
                spriteBatch.Draw(screenManager.BlankTexture, BoundingRectangle, new Color(0, 0, 255, 155));
                // Draw the tiles considered in collision test
                int leftTile = (int)Math.Floor((float)BoundingRectangle.Left / Tile.Width);
                int rightTile = (int)Math.Ceiling(((float)BoundingRectangle.Right / Tile.Width)) - 1;
                int topTile = (int)Math.Floor((float)BoundingRectangle.Top / Tile.Height);
                int bottomTile = (int)Math.Ceiling(((float)BoundingRectangle.Bottom / Tile.Height)) - 1;
                spriteBatch.Draw(screenManager.BlankTexture, Level.GetBounds(leftTile, topTile), new Color(125, 0, 0, 125));
                spriteBatch.Draw(screenManager.BlankTexture, Level.GetBounds(leftTile, bottomTile), new Color(125, 0, 0, 125));
                spriteBatch.Draw(screenManager.BlankTexture, Level.GetBounds(rightTile, topTile), new Color(125, 0, 0, 125));
                spriteBatch.Draw(screenManager.BlankTexture, Level.GetBounds(rightTile, bottomTile), new Color(125, 0, 0, 125));
                screenManager.DrawShadowedString(spriteBatch, screenManager.Font, "Oxygen Left: " + UnderwaterTime, new Vector2(BoundingRectangle.Right, BoundingRectangle.Top), Color.White); 
            }


        }
        
        #endregion

    }
}
