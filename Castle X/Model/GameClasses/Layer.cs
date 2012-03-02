using System;
using System.Collections.Generic;
//using System.Data.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;

namespace CastleX
{
    class Layer
    {
        public Texture2D[] Textures { get; private set; }
        public float ScrollRate { get; private set; }
        Texture2D AltLayer;
        bool errorloadinglayer = false;
        private ScreenManager screenManager;
        public Layer(int baseNumber, float scrollRate, ScreenManager screenManager)
        {
            this.screenManager = screenManager;
            try
            {
                Textures = new Texture2D[3];

                if (baseNumber == 0)
                {
                    Textures[0] = screenManager.Layer0_0Texture;
                    Textures[1] = screenManager.Layer0_1Texture;
                    Textures[2] = screenManager.Layer0_2Texture;
                }
                else if (baseNumber == 1)
                {
                    Textures[0] = screenManager.Layer1_0Texture;
                    Textures[1] = screenManager.Layer1_1Texture;
                    Textures[2] = screenManager.Layer1_2Texture;
                }
                else if (baseNumber == 2)
                {
                    Textures[0] = screenManager.Layer2_0Texture;
                    Textures[1] = screenManager.Layer2_1Texture;
                    Textures[2] = screenManager.Layer2_2Texture;
                }
            }
            catch
            {
                AltLayer = screenManager.DefaultBGTexture;
                errorloadinglayer = true;
            }

            ScrollRate = scrollRate;
        }

        public Layer(int baseNumber, float scrollRate, ScreenManager screenManager, ContentManager content, String[] textureNames)
        {
            try
            {
                Textures = new Texture2D[3];

                for (int i = 0; i < 3; i++)
                {
                    try
                    {
                        if (screenManager.SkinSettings.hasBackgrounds)
                            Textures[i] = content.Load<Texture2D>("Skins/" + screenManager.Settings.Skin + "/Backgrounds/" + textureNames[i]);
                        else
                            Textures[i] = content.Load<Texture2D>("Skins/0/Backgrounds/" + textureNames[i]);
                    }
                    catch
                    {
                        Textures[i] = content.Load<Texture2D>("Skins/0/Backgrounds/" + textureNames[i]);
                    }
                }
            }
            catch
            {
                AltLayer = screenManager.DefaultBGTexture;
                errorloadinglayer = true;
            }

            ScrollRate = scrollRate;
        }

        public void Draw(SpriteBatch spriteBatch, float cameraPosition)
        {
            // Assume each segment is the same width.
            int segmentWidth = Textures[0].Width;

            // Calculate which segments to draw and how much to offset them.
            float x = cameraPosition * ScrollRate;
            int leftSegment = (int)Math.Floor(x / segmentWidth);
            int rightSegment = leftSegment + 1;
            x = (x / segmentWidth - leftSegment) * -segmentWidth;

            if (errorloadinglayer)
            {
                spriteBatch.Draw(AltLayer, new Rectangle(0, Convert.ToInt32(ScreenManager.HUDHeight), 240, 320), Color.White);
            }
            else
            {
                spriteBatch.Draw(Textures[leftSegment % Textures.Length], new Vector2(x, ScreenManager.HUDHeight), Color.White);
                spriteBatch.Draw(Textures[rightSegment % Textures.Length], new Vector2(x + segmentWidth, ScreenManager.HUDHeight), Color.White);

            }
        }
        public void Draw(SpriteBatch spriteBatch, float cameraPosition, Color color)
        {
            // Assume each segment is the same width.
            int segmentWidth = Textures[0].Width;

            // Calculate which segments to draw and how much to offset them.
            float x = cameraPosition * ScrollRate;
            int leftSegment = (int)Math.Floor(x / segmentWidth);
            int rightSegment = leftSegment + 1;
            x = (x / segmentWidth - leftSegment) * -segmentWidth;

            if (errorloadinglayer)
            {
                spriteBatch.Draw(AltLayer, new Rectangle(0, Convert.ToInt32(ScreenManager.HUDHeight), 240, 320), Color.White);
            }
            else
            {
                spriteBatch.Draw(Textures[leftSegment % Textures.Length], new Vector2(x,ScreenManager.HUDHeight), Color.White);
                spriteBatch.Draw(Textures[rightSegment % Textures.Length], new Vector2(x + segmentWidth, ScreenManager.HUDHeight), Color.White);

            }

        }
    }
}
