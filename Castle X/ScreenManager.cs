#region File Description
//-----------------------------------------------------------------------------
// ScreenManager.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Media;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Audio;
using System.Threading;
using Microsoft.Xna.Framework.Input;
#endregion

namespace CastleX
{
    /// <summary>
    /// The screen manager is a component which manages one or more GameScreen
    /// instances. It maintains a stack of screens, calls their Update and Draw
    /// methods at the appropriate times, and automatically routes input to the
    /// topmost active screen.
    /// </summary>
    public class ScreenManager : DrawableGameComponent
    {

        #region Fields

        public List<GameScreen> screens = new List<GameScreen>();
        public List<GameScreen> screensToUpdate = new List<GameScreen>();

        bool isInitialized;

        bool isRotated = false;
        bool traceEnabled = false;
        
        #endregion

        #region Global Properties
        /// <summary>
        /// A default SpriteBatch shared by all the screens. This saves
        /// each screen having to bother creating their own local instance.
        /// </summary>
        public SpriteBatch SpriteBatch { get; set; }
        #region bool
        public bool IsDemo { get; set; }
        public bool IsRotated { get { return isRotated; } set { isRotated = value; } }
        public bool isRunningSlow { get; set; }
        #endregion
        #region int
        public int FPS { get; internal set; }
        public int LevelIndex { get; set; }
        public int LevelNumber { get; set; }
        public int MaxLevels { get; set; }
        public int MaxLevelsFromZero { get; set; }
        public int NumberOfLevels { get; set; }
        public const float HUDHeight = 80;
        #endregion

        /// <summary>
        /// A single player is shared through all levels
        /// </summary>
        public Player Player
        {
            get { return player; }
            set { player = value; }
        }
        private Player player = null;

        public ContentManager Content { get; set; }
        public ContentManager GraphicContent { get; set; }
        public InputState Input { get; set; }
        public PESettings Settings { get; set; }
        public CastleXGame FrontGamePage { get; set; }
        public SkinSettings SkinSettings { get; set; }

        #region Background Music and Sound Effects
        public Song BackgroundMusic { get; set; }
        public SoundEffect ExitReachedSound { get; set; }
        public SoundEffect CoinCollectedSound { get; set; }
        public SoundEffect SpringSound { get; set; }
        public SoundEffect OptionsSound { get; set; }
        public SoundEffect MonsterKilledSound { get; set; }
        public SoundEffect FlyingEnemyKilledSound { get; set; }
        public SoundEffect GhostKilledSound { get; set; }
        public SoundEffect ArrowHitSound { get; set; }
        // Player sound effects
        public SoundEffect PlayerFallSound { get; set; }
        public SoundEffect PlayerJumpSound { get; set; }
        public SoundEffect PlayerHitSound { get; set; }
        public SoundEffect PlayerKilledSound { get; set; }
        public SoundEffect PlayerPowerUpSound { get; set; }
        public SoundEffect PlayerDiveSound { get; set; }
        public SoundEffect PlayerSwordSound { get; set; }
        public SoundEffect PlayerSwordUnderwaterSound  {get; set; }
        public SoundEffect PlayerArrowSound { get; set; }
        public SoundEffect PlayerArrowUnderwaterSound { get; set; }

        #endregion
        #region SpriteFont
        /// <summary>
        /// A default font shared by all the screens. This saves
        /// each screen having to bother loading their own local copy.
        /// </summary>
        public SpriteFont Font { get { return HudFont; } }
        public SpriteFont MainMenuFont { get { return HudBig; } }
        public SpriteFont SmallFont { get { return HudSmall; } }
        public SpriteFont SubMainMenuFont { get { return HudMedium; } }
        public SpriteFont Courier10 { get; set; }
        public SpriteFont Courier10Bold { get; set; }
        public SpriteFont Courier8 { get; set; }
        public SpriteFont Courier8Bold { get; set; }
        public SpriteFont HudFont { get; set; }
        public SpriteFont HudBig { get; set; }
        public SpriteFont HudMedium { get; set; }
        public SpriteFont HudSmall { get; set; }
        public SpriteFont Segoe12 { get; set; }
        #endregion
        public StorageDevice Device { get; set; }

        #region debug and helper textures
        public Texture2D BlankTexture { get; set; }
        public Texture2D CrossHairTexture { get; set; }
        public Texture2D SpacerTexture { get; set; }

        #endregion

        #region Texture2D
        public Texture2D ArrowTexture { get; set; }
        public Texture2D FallingTileTexture { get; set; }
        public Texture2D HUDTexture { get; set; }
        public Texture2D BannerTexture { get; set; }
        public Texture2D[] BlockATexture { get; set; }
        public Texture2D[] BlockBTexture { get; set; }
        public Texture2D MoveablePlatformTexture { get; set; }         // Moveable tiles
        public Texture2D SpringTexture { get; set; }
        public Texture2D StoppedSpringTexture { get; set; }
        public Texture2D TrunkTexture { get; set; }
        public Texture2D Checkpoint2Texture { get; set; }
        public Texture2D DefaultBGTexture { get; set; }
        public Texture2D ExitTexture { get; set; }
        public Texture2D YellowKeyTexture { get; set; }
        public Texture2D GreenKeyTexture { get; set; }
        public Texture2D BlueKeyTexture { get; set; }
        public Texture2D RedKeyTexture { get; set; }
        public Texture2D HUDOxygenTexture { get; set; }
        public Texture2D HUDCandleTexture { get; set; }
        public Texture2D HUDOpenMapTexture { get; set; }
        public Texture2D HUDClosedMapTexture { get; set; }        
        public Texture2D MapTexture { get; set; }
        public Texture2D CoinTexture { get; set; }
        public Texture2D GradientTexture { get; set; }
        public Texture2D HealthPotionTexture { get; set; }
        public Texture2D LadderTexture { get; set; }
        public Texture2D RopeTexture { get; set; }
        public Texture2D WoodBlockTexture { get; set; }
        public Texture2D LifeTexture { get; set; }
        public Texture2D PlatformTexture { get; set; }
        public Texture2D OxygenTexture { get; set; }
        public Texture2D MagicMirrorTexture { get; set; }
        // Terrain Tiles
        public Texture2D[] ClayTerrain { get; set; }
        public Texture2D[] ClaySurface { get; set; }
        public Texture2D[] ClayCeiling { get; set; }
        public Texture2D[] MudTerrain { get; set; }
        public Texture2D[] MudSurface { get; set; }
        public Texture2D[] MudCeiling { get; set; }
        public Texture2D[] RockTerrain { get; set; }
        public Texture2D[] RockSurface { get; set; }
        public Texture2D[] RockCeiling { get; set; }
        // Background textures
        public Texture2D Layer0_0Texture { get; set; }
        public Texture2D Layer0_1Texture { get; set; }
        public Texture2D Layer0_2Texture { get; set; }
        public Texture2D Layer1_0Texture { get; set; }
        public Texture2D Layer1_1Texture { get; set; }
        public Texture2D Layer1_2Texture { get; set; }
        public Texture2D Layer2_0Texture { get; set; }
        public Texture2D Layer2_1Texture { get; set; }
        public Texture2D Layer2_2Texture { get; set; }
        // Multiple state items
        public Texture2D Lever { get; set; }
        public Texture2D YellowDoor { get; set; }
        public Texture2D RedDoor { get; set; }
        public Texture2D GreenDoor { get; set; }
        public Texture2D BlueDoor { get; set; }
        // Animated items
        public Texture2D Torch { get; set; }
        public Texture2D WaterSurface { get; set; }
        public Texture2D UpsideDownSpell { get; set; }
        public Texture2D Candle { get; set; }
        // Brackground tiles
        public Texture2D Water { get; set; }
        public Texture2D Brick{ get; set; }
        // Enemies textures
        private const int numMonsters = 3;
        private const int numGhosts = 1;
        private const int numFlying = 1;
        public Texture2D[] MonsterDieTexture = new Texture2D[numMonsters];
        public Texture2D[] MonsterIdleTexture = new Texture2D[numMonsters];
        public Texture2D[] MonsterRunTexture = new Texture2D[numMonsters];
        public Texture2D[] GhostDieTexture = new Texture2D[numGhosts];
        public Texture2D[] GhostIdleTexture = new Texture2D[numGhosts];
        public Texture2D[] GhostRunTexture = new Texture2D[numGhosts];
        public Texture2D[] FlyingDieTexture = new Texture2D[numFlying];
        public Texture2D[] FlyingIdleTexture = new Texture2D[numFlying];
        public Texture2D[] FlyingRunTexture = new Texture2D[numFlying];
        // Player Textures
        public Texture2D PlayerAttackTexture { get; set; }
        public Texture2D PlayerAttack_swordAnimation { get; set; }
        public Texture2D PlayerCelebrateTexture { get; set; }
        public Texture2D PlayerDieTexture { get; set; }
        public Texture2D PlayerIdleTexture { get; set; }
        public Texture2D PlayerJumpTexture { get; set; }
        public Texture2D PlayerRunTexture { get; set; }
        public Texture2D PlayerClimbingTexture { get; set; }
        //  Messages texture overlays
        public Texture2D You_DiedTexture { get; set; }
        public Texture2D You_LoseTexture { get; set; }
        public Texture2D You_WinTexture { get; set; }
        #endregion
        public Viewport ViewPort { get; set; }

