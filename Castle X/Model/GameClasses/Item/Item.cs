using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System.Diagnostics;

namespace CastleX
{
    public enum ItemType
    {
        Coin = 1,
        Powerup = 2,
        Life = 4,
        Oxygen = 5,
        Trunk = 6,
        YellowKey = 7,
        BlueKey = 8,
        RedKey = 9,
        GreenKey = 10,
        Map = 11,
        MagicMirror = 12
    }

    
    /// <summary>
    /// A valuable item the player can collect.
    /// </summary>
    public class Item
    {
        ScreenManager screenManager;

        private Texture2D texture;
        private Vector2 origin;
        private int width, height;
        private SoundEffect collectedSound;

        public readonly int PointValue;
        public readonly Color Color;

        public ItemType ItemType
        {
            get;
            set;
        }

        // The item is animated from a base position along the Y axis.
        private Vector2 basePosition;
        private float bounce;
        public bool isBouncing;

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

        /// <summary>
        /// Gets a circle which bounds this item in world space.
        /// </summary>
        public Rectangle BoundingRectangle
        {
            get
            {
                return new Rectangle((int)Position.X - (width / 2), (int)Position.Y - (height / 2), width, height);
            }
        }

        bool ischeckpointcurrent = false;

        public Item(ScreenManager ThisScreenManager, Level level, Vector2 position, ItemType itemObject)
        {
            screenManager = ThisScreenManager;
            this.level = level;
            this.basePosition = position;
            this.ItemType = itemObject;

            texture = screenManager.CoinTexture;
            width = texture.Width;
            height = texture.Height;
            origin = new Vector2(width / 2.0f, height / 2.0f);

            collectedSound = level.screenManager.CoinCollectedSound;
            Color = Color.White;  // most items don't use a tint color
            isBouncing = true;    // most items do bounce

            switch (ItemType)
            {
                case ItemType.Trunk:
                    texture = screenManager.TrunkTexture;
                    isBouncing = false;
                    break;
                case ItemType.Coin:
                    PointValue = 30;
                    texture = screenManager.CoinTexture;
                    Color = Color.Yellow;
                    break;
                case ItemType.Powerup:
                    PointValue = 100;
                    texture = screenManager.CoinTexture;
                    Color = Color.Red;
                    break;
                case ItemType.Life:
                    texture = screenManager.LifeTexture;
                    break;
                case ItemType.Oxygen:
                    texture = screenManager.OxygenTexture;
                    break;
                case ItemType.BlueKey:
                    texture = screenManager.BlueKeyTexture;
                    break;
                case ItemType.YellowKey:
                    texture = screenManager.YellowKeyTexture;
                    break;
                case ItemType.RedKey:
                    texture = screenManager.RedKeyTexture;
                    break;
                case ItemType.GreenKey:
                    texture = screenManager.GreenKeyTexture;
                    break;
                case ItemType.Map:
                    texture = screenManager.MapTexture;
                    break;
                case ItemType.MagicMirror:
                    texture = screenManager.MagicMirrorTexture;
                    break;
            }
        }

        public void onTouched()
        {
            // ***TODO: Any action when object is touched by an arrow or something else (player COLLECTS the object)
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
                bounce = (float)Math.Sin(t) * BounceHeight * texture.Height;
            else
                bounce = 0 - texture.Height/4;

        }

        /// <summary>
        /// Called when this item has been collected by a player and removed from the level.
        /// </summary>
        /// <param name="collectedBy">
        /// The player who collected this item. Although currently not used, this parameter would be
        /// useful for creating special powerup itens. For example, a item could make the player invincible.
        /// </param>
        public void OnCollected(Player collectedBy, int itemnumber)
        {
            switch (ItemType)
            {
                case ItemType.Trunk:
                    int checkpointnumber = 0;
                    //Set every other checkpoint to false so only one would be selected.
                    foreach (Item item in level.items)
                    {
                        //Make sure the currently touched checkpoint doesnt get modified.
                        if (checkpointnumber != itemnumber)
                        {
                            if (item.ischeckpointcurrent)
                            {
                                item.ischeckpointcurrent = false;
                                item.texture = screenManager.TrunkTexture;
                            }
                        }
                        checkpointnumber++;
                    }
                    if (!this.ischeckpointcurrent)
                    {
                        level.Checkpoint = this.Position;
                        this.ischeckpointcurrent = true;
                        this.texture = screenManager.Checkpoint2Texture;
                        PlaySound();
                    }
                    checkpointnumber = 0;
                    break;

                case ItemType.Coin:
                    level.items.RemoveAt(itemnumber--);
                    collectedBy.Score += this.PointValue;
                    level.CoinsRemaining -= 1;
                    PlaySound();
                    break;

                case ItemType.Powerup:
                    level.items.RemoveAt(itemnumber--);
                    collectedBy.PowerUp();
                    PlaySound();
                    break;

                case ItemType.Life:
                    level.items.RemoveAt(itemnumber--);
                    collectedBy.Lives += 1;
                    PlaySound();
                    break;
                case ItemType.Oxygen:
                    level.items.RemoveAt(itemnumber--);
                    collectedBy.hasOxygen = true;
                    PlaySound();
                    break;
                case ItemType.YellowKey:
                    level.items.RemoveAt(itemnumber--);
                    collectedBy.YellowKeys++;
                    PlaySound();
                    break;
                case ItemType.BlueKey:
                    level.items.RemoveAt(itemnumber--);
                    collectedBy.BlueKeys++;
                    PlaySound();
                    break;
                case ItemType.GreenKey:
                    level.items.RemoveAt(itemnumber--);
                    collectedBy.GreenKeys++;
                    PlaySound();
                    break;
                case ItemType.RedKey:
                    level.items.RemoveAt(itemnumber--);
                    collectedBy.RedKeys++;
                    PlaySound();
                    break;
                case ItemType.Map:
                    level.items.RemoveAt(itemnumber--);
                    collectedBy.hasMap = true;
                    PlaySound();
                    break;
                case ItemType.MagicMirror:
                    level.items.RemoveAt(itemnumber--);
                    level.onMagicMirrorPlayerCloned();
                    PlaySound();
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
            spriteBatch.Draw(texture, Position, null, Color, 0.0f, origin, 1.0f, SpriteEffects.None, 0.0f);
        }
    }
}
