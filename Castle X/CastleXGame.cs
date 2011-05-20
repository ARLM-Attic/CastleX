using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using System.Diagnostics;

namespace CastleX
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class CastleXGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        ScreenManager screenManager;
        SpriteBatch spriteBatch;
        ContentManager content;
        bool ingamemusicloaded = false;
        Texture2D splash;
        float splashtimer = 0;
        bool menuInit = false;
        bool splashisup = true;
        SoundEffect splashsound;
        BackgroundScreen backgroundScreen;
        //int frameRate = 0;
        int frameCounter = 0;
        TimeSpan elapsedTime = TimeSpan.Zero;


        // When the time remaining is less than the warning time, it blinks on the hud
        private static readonly TimeSpan WarningTime = TimeSpan.FromSeconds(30);

        public GraphicsDeviceManager Graphics
        {
            get { return graphics; }
            set { graphics = value; }
        }

        /// <summary>
        /// This is actually a reroute i did to allow calling the Graphics_ApplyChanges from other pages
        /// just simply have something try to attempt to grab this booleans value and it will reset the screen
        /// and return true.
        /// </summary>
        public bool ApplyChanges
        {
            get { Graphics_ApplyChanges(); return true; }
            set { Graphics_ApplyChanges(); ApplyChanges = false; }
        }
        private int TargetFrameRate = 30;        
        private int BackBufferWidth = 640;//1280;//240;
        private int BackBufferHeight = 400;//720;//320;
/*#if WINDOWS
#else
        private Buttons ContinueButton = Buttons.B;   
#endif*/
        public CastleXGame()
        {
            graphics = new Microsoft.Xna.Framework.GraphicsDeviceManager(this);

            Content.RootDirectory = "GameContent";
            screenManager = new ScreenManager(this, this);
            Components.Add(screenManager);
            graphics.PreferredBackBufferWidth = BackBufferWidth;
            graphics.PreferredBackBufferHeight = BackBufferHeight;

            // Activate the first screens.
            // Framerate differs between platforms.
        TargetElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / TargetFrameRate);

        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            if (content == null)
                content = new ContentManager(this.Services, "GameContent");

            splash = content.Load<Texture2D>("splash");
          //  splashsound = content.Load<SoundEffect>("wolfgrowl");
          //  splashsound.Play();
        }

        public void Graphics_ApplyChanges()
        {
            graphics.ApplyChanges();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime;

            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                screenManager.FPS = frameCounter;
                frameCounter = 0;
            }

            if (!menuInit)
            {
                /////////////////////GO TO MAIN MENU///////////////////
                
                backgroundScreen = new BackgroundScreen();

                splashtimer += 1;
                if (splashtimer > 10)
                {
                        screenManager.AddScreen(backgroundScreen);
                    screenManager.AddScreen(new MainMenuScreen(screenManager));
                    MediaPlayer.Volume = screenManager.Settings.MusicVolumeAmount;
                    if (screenManager.Settings.InGameMusic && !ingamemusicloaded)
                    {
                        MediaPlayer.IsRepeating = true;
                        MediaPlayer.Play(screenManager.BackgroundMusic);
                        
                        ingamemusicloaded = true;
                    }
                    menuInit = true;
                    splashisup = false;
                }

            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Draws the game from background to foreground.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            frameCounter++;
            graphics.GraphicsDevice.Clear(Color.Black);
            if (splashisup)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(splash, new Rectangle(0, 0, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height), Color.White);
                spriteBatch.End();
            }
            base.Draw(gameTime);
            if (screenManager.Settings.DebugMode)
            {
                screenManager.SpriteBatch.Begin();
                screenManager.SpriteBatch.DrawString(screenManager.Font, "FPS: " + screenManager.FPS.ToString(), new Vector2((screenManager.ViewPort.Width - screenManager.Font.MeasureString("FPS: " + screenManager.FPS.ToString()).X) - 10, (screenManager.ViewPort.Height - screenManager.Font.MeasureString("FPS: " + screenManager.FPS.ToString()).Y) - 10), Color.White);
                screenManager.SpriteBatch.End();
            }
        }
    }
}