        /// <summary>
        /// If true, the manager prints out a list of all the screens
        /// each time it is updated. This can be useful for making sure
        /// everything is being added and removed at the right times.
        /// </summary>
        public bool TraceEnabled { get { return traceEnabled; } }


        #endregion

        #region Initialization


        /// <summary>
        /// Constructs a new screen manager component.
        /// </summary>
        public ScreenManager(Game game, CastleXGame gamepage)
            : base(game)
        {
            FrontGamePage = gamepage;
            Input = new InputState(this);
        }


        /// <summary>
        /// Initializes the screen manager component.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            isInitialized = true;
        }


        /// <summary>
        /// Load your graphics content.
        /// </summary>
        protected override void LoadContent()
        {
            this.GetDevice();

            // Load content belonging to the screen manager.
            Content = Game.Content;
            GraphicContent = new ContentManager(Game.Services);
            GraphicContent = Game.Content;
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            HudFont = GraphicContent.Load<SpriteFont>(@"Fonts\Hud");
            Segoe12 = GraphicContent.Load<SpriteFont>(@"Fonts\Segoe12");
            HudBig = GraphicContent.Load<SpriteFont>(@"Fonts\HudBig");
            HudMedium = GraphicContent.Load<SpriteFont>(@"Fonts\HudMedium");
            HudSmall = GraphicContent.Load<SpriteFont>(@"Fonts\HudSmall");
            BlankTexture = Content.Load<Texture2D>(@"blank");
            CrossHairTexture = Content.Load<Texture2D>(@"crosshair");
            HUDTexture = Content.Load<Texture2D>(@"HUD");
            BannerTexture = Content.Load<Texture2D>(@"Banner");
            SpacerTexture = Content.Load<Texture2D>(@"spacer");
           // GradientTexture = Content.Load<Texture2D>(@"gradient");

            LoadSettings(Device);
            loadSkinSettings(false);
            loadGameContent();
            //LoadNumberOfLevels();

            // Tell each of the screens to load their content.
            foreach (GameScreen screen in screens)
            {
                screen.LoadContent();
            }
        }


