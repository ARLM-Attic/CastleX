#region File Description
//-----------------------------------------------------------------------------
// MenuScreen.cs
//
// XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace CastleX
{
    /// <summary>
    /// Base class for screens that contain a menu of options. The user can
    /// move up and down to select an entry, or cancel to back out of the screen.
    /// </summary>
    /// 

    abstract class MenuScreen : GameScreen
    {
        #region Fields

        List<MenuEntry> menuEntries = new List<MenuEntry>();
        int selectedEntry = 0;
        string menuTitle;
        int menunumber = 0;
        bool ismainmenu = false;

        /*int varY = 0;
        int varY2 = 40;
        int lineheight = 0;
        int topnumber = 0;*/

        public bool MenuUp { get; set; }
        public bool MenuDown { get; set; }
        public bool MenuLeft { get; set; }
        public bool MenuRight { get; set; }
        public bool MenuSelect { get; set; }
        public bool MenuCancel { get; set; }
        public bool isAnyButtonPressed
        {
            get
            {
                return MenuUp || MenuDown || MenuLeft || MenuRight || MenuSelect || MenuCancel;
            }
            set { isAnyButtonPressed = value; }
        }

        #endregion

        #region Properties
        

        /// <summary>
        /// Gets the list of menu entries, so derived classes can add
        /// or change the menu contents.
        /// </summary>
        protected IList<MenuEntry> MenuEntries
        {
            get { return menuEntries; }
        }

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public MenuScreen(string menuTitle)
        {
            this.menuTitle = menuTitle;
            ismainmenu = false;
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }



        /// <summary>
        /// Constructor.
        /// </summary>
        public MenuScreen(string menuTitle, int menuNumber)
        {
            this.menuTitle = menuTitle;
            menunumber = menuNumber;
            if (menunumber == 1)
                ismainmenu = true;

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        #endregion

        #region Handle Input


        /// <summary>
        /// Responds to user input, changing the selected entry and accepting
        /// or cancelling the menu.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            // Move to the previous menu entry?
            if (input.MenuUp)
            {
                selectedEntry--;

                if (selectedEntry < 0)
                    selectedEntry = menuEntries.Count - 1;
            }

            // Move to the next menu entry?
            if (input.MenuDown)
            {
                selectedEntry++;

                if (selectedEntry >= menuEntries.Count)
                    selectedEntry = 0;
            }

            // Accept or cancel the menu?
            if (input.MenuSelect)
            {
                OnSelectEntry(selectedEntry);
            }
            else if (input.MenuCancel)
            {
                OnCancel();
            }

            // if the user presses Left or Right on the menu
            if (input.MenuLeft)
            {
                OnLeftEntry(selectedEntry);
            }
            else if (input.MenuRight)
            {
                OnRightEntry(selectedEntry);
            }


            MenuUp = input.MenuUp;
            MenuDown = input.MenuDown;
            MenuLeft = input.MenuLeft;
            MenuRight = input.MenuRight;
            MenuSelect = input.MenuSelect;
            MenuCancel = input.MenuCancel;
        }


        /// <summary>
        /// Handler for when the user has chosen a menu entry.
        /// </summary>
        protected virtual void OnSelectEntry(int entryIndex)
        {
            menuEntries[selectedEntry].OnSelectEntry();
        }


        /// <summary>
        /// Handler for when the user has chosen left on a menu entry.
        /// </summary>
        protected virtual void OnLeftEntry(int entryIndex)
        {
            menuEntries[selectedEntry].OnLeftEntry();
        }
        /// <summary>
        /// Handler for when the user has chosen left on a menu entry.
        /// </summary>
        protected virtual void OnRightEntry(int entryIndex)
        {
            menuEntries[selectedEntry].OnRightEntry();
        }


        /// <summary>
        /// Handler for when the user has cancelled the menu.
        /// </summary>
        protected virtual void OnCancel()
        {
            ExitScreen();
        }


        /// <summary>
        /// Helper overload makes it easy to use OnCancel as a MenuEntry event handler.
        /// </summary>
        protected void OnCancel(object sender, EventArgs e)
        {
            OnCancel();
        }


        #endregion

        #region Update and Draw

        /// <summary>
        /// Updates the menu.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            // Update each nested MenuEntry object.
            for (int i = 0; i < menuEntries.Count; i++)
            {
                bool isSelected = IsActive && (i == selectedEntry);

                menuEntries[i].Update(this, isSelected, gameTime, ScreenManager);
            }
        }


        /// <summary>
        /// Draws the menu.
        /// </summary>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            SpriteFont font = ScreenManager.Font;
            SpriteFont mainMenuFont = ScreenManager.MainMenuFont;

            // Starting Position of the Menu Items
            Vector2 position = new Vector2(10, 150);

            // Calculate the total hieght of the menu items
            float tempHeight = 0.0f;
            for (int i = 0; i < menuEntries.Count; i++)
            {
                MenuEntry menuEntry = menuEntries[i];
                tempHeight += menuEntry.GetHeight(this);
            }
            // Calculate the New Starting Position for the Menu Items
            // This will draw the menu from the bottom up.
            position = new Vector2(10, (ScreenManager.GraphicsDevice.Viewport.Height - 10) - tempHeight);

            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            if (ScreenState == ScreenState.TransitionOn)
                position.X -= transitionOffset * 256;
            else
                position.X += transitionOffset * 512;

            //SpriteBatch.Begin();

            // Draw each menu entry in turn.
            for (int i = 0; i < menuEntries.Count; i++)
            {
                MenuEntry menuEntry = menuEntries[i];

                bool isSelected = IsActive && (i == selectedEntry);

                menuEntry.Draw(this, position, isSelected, gameTime);

                position.Y += menuEntry.GetHeight(this);
            }

            // Draw the menu title.

            Vector2 titlePosition = new Vector2(10, 80);
            Vector2 subTitlePosition = new Vector2(10, 10);
            //Vector2 titleOrigin = font.MeasureString(menuTitle) / 2;
            Vector2 titleOrigin = Vector2.Zero;

            Color titleColor = new Color(100, 100, 100, TransitionAlpha);
            Color subTitleColor = new Color(192, 000, 000, TransitionAlpha);
            titlePosition.Y -= transitionOffset * 100;

            if (ismainmenu)
            {
                ScreenManager.DrawShadowedString(spriteBatch, mainMenuFont, "Castle X", new Vector2(50, 10), subTitleColor);
                ScreenManager.DrawShadowedString(spriteBatch, font, menuTitle, new Vector2((ScreenManager.GraphicsDevice.Viewport.Width / 10 * 1), ScreenManager.GraphicsDevice.Viewport.Height / 6 * 2), titleColor);
            }
            else
            {
                ScreenManager.DrawShadowedString(spriteBatch, font, menuTitle, new Vector2((ScreenManager.GraphicsDevice.Viewport.Width / 10 * 1), ScreenManager.GraphicsDevice.Viewport.Height / 6 * 1), titleColor);
            }
            //SpriteBatch.End();
        }
        #endregion
    }

}