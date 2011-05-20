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
    class MessageBoxScreen : GameScreen
    {
        #region Fields

        string message;

        const string usageTextPrompt = "A Button = Ok\nB Button = Cancel";
        const string usageTextAlert = "A Button = Ok";

        bool IsPrompt = true, isintro = false, smallfont = false;

        GameplayScreen ingamescreen;

        #endregion

        #region Events

        public event EventHandler<EventArgs> Accepted;
        public event EventHandler<EventArgs> Cancelled;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor automatically includes the standard "A=ok, B=cancel"
        /// usage text prompt.
        /// </summary>
        /// 
        ///<param name="prompt">Setting this to true will ask for a yes or no input, setting this to false will just show a "ok" button. </param>
        public MessageBoxScreen(string message, bool prompt, bool smallFont)
        {

            smallfont = smallFont;
            this.message = message;
            IsPrompt = prompt;
            IsPopup = true;

            TransitionOnTime = TimeSpan.FromSeconds(0.2);
            TransitionOffTime = TimeSpan.FromSeconds(0.2);
        }
        /// <summary>
        /// Constructor automatically includes the standard "A=ok, B=cancel"
        /// usage text prompt.
        /// </summary>
        /// 
        ///<param name="prompt">Setting this to true will ask for a yes or no input, setting this to false will just show a "ok" button. </param>
        public MessageBoxScreen(string message, bool prompt, GameplayScreen gamescreen, bool smallFont)
        {

            this.message = message;
            smallfont = smallFont;
            IsPrompt = prompt;
            IsPopup = true;
            isintro = true;
            ingamescreen = gamescreen;
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
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Responds to user input, accepting or cancelling the message box.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (IsPrompt)
            {
                if (input.MenuSelect)
                {
                    // Raise the accepted event, then exit the message box.
                    if (Accepted != null)
                        Accepted(this, EventArgs.Empty);

                    ExitScreen();
                }
                else if (input.MenuCancel)
                {
                    // Raise the cancelled event, then exit the message box.
                    if (Cancelled != null)
                        Cancelled(this, EventArgs.Empty);

                    ExitScreen();
                }
            }
            else
            {
                if (input.MenuSelect)
                {
                    // Raise the cancelled event, then exit the message box.
                    if (Accepted != null)
                        Accepted(this, EventArgs.Empty);
                    if (isintro)
                        ingamescreen.IntroIsUp = false;
                    ExitScreen();
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
            SpriteFont font;
            if (smallfont)
            {

                font = ScreenManager.SmallFont;
            }
            else
            {
                font = ScreenManager.Font;
            }

            //Usage Text
            // Calculate the hieght of the text
            

            // Center the message text in the viewport.
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
            Vector2 textSize = font.MeasureString(message);
            Vector2 textPosition = new Vector2(10, 30);

            // The background includes a border somewhat larger than the text itself.
            const int hPad = 32;
            const int vPad = 16;

            Rectangle backgroundRectangle = new Rectangle((int)textPosition.X - hPad,
                                                          (int)textPosition.Y - vPad,
                                                          (int)textSize.X + hPad * 2,
                                                          (int)textSize.Y + vPad * 2);

            // Fade the popup alpha during transitions.
            Color color = new Color(255, 255, 255, TransitionAlpha);

            //spriteBatch.Begin();
            // Darken down any other screens that were drawn beneath the popup.
            ScreenManager.FadeBackBufferToBlack(spriteBatch, TransitionAlpha * 2 / 3);

            // Draw the message box text.
            spriteBatch.DrawString(font, 
                ScreenManager.WordWrap((ScreenManager.GraphicsDevice.Viewport.Width/6*5),message,font), 
                textPosition, color);

            // Draw Usage Text
            if (this.IsPrompt)
            {
                // Display both Confirmation Buttons
                spriteBatch.DrawString(font, usageTextPrompt, new Vector2(10, ScreenManager.GraphicsDevice.Viewport.Height/8*6), color);
            }
            else
            {
                // Display only the Alert button
                spriteBatch.DrawString(font, usageTextAlert, new Vector2(10, ScreenManager.GraphicsDevice.Viewport.Height / 8 * 7), color);
            }

            //spriteBatch.End();
        }


        #endregion

    }
}