        #region Load Content from the current skin
        public void loadGameContent()
        {
            #region Load Background Layers


            try
            {
                if (SkinSettings.hasBackgrounds)
                    Layer0_0Texture = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Backgrounds/Layer0_0");
                else
                   Layer0_0Texture = GraphicContent.Load<Texture2D>("Skins/0/Backgrounds/Layer0_0");
            }
            catch
            {
                Layer0_0Texture = GraphicContent.Load<Texture2D>("Skins/0/Backgrounds/Layer0_0");
            }
            try
            {
                if (SkinSettings.hasBackgrounds)
                    Layer0_1Texture = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Backgrounds/Layer0_1");
                else
                    Layer0_1Texture = GraphicContent.Load<Texture2D>("Skins/0/Backgrounds/Layer0_1");
            }
            catch
            {
                Layer0_1Texture = GraphicContent.Load<Texture2D>("Skins/0/Backgrounds/Layer0_1");
            }
            try
            {
                if (SkinSettings.hasBackgrounds)
                    Layer0_2Texture = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Backgrounds/Layer0_2");
                else
                    Layer0_2Texture = GraphicContent.Load<Texture2D>("Skins/0/Backgrounds/Layer0_2");
            }
            catch
            {
                Layer0_2Texture = GraphicContent.Load<Texture2D>("Skins/0/Backgrounds/Layer0_2");
            }


            try
            {
                if (SkinSettings.hasBackgrounds)
                    Layer1_0Texture = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Backgrounds/Layer1_0");
                else
                    Layer1_0Texture = GraphicContent.Load<Texture2D>("Skins/0/Backgrounds/Layer1_0");
            }
            catch
            {
                Layer1_0Texture = GraphicContent.Load<Texture2D>("Skins/0/Backgrounds/Layer1_0");
            }
            try
            {
                if (SkinSettings.hasBackgrounds)
                    Layer1_1Texture = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Backgrounds/Layer1_1");
                else
                    Layer1_1Texture = GraphicContent.Load<Texture2D>("Skins/0/Backgrounds/Layer1_1");
            }
            catch
            {
                Layer1_1Texture = GraphicContent.Load<Texture2D>("Skins/0/Backgrounds/Layer1_1");
            }
            try
            {
                if (SkinSettings.hasBackgrounds)
                    Layer1_2Texture = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Backgrounds/Layer1_2");
                else
                    Layer1_2Texture = GraphicContent.Load<Texture2D>("Skins/0/Backgrounds/Layer1_2");
            }
            catch
            {
                Layer1_2Texture = GraphicContent.Load<Texture2D>("Skins/0/Backgrounds/Layer1_2");
            }

            try
            {
                if (SkinSettings.hasBackgrounds)
                    Layer2_0Texture = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Backgrounds/Layer2_0");
                else
                    Layer2_0Texture = GraphicContent.Load<Texture2D>("Skins/0/Backgrounds/Layer2_0");
            }
            catch
            {
                Layer2_0Texture = GraphicContent.Load<Texture2D>("Skins/0/Backgrounds/Layer2_0");
            }
            try
            {
                if (SkinSettings.hasBackgrounds)
                    Layer2_1Texture = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Backgrounds/Layer2_1");
                else
                    Layer2_1Texture = GraphicContent.Load<Texture2D>("Skins/0/Backgrounds/Layer2_1");
            }
            catch
            {
                Layer2_1Texture = GraphicContent.Load<Texture2D>("Skins/0/Backgrounds/Layer2_1");
            }
            try
            {
                if (SkinSettings.hasBackgrounds)
                    Layer2_2Texture = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Backgrounds/Layer2_2");
                else
                    Layer2_2Texture = GraphicContent.Load<Texture2D>("Skins/0/Backgrounds/Layer2_2");
            }
            catch
            {
                Layer2_2Texture = GraphicContent.Load<Texture2D>("Skins/0/Backgrounds/Layer2_2");
            }

            #endregion

            #region Load Overlays
            try
            {
                if (SkinSettings.hasOverlays)
                {
                    You_DiedTexture = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Overlays/you_died");
                }
                else
                {
                    You_DiedTexture = GraphicContent.Load<Texture2D>("Skins/0/Overlays/you_died");
                }
            }
            catch
            {
                You_DiedTexture = GraphicContent.Load<Texture2D>("Skins/0/Overlays/you_died");
            }


            try
            {
                if (SkinSettings.hasOverlays)
                {
                    You_LoseTexture = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Overlays/you_lose");
                }
                else
                {
                    You_LoseTexture = GraphicContent.Load<Texture2D>("Skins/0/Overlays/you_lose");
                }
            }
            catch
            {
                You_LoseTexture = GraphicContent.Load<Texture2D>("Skins/0/Overlays/you_lose");
            }



            try
            {
                if (SkinSettings.hasOverlays)
                {
                    You_WinTexture = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Overlays/you_win");
                }
                else
                {
                    You_WinTexture = GraphicContent.Load<Texture2D>("Skins/0/Overlays/you_win");
                }
            }
            catch
            {
                You_WinTexture = GraphicContent.Load<Texture2D>("Skins/0/Overlays/you_win");
            }

            #endregion

            #region Load Sprites
            #region Load Player Sprites
            try
            {
                if (SkinSettings.hasSprites)
                {
                    PlayerAttackTexture = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Sprites/Player/Attack");
                    PlayerAttack_swordAnimation = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Sprites/Player/attack_sword");
                }
                else
                {
                    PlayerAttackTexture = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Sprites/Player/Attack");
                    PlayerAttack_swordAnimation = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Sprites/Player/Attack_sword");
                }
            }
            catch
            {
                PlayerAttackTexture = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Player/Attack");
                PlayerAttack_swordAnimation = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Player/Attack_sword");
            }

            try
            {
                if (SkinSettings.hasSprites)
                    PlayerCelebrateTexture = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Sprites/Player/Celebrate");
                else
                    PlayerCelebrateTexture = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Player/Celebrate");
            }
            catch
            {
                PlayerCelebrateTexture = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Player/Celebrate");
            }

            try
            {
                if (SkinSettings.hasSprites)
                    PlayerDieTexture = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Sprites/Player/Die");
                else
                    PlayerDieTexture = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Player/Die");
            }
            catch
            {
                PlayerDieTexture = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Player/Die");
            }
            try
            {
                if (SkinSettings.hasSprites)
                    PlayerIdleTexture = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Sprites/Player/Idle");
                else
                    PlayerIdleTexture = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Player/Idle");
            }
            catch
            {
                PlayerIdleTexture = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Player/Idle");
            }

            try
            {
                if (SkinSettings.hasSprites)
                    PlayerJumpTexture = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Sprites/Player/Jump");
                else
                    PlayerJumpTexture = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Player/Jump");
            }
            catch
            {
                PlayerJumpTexture = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Player/Jump");
            }

            try
            {
                if (SkinSettings.hasSprites)
                    PlayerRunTexture = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Sprites/Player/Run");
                else
                    PlayerRunTexture = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Player/Run");
            }
            catch
            {
                PlayerRunTexture = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Player/Run");
            }
            
            try
            {
                if (SkinSettings.hasSprites)
                    PlayerClimbingTexture = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Sprites/Player/Climbing");
                else
                    PlayerClimbingTexture = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Player/Climbing");
            }
            catch
            {
                PlayerClimbingTexture = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Player/Climbing");
            }

            #endregion

            #region Load Multiple state Sprites
            try
            {
                if (SkinSettings.hasSprites)
                {
                    Lever = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Sprites/MultipleState/Lever");
                    YellowDoor = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Sprites/MultipleState/YellowDoor");
                    RedDoor = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Sprites/MultipleState/RedDoor");
                    GreenDoor = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Sprites/MultipleState/GreenDoor");
                    BlueDoor = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Sprites/MultipleState/BlueDoor");
                }
                else
                {
                    Lever = GraphicContent.Load<Texture2D>("Skins/0/Sprites/MultipleState/Lever");
                    YellowDoor = GraphicContent.Load<Texture2D>("Skins/0/Sprites/MultipleState/YellowDoor");
                    RedDoor = GraphicContent.Load<Texture2D>("Skins/0/Sprites/MultipleState/RedDoor");
                    GreenDoor = GraphicContent.Load<Texture2D>("Skins/0/Sprites/MultipleState/GreenDoor");
                    BlueDoor = GraphicContent.Load<Texture2D>("Skins/0/Sprites/MultipleState/BlueDoor");

                }
            }
            catch
            {
                Lever = GraphicContent.Load<Texture2D>("Skins/0/Sprites/MultipleState/Lever");
                YellowDoor = GraphicContent.Load<Texture2D>("Skins/0/Sprites/MultipleState/YellowDoor");
                RedDoor = GraphicContent.Load<Texture2D>("Skins/0/Sprites/MultipleState/RedDoor");
                GreenDoor = GraphicContent.Load<Texture2D>("Skins/0/Sprites/MultipleState/GreenDoor");
                BlueDoor = GraphicContent.Load<Texture2D>("Skins/0/Sprites/MultipleState/BlueDoor");
            }




            YellowDoor = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Sprites/MultipleState/YellowDoor");
            RedDoor = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Sprites/MultipleState/RedDoor");
            GreenDoor = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Sprites/MultipleState/GreenDoor");
            BlueDoor = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Sprites/MultipleState/BlueDoor");

            #endregion

            #region Load Animated Sprites
            try
            {
                if (SkinSettings.hasSprites)
                    Candle = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Sprites/Animated/Candle");
                else
                    Candle = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Animated/Candle");
            }
            catch
            {
                Candle = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Animated/Candle");
            }

            try
            {
                if (SkinSettings.hasSprites)
                    Torch = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Sprites/Animated/Torch");
                else
                    Torch = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Animated/Torch");
            }
            catch
            {
                Torch = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Animated/Torch");
            }
            try
            {
                if (SkinSettings.hasSprites)
                {
                    WaterSurface = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Sprites/Animated/WaterSurface");
                    Water = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Sprites/Water");
                    UpsideDownSpell = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Sprites/UpsideDownSpell3");
                }
                else
                {
                    WaterSurface = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Animated/WaterSurface");
                    Water = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Water");
                    UpsideDownSpell = GraphicContent.Load<Texture2D>("Skins/0/Sprites/UpsideDownSpell3");
                }
            }
            catch
            {
                WaterSurface = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Animated/WaterSurface");
                Water = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Water");
                UpsideDownSpell = GraphicContent.Load<Texture2D>("Skins/0/Sprites/UpsideDownSpell3");
            }
            #endregion

            #region Load Enemies Sprites
            // Monsters
            for (int i = 0; i < numMonsters; i++)
            {
                try
                {
                    if (SkinSettings.hasSprites)
                    {
                        MonsterDieTexture[i] = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Sprites/Enemies/Monster" + (i+1).ToString() + "/Die");
                        MonsterIdleTexture[i] = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Sprites/Enemies/Monster" + (i+1).ToString() + "/Idle");
                        MonsterRunTexture[i] = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Sprites/Enemies/Monster" + (i+1).ToString() + "/Run");
                    }
                    else
                    {
                        MonsterDieTexture[i] = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Enemies/Monster" + (i+1).ToString() + "/Die");
                        MonsterIdleTexture[i] = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Enemies/Monster" + (i+1).ToString() + "/Idle");
                        MonsterRunTexture[i] = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Enemies/Monster" + (i+1).ToString() + "/Run");
                    }
                }
                catch
                {
                    MonsterDieTexture[i] = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Enemies/Monster" + (i+1).ToString() + "/Die");
                    MonsterIdleTexture[i] = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Enemies/Monster" + (i+1).ToString() + "/Idle");
                    MonsterRunTexture[i] = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Enemies/Monster" + (i+1).ToString() + "/Run");
                }
            }
            // Ghosts
            for (int i = 0; i < numGhosts; i++)
            {
                try
                {
                    if (SkinSettings.hasSprites)
                    {
                        GhostDieTexture[i] = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Sprites/Enemies/Ghost" + (i+1).ToString() + "/Die");
                        GhostIdleTexture[i] = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Sprites/Enemies/Ghost" + (i+1).ToString() + "/Idle");
                        GhostRunTexture[i] = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Sprites/Enemies/Ghost" + (i+1).ToString() + "/Run");
                    }
                    else
                    {
                        GhostDieTexture[i] = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Enemies/Ghost" + (i+1).ToString() + "/Die");
                        GhostIdleTexture[i] = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Enemies/Ghost" + (i+1).ToString() + "/Idle");
                        GhostRunTexture[i] = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Enemies/Ghost" + (i+1).ToString() + "/Run");
                    }
                }
                catch
                {
                    GhostDieTexture[i] = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Enemies/Ghost" + (i+1).ToString() + "/Die");
                    GhostIdleTexture[i] = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Enemies/Ghost" + (i+1).ToString() + "/Idle");
                    GhostRunTexture[i] = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Enemies/Ghost" + (i+1).ToString() + "/Run");
                }
            }

            // Flying enemies
            for (int i = 0; i < numFlying; i++)
            {
                try
                {
                    if (SkinSettings.hasSprites)
                    {
                        FlyingDieTexture[i] = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Sprites/Enemies/Flying" + (i+1).ToString() + "/Die");
                        FlyingIdleTexture[i] = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Sprites/Enemies/Flying" + (i+1).ToString() + "/Idle");
                        FlyingRunTexture[i] = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Sprites/Enemies/Flying" + (i+1).ToString() + "/Run");
                    }
                    else
                    {
                        FlyingDieTexture[i] = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Enemies/Flying" + (i+1).ToString() + "/Die");
                        FlyingIdleTexture[i] = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Enemies/Flying" + (i+1).ToString() + "/Idle");
                        FlyingRunTexture[i] = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Enemies/Flying" + (i+1).ToString() + "/Run");
                    }
                }
                catch
                {
                    FlyingDieTexture[i] = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Enemies/Flying" + (i+1).ToString() + "/Die");
                    FlyingIdleTexture[i] = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Enemies/Flying" + (i+1).ToString() + "/Idle");
                    FlyingRunTexture[i] = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Enemies/Flying" + (i+1).ToString() + "/Run");
                }
            }


            #endregion

            #region Load Extra Sprites
            try
            {
                if (SkinSettings.hasSprites)
                {
                    FallingTileTexture = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Sprites/FallingTile");
                    ArrowTexture = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Sprites/Arrow");
                    CoinTexture = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Sprites/Coin");
                    HealthPotionTexture = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Sprites/MultipleState/HealthPotion");
                    LifeTexture = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Sprites/Life");
                    YellowKeyTexture = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Sprites/Key_Yellow");
                    GreenKeyTexture = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Sprites/Key_Green");
                    BlueKeyTexture = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Sprites/Key_Blue");
                    RedKeyTexture = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Sprites/Key_Red");
                    HUDOpenMapTexture = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Sprites/HUDOpenMap");
                    HUDClosedMapTexture = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Sprites/HUDClosedMap");
                    HUDOxygenTexture = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Sprites/HUDOxygen");
                    HUDCandleTexture = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Sprites/HUDCandle");                    
                    MapTexture = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Sprites/Map");
                    Brick = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Sprites/Brick");
                    OxygenTexture = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Sprites/Oxygen");
                    MagicMirrorTexture = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Sprites/MagicMirror");
                }
                else
                {
                    ArrowTexture = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Arrow");
                    FallingTileTexture = GraphicContent.Load<Texture2D>("Skins/0/Sprites/FallingTile");
                    CoinTexture = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Coin");
                    HealthPotionTexture = GraphicContent.Load<Texture2D>("Skins/0/Sprites/MultipleState/HealthPotion");
                    LifeTexture = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Life");
                    YellowKeyTexture = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Key_Yellow");
                    GreenKeyTexture = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Key_Green");
                    BlueKeyTexture = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Key_Blue");
                    RedKeyTexture = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Key_Red");
                    HUDOpenMapTexture = GraphicContent.Load<Texture2D>("Skins/0/Sprites/HUDOpenMap");
                    HUDClosedMapTexture = GraphicContent.Load<Texture2D>("Skins/0/Sprites/HUDClosedMap");
                    HUDOxygenTexture = GraphicContent.Load<Texture2D>("Skins/0/Sprites/HUDOxygen");
                    HUDCandleTexture = GraphicContent.Load<Texture2D>("Skins/0/Sprites/HUDCandle");
                    MapTexture = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Map");
                    Brick = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Brick");
                    OxygenTexture = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Oxygen");
                    MagicMirrorTexture = GraphicContent.Load<Texture2D>("Skins/0/Sprites/MagicMirror");
                }
            }
            catch
            {
                ArrowTexture = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Arrow");
                FallingTileTexture = GraphicContent.Load<Texture2D>("Skins/0/Sprites/FallingTile");
                CoinTexture = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Coin");
                HealthPotionTexture = GraphicContent.Load<Texture2D>("Skins/0/Sprites/MultipleState/HealthPotion");
                LifeTexture = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Life");
                YellowKeyTexture = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Key_Yellow");
                GreenKeyTexture = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Key_Green");
                BlueKeyTexture = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Key_Blue");
                RedKeyTexture = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Key_Red");
                HUDOpenMapTexture = GraphicContent.Load<Texture2D>("Skins/0/Sprites/HUDOpenMap");
                HUDClosedMapTexture = GraphicContent.Load<Texture2D>("Skins/0/Sprites/HUDClosedMap");
                HUDOxygenTexture = GraphicContent.Load<Texture2D>("Skins/0/Sprites/HUDOxygen");
                HUDCandleTexture = GraphicContent.Load<Texture2D>("Skins/0/Sprites/HUDCandle");
                MapTexture = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Map");
                Brick = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Brick");
                OxygenTexture = GraphicContent.Load<Texture2D>("Skins/0/Sprites/Oxygen");
                MagicMirrorTexture = GraphicContent.Load<Texture2D>("Skins/0/Sprites/MagicMirror");
            }
            #endregion
            #endregion

            #region Load Tiles
            BlockATexture = new Texture2D[7];
            BlockBTexture = new Texture2D[2];
            // Terrain Tiles
            ClayTerrain = new Texture2D[5];
            ClaySurface = new Texture2D[3];
            ClayCeiling = new Texture2D[3];
            MudTerrain = new Texture2D[5];
            MudSurface = new Texture2D[3];
            MudCeiling = new Texture2D[3];        
            RockTerrain = new Texture2D[5];
            RockSurface = new Texture2D[3];
            RockCeiling = new Texture2D[3];

            try
            {
                if (SkinSettings.hasTiles)
                {
                    BlockATexture[0] = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Tiles/BlockA0");
                    BlockATexture[1] = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Tiles/BlockA1");
                    BlockATexture[2] = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Tiles/BlockA2");
                    BlockATexture[3] = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Tiles/BlockA3");
                    BlockATexture[4] = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Tiles/BlockA4");
                    BlockATexture[5] = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Tiles/BlockA5");
                    BlockATexture[6] = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Tiles/BlockA6");
                    BlockBTexture[0] = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Tiles/BlockB0");
                    BlockBTexture[1] = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Tiles/BlockB1");
                    // Terrain Tiles
                    ClayTerrain[0] = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Tiles/Terrain/Clay1");
                    ClayTerrain[1] = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Tiles/Terrain/Clay2");
                    ClayTerrain[2] = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Tiles/Terrain/Clay3");
                    ClayTerrain[3] = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Tiles/Terrain/Clay4");
                    ClayTerrain[4] = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Tiles/Terrain/Clay5");
                    ClaySurface[0] = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Tiles/Terrain/ClaySurface1");
                    ClaySurface[1] = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Tiles/Terrain/ClaySurface2");
                    ClaySurface[2] = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Tiles/Terrain/ClaySurface3");
                    ClayCeiling[0] = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Tiles/Terrain/ClayCeiling1");
                    ClayCeiling[1] = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Tiles/Terrain/ClayCeiling2");
                    ClayCeiling[2] = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Tiles/Terrain/ClayCeiling3");
                    MudTerrain[0] = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Tiles/Terrain/Mud1");
                    MudTerrain[1] = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Tiles/Terrain/Mud2");
                    MudTerrain[2] = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Tiles/Terrain/Mud3");
                    MudTerrain[3] = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Tiles/Terrain/Mud4");
                    MudTerrain[4] = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Tiles/Terrain/Mud5");
                    MudSurface[0] = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Tiles/Terrain/MudSurface1");
                    MudSurface[1] = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Tiles/Terrain/MudSurface2");
                    MudSurface[2] = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Tiles/Terrain/MudSurface3");
                    MudCeiling[0] = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Tiles/Terrain/MudCeiling1");
                    MudCeiling[1] = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Tiles/Terrain/MudCeiling2");
                    MudCeiling[2] = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Tiles/Terrain/MudCeiling3");
                    RockTerrain[0] = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Tiles/Terrain/Rock1");
                    RockTerrain[1] = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Tiles/Terrain/Rock2");
                    RockTerrain[2] = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Tiles/Terrain/Rock3");
                    RockTerrain[3] = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Tiles/Terrain/Rock4");
                    RockTerrain[4] = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Tiles/Terrain/Rock5");
                    RockSurface[0] = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Tiles/Terrain/RockSurface1");
                    RockSurface[1] = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Tiles/Terrain/RockSurface2");
                    RockSurface[2] = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Tiles/Terrain/RockSurface3");
                    RockCeiling[0] = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Tiles/Terrain/RockCeiling1");
                    RockCeiling[1] = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Tiles/Terrain/RockCeiling2");
                    RockCeiling[2] = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Tiles/Terrain/RockCeiling3");
                    // Other tiles
                    ExitTexture = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Tiles/Exit");
                    PlatformTexture = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Tiles/Platform");
                    SpringTexture = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Tiles/Spring");
                    StoppedSpringTexture = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Tiles/StoppedSpring");
                    MoveablePlatformTexture = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Tiles/MovingTile");
                }
                else
                {
                    BlockATexture[0] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/BlockA0");
                    BlockATexture[1] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/BlockA1");
                    BlockATexture[2] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/BlockA2");
                    BlockATexture[3] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/BlockA3");
                    BlockATexture[4] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/BlockA4");
                    BlockATexture[5] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/BlockA5");
                    BlockATexture[6] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/BlockA6");
                    BlockBTexture[0] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/BlockB0");
                    BlockBTexture[1] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/BlockB1");
                    // Terrain Tiles
                    ClayTerrain[0] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/Clay1");
                    ClayTerrain[1] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/Clay2");
                    ClayTerrain[2] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/Clay3");
                    ClayTerrain[3] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/Clay4");
                    ClayTerrain[4] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/Clay5");
                    ClaySurface[0] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/ClaySurface1");
                    ClaySurface[1] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/ClaySurface2");
                    ClaySurface[2] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/ClaySurface3");
                    ClayCeiling[0] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/ClayCeiling1");
                    ClayCeiling[1] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/ClayCeiling2");
                    ClayCeiling[2] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/ClayCeiling3");
                    MudTerrain[0] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/Mud1");
                    MudTerrain[1] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/Mud2");
                    MudTerrain[2] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/Mud3");
                    MudTerrain[3] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/Mud4");
                    MudTerrain[4] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/Mud5");
                    MudSurface[0] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/MudSurface1");
                    MudSurface[1] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/MudSurface2");
                    MudSurface[2] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/MudSurface3");
                    MudCeiling[0] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/MudCeiling1");
                    MudCeiling[1] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/MudCeiling2");
                    MudCeiling[2] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/MudCeiling3");
                    RockTerrain[0] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/Rock1");
                    RockTerrain[1] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/Rock2");
                    RockTerrain[2] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/Rock3");
                    RockTerrain[3] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/Rock4");
                    RockTerrain[4] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/Rock5");
                    RockSurface[0] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/RockSurface1");
                    RockSurface[1] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/RockSurface2");
                    RockSurface[2] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/RockSurface3");
                    RockCeiling[0] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/RockCeiling1");
                    RockCeiling[1] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/RockCeiling2");
                    RockCeiling[2] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/RockCeiling3");
                    // Other tiles
                    ExitTexture = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Exit");
                    PlatformTexture = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Platform");
                    SpringTexture = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Spring");
                    StoppedSpringTexture = GraphicContent.Load<Texture2D>("Skins/0/Tiles/StoppedSpring");
                    MoveablePlatformTexture = GraphicContent.Load<Texture2D>("Skins/0/Tiles/MovingTile");
                }
            }
            catch
            {
                BlockATexture[0] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/BlockA0");
                BlockATexture[1] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/BlockA1");
                BlockATexture[2] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/BlockA2");
                BlockATexture[3] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/BlockA3");
                BlockATexture[4] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/BlockA4");
                BlockATexture[5] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/BlockA5");
                BlockATexture[6] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/BlockA6");
                BlockBTexture[0] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/BlockB0");
                BlockBTexture[1] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/BlockB1");
                // Terrain Tiles
                ClayTerrain[0] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/Clay1");
                ClayTerrain[1] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/Clay2");
                ClayTerrain[2] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/Clay3");
                ClayTerrain[3] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/Clay4");
                ClayTerrain[4] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/Clay5");
                ClaySurface[0] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/ClaySurface1");
                ClaySurface[1] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/ClaySurface2");
                ClaySurface[2] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/ClaySurface3");
                ClayCeiling[0] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/ClayCeiling1");
                ClayCeiling[1] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/ClayCeiling2");
                ClayCeiling[2] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/ClayCeiling3");
                MudTerrain[0] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/Mud1");
                MudTerrain[1] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/Mud2");
                MudTerrain[2] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/Mud3");
                MudTerrain[3] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/Mud4");
                MudTerrain[4] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/Mud5");
                MudSurface[0] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/MudSurface1");
                MudSurface[1] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/MudSurface2");
                MudSurface[2] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/MudSurface3");
                MudCeiling[0] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/MudCeiling1");
                MudCeiling[1] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/MudCeiling2");
                MudCeiling[2] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/MudCeiling3");
                RockTerrain[0] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/Rock1");
                RockTerrain[1] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/Rock2");
                RockTerrain[2] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/Rock3");
                RockTerrain[3] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/Rock4");
                RockTerrain[4] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/Rock5");
                RockSurface[0] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/RockSurface1");
                RockSurface[1] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/RockSurface2");
                RockSurface[2] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/RockSurface3");
                RockCeiling[0] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/RockCeiling1");
                RockCeiling[1] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/RockCeiling2");
                RockCeiling[2] = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Terrain/RockCeiling3");
                // Other tiles
                ExitTexture = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Exit");
                PlatformTexture = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Platform");
                SpringTexture = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Spring");
                StoppedSpringTexture = GraphicContent.Load<Texture2D>("Skins/0/Tiles/StoppedSpring");
                MoveablePlatformTexture = GraphicContent.Load<Texture2D>("Skins/0/Tiles/MovingTile");
            }

            try
            {
                if (SkinSettings.hasTiles)
                {
                    TrunkTexture = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Tiles/Checkpoint1");
                    Checkpoint2Texture = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Tiles/Checkpoint2");
                    LadderTexture = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Tiles/Ladder");
                    RopeTexture = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Tiles/Rope");
                    WoodBlockTexture = GraphicContent.Load<Texture2D>("Skins/" + Settings.Skin + "/Tiles/WoodBlock");
                }
                else
                {
                    TrunkTexture = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Checkpoint1");
                    Checkpoint2Texture = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Checkpoint2");
                    LadderTexture = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Ladder");
                    RopeTexture = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Rope");
                    WoodBlockTexture = GraphicContent.Load<Texture2D>("Skins/0/Tiles/WoodBlock");
                }
            }
            catch
            {
                TrunkTexture = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Checkpoint1");
                Checkpoint2Texture = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Checkpoint2");
                LadderTexture = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Ladder");
                RopeTexture = GraphicContent.Load<Texture2D>("Skins/0/Tiles/Rope");
                WoodBlockTexture = GraphicContent.Load<Texture2D>("Skins/0/Tiles/WoodBlock");
            }
            #endregion

            #region Load Sounds
            try
            {
                if (SkinSettings.hasSounds)
                {
                    ExitReachedSound = GraphicContent.Load<SoundEffect>("Skins/" + Settings.Skin + "/Sounds/ExitReached");
                    CoinCollectedSound = GraphicContent.Load<SoundEffect>("Skins/" + Settings.Skin + "/Sounds/CoinCollected");
                    SpringSound = GraphicContent.Load<SoundEffect>("Skins/" + Settings.Skin + "/Sounds/Bounce");
                    OptionsSound = GraphicContent.Load<SoundEffect>("Skins/" + Settings.Skin + "/Sounds/Options");
                    PlayerFallSound = GraphicContent.Load<SoundEffect>("Skins/" + Settings.Skin + "/Sounds/PlayerFall");
                    MonsterKilledSound = GraphicContent.Load<SoundEffect>("Skins/" + Settings.Skin + "/Sounds/MonsterKilled");
                    FlyingEnemyKilledSound = GraphicContent.Load<SoundEffect>("Skins/" + Settings.Skin + "/Sounds/FlyingEnemyKilled");
                    GhostKilledSound = GraphicContent.Load<SoundEffect>("Skins/" + Settings.Skin + "/Sounds/GhostKilled");
                    ArrowHitSound = GraphicContent.Load<SoundEffect>("Skins/" + Settings.Skin + "/Sounds/ArrowHit");

                    // Player Sound effects
                    PlayerHitSound = GraphicContent.Load<SoundEffect>("Skins/" + Settings.Skin + "/Sounds/PlayerHit");
                    PlayerJumpSound = GraphicContent.Load<SoundEffect>("Skins/" + Settings.Skin + "/Sounds/PlayerJump");
                    PlayerKilledSound = GraphicContent.Load<SoundEffect>("Skins/" + Settings.Skin + "/Sounds/PlayerKilled");
                    PlayerPowerUpSound = GraphicContent.Load<SoundEffect>("Skins/" + Settings.Skin + "/Sounds/PowerUp");
                    PlayerDiveSound = GraphicContent.Load<SoundEffect>("Skins/" + Settings.Skin + "/Sounds/Dive");
                    PlayerSwordSound = GraphicContent.Load<SoundEffect>("Skins/" + Settings.Skin + "/Sounds/Sword");
                    PlayerSwordUnderwaterSound = GraphicContent.Load<SoundEffect>("Skins/" + Settings.Skin + "/Sounds/SwordUnderwater");
                    PlayerArrowSound = GraphicContent.Load<SoundEffect>("Skins/" + Settings.Skin + "/Sounds/Arrow");
                    PlayerArrowUnderwaterSound = GraphicContent.Load<SoundEffect>("Skins/" + Settings.Skin + "/Sounds/ArrowUnderwater");
                }
                else
                {
                    ExitReachedSound = GraphicContent.Load<SoundEffect>("Skins/0/Sounds/ExitReached");
                    CoinCollectedSound = GraphicContent.Load<SoundEffect>("Skins/0/Sounds/CoinCollected");
                    SpringSound = GraphicContent.Load<SoundEffect>("Skins/0/Sounds/Bounce");
                    OptionsSound = GraphicContent.Load<SoundEffect>("Skins/0/Sounds/Options");
                    MonsterKilledSound = GraphicContent.Load<SoundEffect>("Skins/0/Sounds/MonsterKilled");
                    FlyingEnemyKilledSound = GraphicContent.Load<SoundEffect>("Skins/0/Sounds/FlyingEnemyKilled");
                    GhostKilledSound = GraphicContent.Load<SoundEffect>("Skins/0/Sounds/GhostKilled");
                    ArrowHitSound = GraphicContent.Load<SoundEffect>("Skins/0/Sounds/ArrowHit");
                    // Player sound efects
                    PlayerFallSound = GraphicContent.Load<SoundEffect>("Skins/0/Sounds/PlayerFall");
                    PlayerHitSound = GraphicContent.Load<SoundEffect>("Skins/0/Sounds/PlayerHit");
                    PlayerJumpSound = GraphicContent.Load<SoundEffect>("Skins/0/Sounds/PlayerJump");
                    PlayerKilledSound = GraphicContent.Load<SoundEffect>("Skins/0/Sounds/PlayerKilled");
                    PlayerPowerUpSound = GraphicContent.Load<SoundEffect>("Skins/0/Sounds/PowerUp");
                    PlayerDiveSound = GraphicContent.Load<SoundEffect>("Skins/0/Sounds/Dive");
                    PlayerSwordSound = GraphicContent.Load<SoundEffect>("Skins/0/Sounds/Sword");
                    PlayerSwordUnderwaterSound = GraphicContent.Load<SoundEffect>("Skins/0/Sounds/SwordUnderwater");
                    PlayerArrowSound = GraphicContent.Load<SoundEffect>("Skins/0/Sounds/Arrow");
                    PlayerArrowUnderwaterSound = GraphicContent.Load<SoundEffect>("Skins/0/Sounds/ArrowUnderwater");
                }
            }
            catch
            {
                ExitReachedSound = GraphicContent.Load<SoundEffect>("Skins/0/Sounds/ExitReached");
                CoinCollectedSound = GraphicContent.Load<SoundEffect>("Skins/0/Sounds/CoinCollected");
                SpringSound = GraphicContent.Load<SoundEffect>("Skins/0/Sounds/Bounce");
                OptionsSound = GraphicContent.Load<SoundEffect>("Skins/0/Sounds/Options");
                MonsterKilledSound = GraphicContent.Load<SoundEffect>("Skins/0/Sounds/MonsterKilled");
                FlyingEnemyKilledSound = GraphicContent.Load<SoundEffect>("Skins/0/Sounds/FlyingEnemyKilled");
                GhostKilledSound = GraphicContent.Load<SoundEffect>("Skins/0/Sounds/GhostKilled");
                ArrowHitSound = GraphicContent.Load<SoundEffect>("Skins/0/Sounds/ArrowHit");
                // Player sound effects                 
                PlayerFallSound = GraphicContent.Load<SoundEffect>("Skins/0/Sounds/PlayerFall");
                PlayerHitSound = GraphicContent.Load<SoundEffect>("Skins/0/Sounds/PlayerHit");
                PlayerJumpSound = GraphicContent.Load<SoundEffect>("Skins/0/Sounds/PlayerJump");
                PlayerKilledSound = GraphicContent.Load<SoundEffect>("Skins/0/Sounds/PlayerKilled");
                PlayerPowerUpSound = GraphicContent.Load<SoundEffect>("Skins/0/Sounds/PowerUp");
                PlayerDiveSound = GraphicContent.Load<SoundEffect>("Skins/0/Sounds/Dive");
                PlayerSwordSound = GraphicContent.Load<SoundEffect>("Skins/0/Sounds/Sword");
                PlayerSwordUnderwaterSound = GraphicContent.Load<SoundEffect>("Skins/0/Sounds/SwordUnderwater");
                PlayerArrowSound = GraphicContent.Load<SoundEffect>("Skins/0/Sounds/Arrow");
                PlayerArrowUnderwaterSound = GraphicContent.Load<SoundEffect>("Skins/0/Sounds/ArrowUnderwater");
        
            }
            #endregion

            #region Load Music
            try
            {
                if (SkinSettings.hasMusic)
                {
                    BackgroundMusic = GraphicContent.Load<Song>("Skins/" + Settings.Skin + "/Sounds/Castle");
                }
                else
                {
                    BackgroundMusic = GraphicContent.Load<Song>("Skins/0/Sounds/Castle");
                }
            }
            catch
            {
                BackgroundMusic = GraphicContent.Load<Song>("Skins/0/Sounds/Castle");
            }
            #endregion

            DefaultBGTexture = GraphicContent.Load<Texture2D>("Skins/0/Backgrounds/Black");
        }
        #endregion

