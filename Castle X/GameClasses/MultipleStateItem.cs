using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System.Diagnostics;

namespace CastleX
{
    enum MultipleStateItemType
    {
        Lever = 1,
        BlueDoor = 2,
        GreenDoor = 3,
        RedDoor = 4,
        YellowDoor = 5
    }

    
    /// <summary>
    /// A valuable item the player can collect.
    /// </summary>
    class MultipleStateItem
    {
        ScreenManager screenManager;

        private Animation spriteSheet;
        private AnimationPlayer sprite;
        private SoundEffect ChangeStateSound;
        private Boolean touching = false;  // controls if the item already changed its state
        private Boolean collected = false;
        private int tileX, tileY; // tile array index for the item (used to change tile type by doors)

        public MultipleStateItemType ItemType
        {
            get;
            set;
        }

        /// <summary>
        /// Current state (index of sprite on spritesheet)
        /// </summary>
        public int State
        {
            get;
            set;
        }

        private Vector2 Position;

        public Level Level
        {
            get { return level; }
        }
        Level level;


        /// <summary>
        /// Gets a circle which bounds this item in world space.
        /// </summary>
        //public Circle BoundingCircle
        //{
        //    get
        //    {
        //        return new Circle(Position, Tile.Width / 3.0f);
        //    }
        //}

        /// <summary>
        /// Gets a rectangle which bounds this item in world space.  Width = height in bounding rectangle, although they
        ///   are different on spritesheet (width is a multiple of height)
        /// </summary>
        public Rectangle BoundingRectangle
        {
            get
            {
                int left = (int)Math.Round(Position.X - sprite.Origin.X) ;
                int top = (int)Math.Round(Position.Y - sprite.Origin.Y) ;

                return new Rectangle(left, top, spriteSheet.FrameWidth, spriteSheet.FrameHeight);
            }
        }

        public MultipleStateItem(ScreenManager ThisScreenManager, Level level,int x, int y, MultipleStateItemType spriteName)
        {                   
            Point position = level.GetBounds(x, y).Center;
            tileX = x; tileY = y;
            screenManager = ThisScreenManager;
            this.level = level;
            this.ItemType = spriteName;
            LoadContent(spriteName, new Vector2(position.X, position.Y));
           
        }


        /// <summary>
        /// Loads the item texture and collected sound.
        /// </summary>
        public void LoadContent(MultipleStateItemType spriteName, Vector2 position)
        {
           ChangeStateSound = null;            
            switch (spriteName)
            {
                case MultipleStateItemType.Lever:
                    spriteSheet = new Animation(Level.screenManager.Lever, 0.1f, false);
                    ChangeStateSound = level.screenManager.CoinCollectedSound;
                    this.Position = new Vector2(position.X, position.Y + Tile.Height/2 );
                    // If there is a lever on the level, the movable tiles are stopped by default
                    level.MovableTilesAreActive = false;
                    break;
                case MultipleStateItemType.YellowDoor:
                    spriteSheet = new Animation(Level.screenManager.YellowDoor, 0.1f, false, 32);
                    this.Position = new Vector2(position.X , position.Y + Tile.Height / 2);
                    break;
                case MultipleStateItemType.RedDoor:
                    spriteSheet = new Animation(Level.screenManager.RedDoor, 0.1f, false, 32);
                    this.Position = new Vector2(position.X, position.Y + Tile.Height / 2);
                    break;
                case MultipleStateItemType.GreenDoor:
                    spriteSheet = new Animation(Level.screenManager.GreenDoor, 0.1f, false, 32);
                    this.Position = new Vector2(position.X, position.Y + Tile.Height / 2);
                    break;
                case MultipleStateItemType.BlueDoor:
                    spriteSheet = new Animation(Level.screenManager.BlueDoor, 0.1f, false, 32);
                    this.Position = new Vector2(position.X, position.Y + Tile.Height / 2);
                    break;
            }
            sprite.PlayAnimation(spriteSheet);  // Sets the spritesheet to draw
        }

