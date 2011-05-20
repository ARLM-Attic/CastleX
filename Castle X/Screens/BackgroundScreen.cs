#region File Description
//-----------------------------------------------------------------------------
// BackgroundScreen.cs
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
using System.Threading;
using Microsoft.Xna.Framework.Storage;
using System.IO;
using System.Diagnostics;
#endregion

namespace CastleX
{
    /// <summary>
    /// The background screen sits behind all the other menu screens.
    /// It draws a background image that remains fixed in place regardless
    /// of whatever transitions the screens on top of it may be doing.
    /// </summary>
    
    class BackgroundScreen : GameScreen
    {
        #region Fields

        private Layer[] layers;
        bool errorloadinglayer = false;
        Texture2D AltLayer;
        float cameraPosition = 0;
        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public BackgroundScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }


        /// <summary>
        /// Loads graphics content for this screen. The background texture is quite
        /// big, so we use our own local ContentManager to load it. This allows us
        /// to unload before going from the menus into the game itself, wheras if we
        /// used the shared ContentManager provided by the Game class, the content
        /// would remain loaded forever.
        /// </summary>
        public override void LoadContent()
        {
            try{
                layers = new Layer[3];
                layers[0] = new Layer(0, 0.2f, ScreenManager);
                layers[1] = new Layer(1, 0.5f, ScreenManager);
                layers[2] = new Layer(2, 0.8f, ScreenManager);
            
            }
            catch
            {
                errorloadinglayer = true;
                AltLayer = ScreenManager.DefaultBGTexture;
            }
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the background screen. Unlike most screens, this should not
        /// transition off even if it has been covered by another screen: it is
        /// supposed to be covered, after all! This overload forces the
        /// coveredByOtherScreen parameter to false in order to stop the base
        /// Update method wanting to transition off.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            if (!errorloadinglayer)
            cameraPosition += 1;
            base.Update(gameTime, otherScreenHasFocus, false);
        }


        /// <summary>
        /// Draws the background screen.
        /// </summary>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);
            byte fade = TransitionAlpha;

            //SpriteBatch.Begin();

            if (errorloadinglayer)
            {
                spriteBatch.Draw(AltLayer, new Rectangle(0, 0, 240, 320), new Color(fade, fade, fade));
            }
            else
            {
                for (int i = 0; i <= 2; ++i)
                    layers[i].Draw(spriteBatch, cameraPosition, new Color(fade, fade, fade));
            }
               
            //SpriteBatch.End();

        }


        #endregion
    }

}