        #region Load Skin Settings

        public void loadSkinSettings(bool loadonlyheader)
        {
            SkinSettings = new SkinSettings();
            String line;
            List<String> myLineVariable = new List<String>();
            string myNewLineVariable = "";
            StreamReader reader = null;
            try
            {
                reader = new StreamReader(Path.Combine(StorageContainer.TitleLocation, "GameContent\\Skins\\" + Settings.Skin + "\\SkinMetadata.txt"));
            }
            catch
            {
                resetSkinNumber();
                reader = new StreamReader(Path.Combine(StorageContainer.TitleLocation, "GameContent\\Skins\\" + Settings.Skin + "\\SkinMetadata.txt"));
            }

            while ((line = reader.ReadLine()) != null)
            {
                #region Retrieve skins title
                if (line.StartsWith("@tit."))
                {
                    SkinSettings.skinTitle = line.Remove(0, 5).ToString();
                }
                #endregion
                if (!loadonlyheader)
                {
                    #region Gather True False settings for what all is stored
                    if (line.StartsWith("@backgrounds.1"))
                        SkinSettings.hasBackgrounds = true;
                    else
                        if (line.StartsWith("@backgrounds.0"))
                        {
                            SkinSettings.hasBackgrounds = false;
                        }
                    if (line.StartsWith("@overlays.1"))
                        SkinSettings.hasOverlays = true;
                    else
                        if (line.StartsWith("@overlays.0"))
                        {
                            SkinSettings.hasOverlays = false;
                        }
                    if (line.StartsWith("@sprites.1"))
                        SkinSettings.hasSprites = true;
                    else
                        if (line.StartsWith("@sprites.0"))
                        {
                            SkinSettings.hasSprites = false;
                        }
                    if (line.StartsWith("@tiles.1"))
                        SkinSettings.hasTiles = true;
                    else
                        if (line.StartsWith("@tiles.0"))
                        {
                            SkinSettings.hasTiles = false;
                        }
                    if (line.StartsWith("@sounds.1"))
                        SkinSettings.hasSounds = true;
                    else
                        if (line.StartsWith("@sounds.0"))
                        {
                            SkinSettings.hasSounds = false;
                        }
                    if (line.StartsWith("@music.1"))
                        SkinSettings.hasMusic = true;
                    else
                        if (line.StartsWith("@music.0"))
                        {
                            SkinSettings.hasMusic = false;
                        }
                    #endregion

                    #region Gather Number settings for each animatons frame width
                    #region Players animation frame widths
                    if (line.StartsWith("@P_Celeb."))
                    {
                        SkinSettings.FrameWidth_Player_Celebrate = Int32.Parse(line.Remove(0, 9).ToString());
                    }
                    if (line.StartsWith("@P_Die."))
                    {
                        SkinSettings.FrameWidth_Player_Die = Int16.Parse(line.Remove(0, 7).ToString());
                    }
                    if (line.StartsWith("@P_Idle."))
                    {
                        SkinSettings.FrameWidth_Player_Idle = Int16.Parse(line.Remove(0, 8).ToString());
                    }
                    if (line.StartsWith("@P_Jump."))
                    {
                        SkinSettings.FrameWidth_Player_Jump = Int16.Parse(line.Remove(0, 8).ToString());
                    }
                    if (line.StartsWith("@P_Run."))
                    {
                        SkinSettings.FrameWidth_Player_Run = Int16.Parse(line.Remove(0, 7).ToString());
                    }
                    #endregion
                    #region enemies animation frame widths
                    #region Monsters
                    for (int i = 0; i < numMonsters; i++)
                    {
                        if (line.StartsWith("@M" + (i + 1).ToString() + "_Die."))
                        {
                            SkinSettings.FrameWidth_Monster_Die[i] = Int16.Parse(line.Remove(0, 8).ToString());
                        }
                        if (line.StartsWith("@M" + (i + 1).ToString() + "_Idle."))
                        {
                            SkinSettings.FrameWidth_Monster_Idle[i] = Int16.Parse(line.Remove(0, 9).ToString());
                        }
                        if (line.StartsWith("@M" + (i + 1).ToString() + "_Run."))
                        {
                            SkinSettings.FrameWidth_Monster_Run[i] = Int16.Parse(line.Remove(0, 8).ToString());
                        }
                    }
                    #endregion                    

                    #region Ghosts
                    for (int i = 0; i < numGhosts; i++)
                    {
                        if (line.StartsWith("@G" + (i + 1).ToString() + "_Die."))
                        {
                            SkinSettings.FrameWidth_Ghost_Die[i] = Int16.Parse(line.Remove(0, 8).ToString());
                        }
                        if (line.StartsWith("@G" + (i + 1).ToString() + "_Idle."))
                        {
                            SkinSettings.FrameWidth_Ghost_Idle[i] = Int16.Parse(line.Remove(0, 9).ToString());
                        }
                        if (line.StartsWith("@G" + (i + 1).ToString() + "_Run."))
                        {
                            SkinSettings.FrameWidth_Ghost_Run[i] = Int16.Parse(line.Remove(0, 8).ToString());
                        }
                    }
                    #endregion    

                    #region Flying enemies
                    for (int i = 0; i < numFlying; i++)
                    {
                        if (line.StartsWith("@F" + (i + 1).ToString() + "_Die."))
                        {
                            SkinSettings.FrameWidth_Flying_Die[i] = Int16.Parse(line.Remove(0, 8).ToString());
                        }
                        if (line.StartsWith("@F" + (i + 1).ToString() + "_Idle."))
                        {
                            SkinSettings.FrameWidth_Flying_Idle[i] = Int16.Parse(line.Remove(0, 9).ToString());
                        }
                        if (line.StartsWith("@F" + (i + 1).ToString() + "_Run."))
                        {
                            SkinSettings.FrameWidth_Flying_Run[i] = Int16.Parse(line.Remove(0, 8).ToString());
                        }
                    }
                    #endregion    

                    #endregion
                    #endregion
                }
                myNewLineVariable += line + "\n";
            }
            if (reader != null)
            {
                reader.Close();
                reader.Dispose();
            }
        }

