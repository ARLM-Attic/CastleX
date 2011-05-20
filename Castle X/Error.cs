using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;
using System.IO;
using Microsoft.Xna.Framework.GamerServices;
using System.Diagnostics;

namespace CastleX
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Error : Microsoft.Xna.Framework.Game
    {
        ContentManager content;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont courier10bold;
        SpriteFont courier10;
        SpriteFont courier8bold;
        SpriteFont courier8;
        Texture2D myrectangle;
        

        string myerrornote1;
        string myerrornote2,  gameName, gameName2;


        bool showdash = false, isinitialized = false;
        int dashtimer = 0, errorx = 0, errory = 0, logx = 0, logy = 0;

        bool showlog = false;

        

        /// <summary>
        /// brings up a BSOD to show an error.
        /// </summary>
        public Error(Exception error)
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 240;
            graphics.PreferredBackBufferHeight = 320;

            myerrornote2 = error.Message.ToString();
            myerrornote1 = error.StackTrace.ToString();
            Content.RootDirectory = "GameContent";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
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


            courier10bold = content.Load<SpriteFont>("Fonts/courier10bold");
            courier10 = content.Load<SpriteFont>("Fonts/courier10");
            courier8bold = content.Load<SpriteFont>("Fonts/courier8bold");
            courier8 = content.Load<SpriteFont>("Fonts/courier8");
            myrectangle = content.Load<Texture2D>("blank");

            // TODO: use this.Content to load your game content here

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            gameName = typeof(Error).Namespace.ToString();

            if (!isinitialized)
            {
                dashtimer = 0;
                isinitialized = true;
            }


            // Allows the game to exit
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
              //  this.Exit();
            if (gameName.Length > 4)
            {
                gameName2 = gameName;
                gameName = gameName.Substring(0, 5) + "..";
            }
            else
            {
                gameName2 = gameName;
                gameName = gameName.ToString();
            }
            


            if(Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Back) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Back))
                throw new Exception("This should make the game 'Crash'");
                    
            if(Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Enter) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Back))
                Exit();

            if (GamePad.GetState(PlayerIndex.One).DPad.Right == ButtonState.Pressed || Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Right))
            {
                if (!showlog)
                    errorx -= 2;
                else
                    logx -= 2;
            }

            if (GamePad.GetState(PlayerIndex.One).DPad.Down == ButtonState.Pressed || Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Down))
            {
                if (!showlog)
                    errory -= 2;
                else
                    logy -= 2;
            }

            if (GamePad.GetState(PlayerIndex.One).DPad.Left == ButtonState.Pressed || Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Left))
            {
                if (!showlog)
                    errorx = 0;
                else
                    logx = 0;
            }
            if (GamePad.GetState(PlayerIndex.One).DPad.Up == ButtonState.Pressed || Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Up))
            {
                if (!showlog)
                    errory = 0;
                else
                    logy += 2;
            }

            if (dashtimer > 50)
                dashtimer = 0;
            else
                dashtimer += 1;

            if (dashtimer < 26)
                showdash = true;
            else
                showdash = false;

            // TODO: Add your update logic here
            base.Update(gameTime);

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            if (!showlog)
            {
                spriteBatch.DrawString(courier8, "  StackTrace:", new Vector2(30 + errorx, 50 + errory), Color.White);
                spriteBatch.DrawString(courier8, myerrornote2, new Vector2(30 + errorx, 60 + errory), Color.White);
                spriteBatch.DrawString(courier8, "  Message:", new Vector2(30 + errorx, 80 + errory), Color.White);
                spriteBatch.DrawString(courier8bold, myerrornote1, new Vector2(30 + errorx, 90 + errory), Color.White);    

                spriteBatch.Draw(myrectangle, new Rectangle(0, 0, 240, 50), Color.Navy);
                spriteBatch.Draw(myrectangle, new Rectangle(220, 50, 25, 200), Color.Navy);
                spriteBatch.Draw(myrectangle, new Rectangle(0, 150, 240, 200), Color.Navy);
                spriteBatch.Draw(myrectangle, new Rectangle(0, 50, 25, 150), Color.Navy);
                spriteBatch.Draw(myrectangle, new Rectangle(0, 150, 240, 200), Color.Navy);

                spriteBatch.Draw(myrectangle, new Rectangle(58, 19, 130, 16), Color.Gray);
                    
                spriteBatch.DrawString(courier10bold, "Error in " + gameName, new Vector2(60, 20), Color.Navy);
#if WINDOWS
                spriteBatch.DrawString(courier8bold, "Write down first 3-5 Messages", new Vector2(15, 165), Color.White);
                spriteBatch.DrawString(courier8bold, "stop at \"in _filename_\"", new Vector2(15, 175), Color.White);
                spriteBatch.DrawString(courier8bold, "Go to thread and post the lines.", new Vector2(15, 185), Color.White);
#else
                spriteBatch.DrawString(courier8bold, "Write down first 3-5 Messages", new Vector2(15, 170), Color.White);
                spriteBatch.DrawString(courier8bold, "Go to thread and post the lines.", new Vector2(15, 180), Color.White);
#endif
                if (showdash)
                {
                    spriteBatch.DrawString(courier10, "Press Any Key To Continue _", new Vector2(13, 260), Color.White);
                }
                else
                {
                    spriteBatch.DrawString(courier10, "Press Any Key To Continue  ", new Vector2(13, 260), Color.White);
                }
            }

            spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