        /// <summary>
        /// Called when this item has been touched by a player 
        /// </summary>
        /// <param name="collectedBy">
        /// The player who touched this item. Although currently not used, this parameter would be
        /// useful for creating special effects.
        /// </param>
        public void OnCollected(Player collectedBy, int itemnumber)
        {
            switch (ItemType)
            {
                case MultipleStateItemType.Lever:
                    if (!collected)
                    {
                        collected = true;
                        sprite.FrameIndex++;
                        if (sprite.FrameIndex > spriteSheet.FrameCount - 1)
                            sprite.FrameIndex = 0;
                        level.MovableTilesAreActive = !level.MovableTilesAreActive;
                    }
                    break;
                case MultipleStateItemType.YellowDoor:
                    if (sprite.FrameIndex == 0)  // If the door is closed...
                    {
                        if (collectedBy.YellowKeys > 0)
                        {
                            collectedBy.YellowKeys--;
                            sprite.FrameIndex = 1; // only changes the state (to "open door") once
                            //level.ChangeTileCollision(tileX, tileY, TileCollision.Passable);
                            // make passable the invisible obstacles after the door 
                            level.ChangeTileCollision(tileX + 1, tileY, TileCollision.Passable);
                            level.ChangeTileCollision(tileX + 1, tileY-1, TileCollision.Passable);
                        }
                        //else
                        //{
                        //    collectedBy.Position -= collectedBy.Velocity;
                        //    collectedBy.Velocity = Vector2.Zero;
                        //}
                    }
                    break;

                case MultipleStateItemType.RedDoor:
                    if (collectedBy.RedKeys > 0)
                    {
                        collectedBy.RedKeys--;
                        sprite.FrameIndex = 1; // only changes the state (to "open door") once
                        level.ChangeTileCollision(tileX, tileY, TileCollision.Passable);
                        level.ChangeTileCollision(tileX, tileY - 2, TileCollision.Passable);
                    }
                    break;
                case MultipleStateItemType.GreenDoor:
                    if (collectedBy.GreenKeys > 0)
                    {
                        collectedBy.GreenKeys--;
                        sprite.FrameIndex = 1; // only changes the state (to "open door") once
                        level.ChangeTileCollision(tileX, tileY, TileCollision.Passable);
                        level.ChangeTileCollision(tileX, tileY - 2, TileCollision.Passable);
                    }
                    break;

                case MultipleStateItemType.BlueDoor:
                    if (collectedBy.BlueKeys > 0)
                    {
                        collectedBy.BlueKeys--;
                        sprite.FrameIndex = 1; // only changes the state (to "open door") once
                        level.ChangeTileCollision(tileX, tileY, TileCollision.Passable);
                        level.ChangeTileCollision(tileX, tileY - 2, TileCollision.Passable);
                    }
                    break;

            }
        }

        //  Do any action needed when object is NOT touched by an arrow
        public void OnNotTouched()
        {
            touching = false;  
        }

        //  Do any action needed when object is NOT touched by the player
        public void OnNotCollected()
        {
            collected = false;
        }
        //  Do any action needed when object is touched by an arrow
        public void onTouched()
        {
            if (!touching)
            {
                touching = true;
                switch (ItemType)
                {
                    case MultipleStateItemType.Lever:
                        sprite.FrameIndex++;
                        if (sprite.FrameIndex > spriteSheet.FrameCount - 1)
                            sprite.FrameIndex = 0;
                        level.MovableTilesAreActive = !level.MovableTilesAreActive;
                        break;
                }
            }
        }

        private void PlaySound()
        {
            try
            {
                if(ChangeStateSound != null)
                    ChangeStateSound.Play(screenManager.Settings.SoundVolumeAmount, 0, 0);
            }
            catch { }
        }

        //public void Update(GameTime gameTime)
        //{
        //    //elapsedTime += gameTime.ElapsedGameTime;
        //    //if (elapsedTime > TimeSpan.FromSeconds(0.5))
        //    //{
        //    //    changeableState = true;  // can only change states once a half second
        //    //}
        //}

        /// <summary>
        /// Draws the item in the appropriate color.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            sprite.DrawSprite(gameTime, spriteBatch, Position, SpriteEffects.None);

            if (screenManager.Settings.DebugMode) //  Show bounding box if debugging
                spriteBatch.Draw(screenManager.BlankTexture, BoundingRectangle, new Color(255, 0, 0, 155)); 
        }


    }
}