        private void resetSkinNumber()
        {
            Trace.Write("\nAttempting to resetSkinNumber\n");
            Settings.Skin = 0;
            SaveData();
            Trace.Write("\nReset skin number to zero and resaved data\n");
        }
        #endregion

        #region Reload games content if skin is changed
        public void reloadSkin()
        {
            GraphicContent.Unload();
            GraphicContent.RootDirectory = "GameContent";
            GradientTexture = Content.Load<Texture2D>(@"gradient");
            Segoe12 = GraphicContent.Load<SpriteFont>(@"Fonts\Segoe12");
            HudBig = GraphicContent.Load<SpriteFont>(@"Fonts\HudBig");
            HudMedium = GraphicContent.Load<SpriteFont>(@"Fonts\HudMedium");
            HudSmall = GraphicContent.Load<SpriteFont>(@"Fonts\HudSmall");
            BlankTexture = Content.Load<Texture2D>(@"blank");
            CrossHairTexture = Content.Load<Texture2D>(@"crosshair");
            HUDTexture = Content.Load<Texture2D>(@"HUD");
            BannerTexture = Content.Load<Texture2D>(@"Banner");
            loadGameContent();
        }

        #endregion

        /// <summary>
        /// Unload your graphics content.
        /// </summary>
        protected override void UnloadContent()
        {
            // Tell each of the screens to unload their content.
            foreach (GameScreen screen in screens)
                screen.UnloadContent();
        }



