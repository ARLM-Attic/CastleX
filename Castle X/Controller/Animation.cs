using System;
using Microsoft.Xna.Framework.Graphics;

namespace CastleX
{
    /// <summary>
    /// Represents an animated texture.
    /// </summary>
    /// <remarks>
    /// Currently, this class assumes that each frame of animation is
    /// as wide as each animation is tall. The number of frames in the
    /// animation are inferred from this.
    /// </remarks>
    internal class Animation
    {
        /// <summary>
        /// All frames in the animation arranged horizontally.
        /// </summary>
        public Texture2D Texture
        {
            get { return texture; }
        }
        Texture2D texture;

        /// <summary>
        /// Duration of time to show each frame.
        /// </summary>
        public float FrameTime
        {
            get { return frameTime; }
        }
        float frameTime;

        /// <summary>
        /// When the end of the animation is reached, should it
        /// continue playing from the beginning?
        /// </summary>
        public bool IsLooping
        {
            get { return isLooping; }
            set { IsLooping = value; }
        }
        bool isLooping;

        /// <summary>
        /// Gets the number of frames in the animation.
        /// </summary>
        public int FrameCount
        {
            get { return Texture.Width / FrameWidth; }
        }

        /// <summary>
        /// Gets the width of a frame in the animation.
        /// </summary>
        public int FrameWidth
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the height of a frame in the animation.
        /// </summary>
        public int FrameHeight
        {
            get { return Texture.Height; }
        }


        /// <summary>
        /// Constructs a new animation with a predefined with.
        /// </summary>        
        public Animation(Texture2D texture, float frameTime, bool isLooping, int frameWidth)
        {
            this.texture = texture;
            this.frameTime = frameTime;
            this.isLooping = isLooping;
            if (frameWidth == 0)
                this.FrameWidth = texture.Height;
            else if (frameWidth > 0)
                this.FrameWidth = frameWidth;
        }
        /// <summary>
        /// Constructs a new animation with the width being the same as the height.
        /// </summary>        
        public Animation(Texture2D texture, float frameTime, bool isLooping)
        {
            this.texture = texture;
            this.frameTime = frameTime;
            this.isLooping = isLooping;
            this.FrameWidth = texture.Height;
        }
    }
}
