#region File Description
//-----------------------------------------------------------------------------
// MessageBoxScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace CastleX
{
    /// <summary>
    /// A popup message box screen, used to display "are you sure?"
    /// confirmation messages.
    /// </summary>
    class NumberScreen : GameScreen
    {
        #region Fields
        Texture2D gradientTexture;


        string keypadnumber = "";
        string keypadnumberselected;
        int keypadchoicex, keypadchoicey, keypadnumber2;

        bool isDone = false;

        #endregion

        #region Initialization

        public bool IsDone
        {
            get { return isDone; }
            set { isDone = value; }
        }


        public string NumberTyped
        {
            get { return keypadnumber; }
            set { keypadnumber = value; }
        }


        /// <summary>
        /// Constructor automatically includes the standard "A=ok, B=cancel"
        /// usage text prompt.
        /// </summary>
        /// 
        ///<param name="prompt">Setting this to true will ask for a yes or no input, setting this to false will just show a "ok" button. </param>
        public NumberScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.2);
            TransitionOffTime = TimeSpan.FromSeconds(0.2);
        }

        /// <summary>
        /// Loads graphics content for this screen. This uses the shared ContentManager
        /// provided by the Game class, so the content will remain loaded forever.
        /// Whenever a subsequent MessageBoxScreen tries to load this same content,
        /// it will just get back another reference to the already loaded data.
        /// </summary>
        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;

            gradientTexture = content.Load<Texture2D>(@"gradient");
        }


        #endregion

        #region Handle Input

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {

            if (keypadchoicex == 0 && keypadchoicey == 0)
                keypadnumberselected = "1";
            if (keypadchoicex == 1 && keypadchoicey == 0)
                keypadnumberselected = "2";
            if (keypadchoicex == 2 && keypadchoicey == 0)
                keypadnumberselected = "3";
            if (keypadchoicex == 0 && keypadchoicey == 1)
                keypadnumberselected = "4";
            if (keypadchoicex == 1 && keypadchoicey == 1)
                keypadnumberselected = "5";
            if (keypadchoicex == 2 && keypadchoicey == 1)
                keypadnumberselected = "6";
            if (keypadchoicex == 0 && keypadchoicey == 2)
                keypadnumberselected = "7";
            if (keypadchoicex == 1 && keypadchoicey == 2)
                keypadnumberselected = "8";
            if (keypadchoicex == 2 && keypadchoicey == 2)
                keypadnumberselected = "9";
            if (keypadchoicex == 0 && keypadchoicey == 3)
                keypadnumberselected = "0";
            if (keypadchoicex == 1 && keypadchoicey == 3)
                keypadnumberselected = "";
            if (keypadchoicex == 2 && keypadchoicey == 3)
                keypadnumberselected = "Continue";

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        /// <summary>
        /// Responds to user input, accepting or cancelling the message box.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input.Left_Pressed)
            {
                if (keypadchoicex == 0)
                    keypadchoicex = 2;
                else
                    keypadchoicex -= 1;
            }
            if (input.Right_Pressed)
            {
                if (keypadchoicex == 2)
                    keypadchoicex = 0;
                else
                    keypadchoicex += 1;
            }
            if (input.Up_Pressed)
            {
                if (keypadchoicey == 0)
                    keypadchoicey = 3;
                else
                    keypadchoicey -= 1;
            }
            if (input.Down_Pressed)
            {
                if (keypadchoicey == 3)
                    keypadchoicey = 0;
                else
                    keypadchoicey += 1;
            }
            if (input.A_Pressed)
            {
                if (keypadnumberselected.Contains("Continue"))
                {
                    keypadnumber2 = int.Parse(keypadnumber);
                    if (keypadnumber2 > 500)
                    {
                        string tooHighMessage = "Number Too High!\n\nMaximum number is 500";
                        ScreenManager.AddScreen(new MessageBoxScreen(tooHighMessage, false, false));
                        keypadnumber = "";
                        keypadnumber2 = 0;
                    }
                    else
                    {
                        LoadingScreen.Load(ScreenManager, true,
                                                                 new LevelEditorScreen(ScreenManager, new Vector2(keypadnumber2, keypadnumber2)));
                    }
                }
                else
                {
                    keypadnumber += keypadnumberselected;
                }
            }
        }


        #endregion

        #region Draw


        /// <summary>
        /// Draws the message box.
        /// </summary>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            // Darken down any other screens that were drawn beneath the popup.
            ScreenManager.FadeBackBufferToBlack(spriteBatch, TransitionAlpha * 2 / 3);

            //Usage Text
            // Calculate the hieght of the text
            

            // Center the message text in the viewport.
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;

            // The background includes a border somewhat larger than the text itself.
            //const int hPad = 32;
            //const int vPad = 16;

            // Fade the popup alpha during transitions.
            Color color = new Color(255, 255, 255, TransitionAlpha);

            //spriteBatch.Begin();
                spriteBatch.Draw(gradientTexture, new Rectangle(ScreenManager.GraphicsDevice.Viewport.Width / 9 * 2, ScreenManager.GraphicsDevice.Viewport.Height / 13 * 3, ScreenManager.GraphicsDevice.Viewport.Width / 13 * 6, ScreenManager.GraphicsDevice.Viewport.Height / 13 * 7), Color.Gray);
                spriteBatch.DrawString(ScreenManager.Segoe12, keypadnumber + "x" + keypadnumber, new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 9 * 4, ScreenManager.GraphicsDevice.Viewport.Height / 13 * 4 - 10), Color.White, 0, ScreenManager.Segoe12.MeasureString(keypadnumber) / 2, 1.0f, SpriteEffects.None, 0.5f);

                if (keypadchoicex == 0 && keypadchoicey == 0)
                    spriteBatch.DrawString(ScreenManager.Segoe12, "1", new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 9 * 3, ScreenManager.GraphicsDevice.Viewport.Height / 13 * 5), Color.White);
                else
                    spriteBatch.DrawString(ScreenManager.Segoe12, "1", new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 9 * 3, ScreenManager.GraphicsDevice.Viewport.Height / 13 * 5), Color.Gray);

                if (keypadchoicex == 1 && keypadchoicey == 0)
                    spriteBatch.DrawString(ScreenManager.Segoe12, "2", new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 9 * 4, ScreenManager.GraphicsDevice.Viewport.Height / 13 * 5), Color.White);
                else
                    spriteBatch.DrawString(ScreenManager.Segoe12, "2", new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 9 * 4, ScreenManager.GraphicsDevice.Viewport.Height / 13 * 5), Color.Gray);

                if (keypadchoicex == 2 && keypadchoicey == 0)
                    spriteBatch.DrawString(ScreenManager.Segoe12, "3", new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 9 * 5, ScreenManager.GraphicsDevice.Viewport.Height / 13 * 5), Color.White);
                else
                    spriteBatch.DrawString(ScreenManager.Segoe12, "3", new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 9 * 5, ScreenManager.GraphicsDevice.Viewport.Height / 13 * 5), Color.Gray);

                if (keypadchoicex == 0 && keypadchoicey == 1)
                    spriteBatch.DrawString(ScreenManager.Segoe12, "4", new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 9 * 3, ScreenManager.GraphicsDevice.Viewport.Height / 13 * 6), Color.White);
                else
                    spriteBatch.DrawString(ScreenManager.Segoe12, "4", new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 9 * 3, ScreenManager.GraphicsDevice.Viewport.Height / 13 * 6), Color.Gray);

                if (keypadchoicex == 1 && keypadchoicey == 1)
                    spriteBatch.DrawString(ScreenManager.Segoe12, "5", new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 9 * 4, ScreenManager.GraphicsDevice.Viewport.Height / 13 * 6), Color.White);
                else
                    spriteBatch.DrawString(ScreenManager.Segoe12, "5", new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 9 * 4, ScreenManager.GraphicsDevice.Viewport.Height / 13 * 6), Color.Gray);

                if (keypadchoicex == 2 && keypadchoicey == 1)
                    spriteBatch.DrawString(ScreenManager.Segoe12, "6", new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 9 * 5, ScreenManager.GraphicsDevice.Viewport.Height / 13 * 6), Color.White);
                else
                    spriteBatch.DrawString(ScreenManager.Segoe12, "6", new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 9 * 5, ScreenManager.GraphicsDevice.Viewport.Height / 13 * 6), Color.Gray);

                if (keypadchoicex == 0 && keypadchoicey == 2)
                    spriteBatch.DrawString(ScreenManager.Segoe12, "7", new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 9 * 3, ScreenManager.GraphicsDevice.Viewport.Height / 13 * 7), Color.White);
                else
                    spriteBatch.DrawString(ScreenManager.Segoe12, "7", new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 9 * 3, ScreenManager.GraphicsDevice.Viewport.Height / 13 * 7), Color.Gray);

                if (keypadchoicex == 1 && keypadchoicey == 2)
                    spriteBatch.DrawString(ScreenManager.Segoe12, "8", new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 9 * 4, ScreenManager.GraphicsDevice.Viewport.Height / 13 * 7), Color.White);
                else
                    spriteBatch.DrawString(ScreenManager.Segoe12, "8", new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 9 * 4, ScreenManager.GraphicsDevice.Viewport.Height / 13 * 7), Color.Gray);

                if (keypadchoicex == 2 && keypadchoicey == 2)
                    spriteBatch.DrawString(ScreenManager.Segoe12, "9", new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 9 * 5, ScreenManager.GraphicsDevice.Viewport.Height / 13 * 7), Color.White);
                else
                    spriteBatch.DrawString(ScreenManager.Segoe12, "9", new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 9 * 5, ScreenManager.GraphicsDevice.Viewport.Height / 13 * 7), Color.Gray);

                if (keypadchoicex == 0 && keypadchoicey == 3)
                    spriteBatch.DrawString(ScreenManager.Segoe12, "0", new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 9 * 3, ScreenManager.GraphicsDevice.Viewport.Height / 13 * 8), Color.White);
                else
                    spriteBatch.DrawString(ScreenManager.Segoe12, "0", new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 9 * 3, ScreenManager.GraphicsDevice.Viewport.Height / 13 * 8), Color.Gray);

                if (keypadchoicex == 1 && keypadchoicey == 3)
                    spriteBatch.DrawString(ScreenManager.Segoe12, "", new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 9 * 4 - 10, ScreenManager.GraphicsDevice.Viewport.Height / 13 * 8), Color.White);
                else
                    spriteBatch.DrawString(ScreenManager.Segoe12, "", new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 9 * 4 - 10, ScreenManager.GraphicsDevice.Viewport.Height / 13 * 8), Color.Gray);

                if (keypadchoicex == 2 && keypadchoicey == 3)
                    spriteBatch.DrawString(ScreenManager.Segoe12, "GO", new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 9 * 5, ScreenManager.GraphicsDevice.Viewport.Height / 13 * 8), Color.White);
                else
                    spriteBatch.DrawString(ScreenManager.Segoe12, "GO", new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 9 * 5, ScreenManager.GraphicsDevice.Viewport.Height / 13 * 8), Color.Gray);
                
            //spriteBatch.End();

        }
        #endregion

    }
}