        #endregion

        #region Update
        /// <summary>
        /// Allows each screen to run logic.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            if (!isRunningSlow)
                if (FPS < 5)
                    isRunningSlow = true;

            // Read the keyboard and gamepad.
            Input.Update();

            // Make a copy of the master screen list, to avoid confusion if
            // the process of updating one screen adds or removes others.
            screensToUpdate.Clear();

            foreach (GameScreen screen in screens)
                screensToUpdate.Add(screen);

            bool otherScreenHasFocus = !Game.IsActive;
            bool coveredByOtherScreen = false;

            // Loop as long as there are screens waiting to be updated.
            while (screensToUpdate.Count > 0)
            {

                // Pop the topmost screen off the waiting list.
                GameScreen screen = screensToUpdate[screensToUpdate.Count - 1];

                screensToUpdate.RemoveAt(screensToUpdate.Count - 1);

                // Update the screen.
                screen.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
                if (screen.ScreenState == ScreenState.TransitionOn ||
                    screen.ScreenState == ScreenState.Active)
                {
                    // If this is the first active screen we came across,
                    // give it a chance to handle input.
                    if (!otherScreenHasFocus)
                    {
                        screen.HandleInput(Input);

                        otherScreenHasFocus = true;
                    }

                    // If this is an active non-popup, inform any subsequent
                    // screens that they are covered by it.
                    if (!screen.IsPopup)
                        coveredByOtherScreen = true;
                }
            }

