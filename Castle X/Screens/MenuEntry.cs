#region File Description
//-----------------------------------------------------------------------------
// MenuEntry.cs
//
// XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
#endregion

namespace CastleX
{
    /// <summary>
    /// Helper class represents a single entry in a MenuScreen. By default this
    /// just draws the entry text string, but it can be customized to display menu
    /// entries in different ways. This also provides an event that will be raised
    /// when the menu entry is selected.
    /// </summary>
    class MenuEntry
    {
        #region Fields

        /// <summary>
        /// The text rendered for this entry.
        /// </summary>
        string text;

        /// <summary>
        /// Tracks a fading selection effect on the entry.
        /// </summary>
        /// <remarks>
        /// The entries transition out of the selection effect when they are deselected.
        /// </remarks>
        float selectionFade;

        bool scrollingActive = false;
        float scrollx = 0;
        //float lastscrollx = 0;
        int stillTime = 0;
        //float scrollx2 = 0;
        bool goingback = false;
        bool stayingstill = false;
        bool isscrollinit = false;
        ScreenManager screenManager;
        SpriteBatch spriteBatch;
        SpriteFont font;

        // Draw text, centered on the middle of each line.

        #endregion

        #region Properties


        /// <summary>
        /// Gets or sets the text of this menu entry.
        /// </summary>
        public string Text
        {
            get { return text; }
            set { text = value; }
        }



        #endregion

        #region Events


        /// <summary>
        /// Event raised when the menu entry is selected.
        /// </summary>
        //public event EventHandler<EventArgs> Selected;

        /// <summary>
        /// Event raised when the menu entry is selected.
        /// </summary>
        public event EventHandler<EventArgs> Selected;
        public event EventHandler<EventArgs> Left;
        public event EventHandler<EventArgs> Right;


        /// <summary>
        /// Method for raising the Selected event.
        /// </summary>
        protected internal virtual void OnSelectEntry()
        {
            if (Selected != null)
                Selected(this, EventArgs.Empty);
        }
        /// <summary>
        /// Method for raising the Selected event.
        /// </summary>
        protected internal virtual void OnLeftEntry()
        {
            if (Left != null)
                Selected(this, EventArgs.Empty);
        }

        /// <summary>
        /// Method for raising the Selected event.
        /// </summary>
        protected internal virtual void OnRightEntry()
        {
            if (Right != null)
                Selected(this, EventArgs.Empty);
        }

        #endregion

        #region Initialization


        /// <summary>
        /// Constructs a new menu entry with the specified text.
        /// </summary>
        public MenuEntry(string text)
        {
            this.text = text;
        }

        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the menu entry.
        /// </summary>
        public virtual void Update(MenuScreen screen, bool isSelected,
                                                      GameTime gameTime, ScreenManager screenManager)
        {
            // When the menu selection changes, entries gradually fade between
            // their selected and deselected appearance, rather than instantly
            // popping to the new state.
            float fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4;

            if (isSelected)
                selectionFade = Math.Min(selectionFade + fadeSpeed, 1);
            else
                selectionFade = Math.Max(selectionFade - fadeSpeed, 0);

            if (scrollingActive)
            {
                if (!isscrollinit)
                {
                    stayingstill = false;
                    isscrollinit = true;
                    goingback = true;
                }
                float textsize = screenManager.Font.MeasureString(text).X;
                float offscreenamount = screenManager.Font.MeasureString(text).X - (screenManager.Game.GraphicsDevice.Viewport.Width / 8 * 7);
                float scrollSpeed = 2;

                if (!stayingstill)
                {
                    if (goingback)
                    {
                        if (scrollx < -offscreenamount)
                            stayingstill = true;
                        else
                            scrollx -= scrollSpeed;
                    }
                    else
                    {

                        if (scrollx > 10)
                            stayingstill = true;
                        else
                            scrollx += scrollSpeed;
                    }
                }
                else
                {
                    if (stillTime >= 10)
                    {
                        goingback = !goingback;
                        stayingstill = false;
                        stillTime = 0;
                    }
                    else
                    {
                        stillTime += 1;
                    }
                }
            }

        }
        

        


        /// <summary>
        /// Draws the menu entry. This can be overridden to customize the appearance.
        /// </summary>
        public virtual void Draw(MenuScreen screen, Vector2 position,
                                 bool isSelected, GameTime gameTime)
        {
            // Draw the selected entry in yellow, otherwise white.
            Color color = isSelected ? Color.Yellow : Color.White;
            // Pulsate the size of the selected menu entry.
            double time = gameTime.TotalGameTime.TotalSeconds;

            float pulsate = (float)Math.Sin(time * 6) + 1;

            float scale = 1 + pulsate * 0.015f * selectionFade;

            // Modify the alpha to fade text out during transitions.
            color = new Color(color.R, color.G, color.B, screen.TransitionAlpha);

            // Draw text, centered on the middle of each line.
            screenManager = screen.ScreenManager;
            spriteBatch = screenManager.SpriteBatch;
            font = screenManager.Font;

            Vector2 origin = new Vector2(0, font.LineSpacing / 2);

               
            origin = new Vector2(0, font.LineSpacing / 2);
            if (isSelected && (font.MeasureString(text).X > (screenManager.Game.GraphicsDevice.Viewport.Width/8*7)))
            {
                scrollingActive = true;
                screenManager.DrawShadowedString(spriteBatch, font, text, new Vector2(position.X + scrollx, position.Y), color, 0, origin, scale, SpriteEffects.None, 0);

            }
            else
            {

                screenManager.DrawShadowedString(spriteBatch, font, text, position, color, 0, origin, scale, SpriteEffects.None, 0);

            }
        
        }


        /// <summary>
        /// Queries how much space this menu entry requires.
        /// </summary>
        public virtual int GetHeight(MenuScreen screen)
        {
                return screen.ScreenManager.Font.LineSpacing;
            
        }

        #endregion
    }
}