            // Print debug trace?
            if (traceEnabled)
                TraceScreens();
        }

        /// <summary>
        /// Prints a list of all the screens, for debugging.
        /// </summary>
        public void TraceScreens()
        {
            List<string> screenNames = new List<string>();

            foreach (GameScreen screen in screens)
                screenNames.Add(screen.GetType().Name);

            Trace.WriteLine(string.Join(", ", screenNames.ToArray()));
        }

        List<GameScreen> previousScreens = new List<GameScreen>();

        /// <summary>
        /// Prints a list of all the screens, for debugging.
        /// </summary>
        public void RefreshScreens()
        {
            foreach (GameScreen screen in screens)
            {
                screen.ExitScreen();
                previousScreens.Add(screen);
            }
            screens.Clear();

        }

        public void RefreshScreens2()
        {

            foreach (GameScreen screen in previousScreens)
            {
                screens.Add(screen);
            }

        }

        #endregion

        #region Settings

        public void SaveData()
        {
            this.SaveSettings(this.Device);
        }

        Object stateobj;

        public void GetDevice()
        {
            // Reset the device
            Device = null;
            stateobj = (Object)"GetDevice for Player One";
            //Guide.BeginShowStorageDeviceSelector(PlayerIndex.One, this.GetDeviceAsync, stateobj);
            Guide.BeginShowStorageDeviceSelector(this.GetDeviceAsync, stateobj);
        }

        //StorageDevice device;
        void GetDeviceAsync(IAsyncResult result)
        {
            Device = Guide.EndShowStorageDeviceSelector(result);
        }

        public void LoadNumberOfLevels()
        {
            // Find the path of the next level.


            //Since there isnt a visible folder viewer on the xbox
            //like we can on the computer, we check to see if this game is ran on the computer or not.

#if !WINDOWS

            string levelPath;


            if (Settings.LoadLevelsFrom == 0)
            {
                levelPath = String.Format("Levels\\{0}.txt", ++LevelIndex);
                levelPath = Path.Combine(StorageContainer.TitleLocation, "GameContent\\" + levelPath);
                //If the user has the game set to load levels from the interior game,
                //the game will only count the levels from the interior games directory.

                //Open Book Directory and Get all files from inside it
                DirectoryInfo dir = new DirectoryInfo(Path.Combine(StorageContainer.TitleLocation, "GameContent\\Levels\\"));
                FileInfo[] dirfiles = dir.GetFiles();
                MaxLevels = dirfiles.Length;
                //Since the amount of files starts at 1, we subtract one from
                //the amount of files so we have a new copy of the amount starting from 0.
                MaxLevelsFromZero = dirfiles.Length - 1;
                Trace.Write("Max Levels: " + MaxLevels + "\n");
            }
            else if (Settings.LoadLevelsFrom == 1)
            {
                //If the user has the game set to load levels from the game folder,
                //we will check if each file exists in the folder,
                //and if it doesnt exist, we are to copy the level from the games
                //interior folder to the external folder.
                //we then count the file that is in the container (after it is copied if it didnt exist before).

                StorageContainer mycontainer = Device.OpenContainer("Castle_X");
                
                //Have a timer running that when a new level is counted it resets, but
                //but if it reaches 5 it will be done counting the levels.
                //This will make the game continue when it hasnt counted any new levels,
                //(meaning it reached the amount of levels), for a couple of seconds.
                int loadContainerTimer = 0;

                //Have a number that represents the current file number and increases it on each count.
                int numberOfLevel = 0;

                while (true)
                {
                    levelPath = String.Format("{00}.txt", ++numberOfLevel);

                    if (!File.Exists(Path.Combine(mycontainer.Path, "GameContent\\Levels\\" + levelPath)))
                    {
                        if (File.Exists(Path.Combine(StorageContainer.TitleLocation, "GameContent/Levels/" + levelPath)))
                        {
                            if (!Directory.Exists(Path.Combine(StorageContainer.TitleLocation, "GameContent/")))
                            {
                                Directory.CreateDirectory(Path.Combine(StorageContainer.TitleLocation, "GameContent/Levels/"));
                                Trace.Write("GameContent folder created\n\n");
                            }

                            if (!Directory.Exists(Path.Combine(StorageContainer.TitleLocation, "GameContent/Levels")))
                            {
                                Directory.CreateDirectory(Path.Combine(StorageContainer.TitleLocation, "GameContent/Levels/"));
                                Trace.Write("GameContent Levels folder created\n\n");
                            }

                            File.Copy(Path.Combine(StorageContainer.TitleLocation, "GameContent\\Levels\\" + levelPath), Path.Combine(mycontainer.Path, "GameContent\\Levels\\" + levelPath));
                            Trace.Write("\nCopied the internal storage version of level " + numberOfLevel + " to the containers folder\n");

                            //After we have copied the level from the game container
                            //to the users directory, we restart the timer and count the new level.
                            loadContainerTimer = 0;
                            Trace.Write(" copyTimer: " + loadContainerTimer + "  ");
                            numberOfLevel++;
                        }
                    }
                    else
                    {
                        //If the file already exists in the users directory,
                        //we restart the timer so we can load the next level 
                        loadContainerTimer = 0;
                        Trace.Write(" existTimer: " + loadContainerTimer + "  ");
                        numberOfLevel++;
                    }

                    loadContainerTimer += 1;
                    Trace.Write(" Timer: " + loadContainerTimer + "  ");
                    if (loadContainerTimer >= 10)
                    {
                        loadContainerTimer = 0;
                        DirectoryInfo dir = new DirectoryInfo(Path.Combine(mycontainer.Path, "GameContent\\Levels\\"));
                        FileInfo[] dirfiles = dir.GetFiles();
                        MaxLevels = dirfiles.Length;
                        //Since the amount of files starts at 1, we subtract one from
                        //the amount of files so we have a new copy of the amount starting from 0.
                        MaxLevelsFromZero = dirfiles.Length - 1;
                        Trace.Write("Max Levels: " + MaxLevels + "\n");
                        break;
                    }
                }
                mycontainer.Dispose();
            }
            else if (Settings.LoadLevelsFrom == 2)
            {
                //Have a timer running that when a new level is counted it resets, but
                //but if it reaches 5 it will be done counting the levels.
                //This will make the game continue when it hasnt counted any new levels,
                //(meaning it reached the amount of levels), for a couple of seconds.
                int loadContainerTimer = 0;

                //Have a number that represents the current file number and increases it on each count.
                int numberOfLevel = 0;

                while (true)
                {
                    levelPath = String.Format("{00}.txt", ++numberOfLevel);

                    if (!File.Exists(Settings.LevelFolderPath + "\\" + levelPath))
                    {
                        if (File.Exists(Path.Combine(StorageContainer.TitleLocation, "GameContent/Levels/" + levelPath)))
                        {
                            if (!Directory.Exists(Path.Combine(StorageContainer.TitleLocation, "GameContent/")))
                            {
                                Directory.CreateDirectory(Path.Combine(StorageContainer.TitleLocation, "GameContent/Levels/"));
                            }

                            if (!Directory.Exists(Path.Combine(StorageContainer.TitleLocation, "GameContent/Levels")))
                            {
                                Directory.CreateDirectory(Path.Combine(StorageContainer.TitleLocation, "GameContent/Levels/"));
                            }

                            File.Copy(Path.Combine(StorageContainer.TitleLocation, "GameContent/Levels/" + levelPath), Settings.LevelFolderPath + "\\" + levelPath);
                            Trace.Write("\nCopied the internal storage version of level " + numberOfLevel + " to the containers folder\n");

                            //After we have copied the level from the game container
                            //to the users directory, we restart the timer and count the new level.
                            loadContainerTimer = 0;
                            Trace.Write(" copyTimer: " + loadContainerTimer + "  ");
                            numberOfLevel++;
                        }
                    }
                    else
                    {
                        //If the file already exists in the users directory,
                        //we restart the timer so we can load the next level 
                        loadContainerTimer = 0;
                        Trace.Write(" existTimer: " + loadContainerTimer + "  ");
                        numberOfLevel++;
                    }

                    loadContainerTimer += 1;
                    Trace.Write(" Timer: " + loadContainerTimer + "  ");
                    if (loadContainerTimer >= 10)
                    {
                        loadContainerTimer = 0;
                        DirectoryInfo dir = new DirectoryInfo(Settings.LevelFolderPath + "\\" + levelPath);
                        try
                        {
                            FileInfo[] dirfiles = dir.GetFiles();
                            MaxLevels = dirfiles.Length;
                        //Since the amount of files starts at 1, we subtract one from
                        //the amount of files so we have a new copy of the amount starting from 0.
                        MaxLevelsFromZero = dirfiles.Length - 1;
                        Trace.Write("Max Levels: " + MaxLevels + "\n");
                        NumberOfLevels = MaxLevels;
                        break;
                        }
                        catch
                        {
                            Settings.LoadLevelsFrom = 0;

                        }
                    }
                }
            }


#else
            //if the game is ran on an xbox, we go ahead and load the files
            //straight from the games itself, since there would be no use having a separate folder
            //containing the games.
            DirectoryInfo dir = new DirectoryInfo(Path.Combine(StorageContainer.TitleLocation, "GameContent\\Levels\\"));
            FileInfo[] dirfiles = dir.GetFiles();
            MaxLevels = dirfiles.Length;
            //Since the amount of files starts at 1, we subtract one from
            //the amount of files so we have a new copy of the amount starting from 0.
            MaxLevelsFromZero = dirfiles.Length - 1;
            Trace.Write("Max Levels: " + MaxLevels + "\n");
#endif

        }

        public void LoadSettings(StorageDevice device)
        {
            // Open a storage container.
            //MyContainer.Dispose();
            StorageContainer mycontainer = device.OpenContainer("Castle_X");
            // Get the path of the save game.
            string filename = Path.Combine(mycontainer.Path, "settings.xml");

            if (File.Exists(filename))
            {
#if DEBUG
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    using (TextReader tr = new StreamReader(filename))
                    {
                        string xml = tr.ReadToEnd();
                        System.Diagnostics.Trace.WriteLine(xml);
                    }
                }
#endif
                try
                {
                    // Open the file
                    FileStream stream = File.Open(filename, FileMode.Open);

                    // Convert the object to XML data and put it in the stream.
                    XmlSerializer serializer = new XmlSerializer(typeof(PESettings));
                    Settings = serializer.Deserialize(stream) as PESettings;

                    // Close the file.
                    stream.Close();
                }
                catch (Exception exception)
                {
                    // In some cases, XmlSerializer throws exception when loading the data. We try to fix the problem by deleting the file.
                    // Hopefully this issue should be fixed now.
                    System.Diagnostics.Trace.WriteLine(exception.ToString());

                    //File.Delete(filename);
                }
            }

            // Dispose the container, to commit changes.
            mycontainer.Dispose();

            if (Settings == null)
            {
                Settings = new PESettings();
            }

        }


        public void SaveSettings(StorageDevice device)
        {
            // Open a storage container.

            IAsyncResult result = Guide.BeginShowStorageDeviceSelector(null, null);
            while (!result.IsCompleted) { }
            device = Guide.EndShowStorageDeviceSelector(result);

            StorageContainer mycontainer = device.OpenContainer("Castle_X");
            // Get the path of the save game.
            string filename = Path.Combine(mycontainer.Path, "settings.xml");

            // Open the file, creating it if necessary.
            FileStream stream = File.Open(filename, FileMode.Create);

            // Convert the object to XML data and put it in the stream.
            XmlSerializer serializer = new XmlSerializer(typeof(PESettings));
            serializer.Serialize(stream, Settings);

            // Close the file.
            stream.Close();

            // Dispose the container, to commit changes.
            mycontainer.Dispose();
        }


        #endregion

        #region Draw

        /// <summary>
        /// Tells each screen to draw itself.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            ViewPort = GraphicsDevice.Viewport;

            foreach (GameScreen screen in screens)
            {
                if (screen.ScreenState == ScreenState.Hidden)
                    continue;
                try
                {
                    SpriteBatch.Begin();
                }
                catch { }
                screen.Draw(gameTime, SpriteBatch);
                try
                {
                    SpriteBatch.End();
                }
                catch { }
            }
        }



        #endregion

        #region Public Methods

        /// <summary>
        /// Adds a new screen to the screen manager.
        /// </summary>
        public void AddScreen(GameScreen screen)
        {
            screen.ScreenManager = this;
            screen.IsExiting = false;

            // If we have a graphics device, tell the screen to load content.
            if (isInitialized)
            {
                screen.LoadContent();
            }

            screens.Add(screen);

        }

        /// <summary>
        /// Adds a new screen to the screen manager.
        /// </summary>
        public void SwitchScreen(GameScreen previousScreen, GameScreen nextScreen)
        {
            screens.Add(nextScreen);
            screens.Remove(previousScreen);

        }
        /// <summary>
        /// Removes a screen from the screen manager. You should normally
        /// use GameScreen.ExitScreen instead of calling this directly, so
        /// the screen can gradually transition off rather than just being
        /// instantly removed.
        /// </summary>
        public void RemoveScreen(GameScreen screen)
        {
            // If we have a graphics device, tell the screen to unload content.
            if (isInitialized)
            {
                screen.UnloadContent();
            }

            screens.Remove(screen);
            screensToUpdate.Remove(screen);

        }

        /// <summary>
        /// Expose an array holding all the screens. We return a copy rather
        /// than the real master list, because screens should only ever be added
        /// or removed using the AddScreen and RemoveScreen methods.
        /// </summary>
        public GameScreen[] GetScreens()
        {
            return screens.ToArray();
        }


        /// <summary>
        /// Helper draws a translucent black fullscreen sprite, used for fading
        /// screens in and out, and for darkening the background behind popups.
        /// </summary>
        public void FadeBackBufferToBlack(SpriteBatch spriteBatch, int alpha)
        {
            spriteBatch.Draw(BlankTexture, new Rectangle(0, 0, ViewPort.Width, ViewPort.Height), new Color(0, 0, 0, (byte)alpha));
        }

        public void DrawShadowedString(SpriteBatch spritebatch, SpriteFont font, string value, Vector2 position, Color color)
        {
            byte Alpha = color.A;
            spritebatch.DrawString(font, value, position + new Vector2(1.0f, 1.0f), new Color(0, 0, 0, Alpha));
            spritebatch.DrawString(font, value, position, color);
        }

        public void DrawShadowedString(SpriteBatch spritebatch, SpriteFont font, string value, Vector2 position, Color color, bool iscentered)
        {
            int Alpha = color.A;
            spritebatch.DrawString(font, value, position + new Vector2(1.0f, 1.0f), new Color(0, 0, 0, Alpha), 0, font.MeasureString(value) / 2, 1.0f, SpriteEffects.None, 0);
            spritebatch.DrawString(font, value, position, color, 0, font.MeasureString(value) / 2, 1.0f, SpriteEffects.None, 0);
        }

        public void DrawShadowedString(SpriteBatch spritebatch, SpriteFont font, string value, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects spriteeffects, float layerdepth)
        {
            int Alpha = color.A;
            spritebatch.DrawString(font, value, position + new Vector2(1.0f, 1.0f), new Color(0, 0, 0, Alpha), rotation, origin, scale, spriteeffects, layerdepth);
            spritebatch.DrawString(font, value, position, color, rotation, origin, scale, spriteeffects, layerdepth);
        }

        public string WordWrap(int width, String in_string, SpriteFont in_font)
        {
            int x;
            String current_line = "";
            String current_word = "";
            String new_string = "";

            for (x = 0; x < in_string.Length; x++)
            {
                if (in_string[x].CompareTo(' ') == 0)
                {
                    if (in_font.MeasureString(current_word).X + in_font.MeasureString(current_line + " ").X > width)
                    {
                        new_string = new_string + current_line + "\n";
                        current_line = current_word + " ";
                        current_word = "";
                    }
                    else
                    {
                        if (current_line.Length > 0)
                        {
                            current_line = current_line + " " + current_word;
                            current_word = "";
                        }
                        else
                        {
                            current_line = current_word;
                            current_word = "";
                        }
                    }
                }
                else
                {
                    current_word = current_word + in_string[x];
                }
            }
            new_string = new_string + current_line + " " + current_word;
            return new_string;
        }

        #endregion
    }
}
