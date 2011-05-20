using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace CastleX
{
    /// <summary>
    /// The Entrance point.
    /// </summary>
    class Entrance
    {
        private int entranceNumber;
        public int EntranceNumber
        {
            get { return entranceNumber; }
            set { entranceNumber = value; }
        }

        /// <summary>
        /// Gets the current position of this tester in world space.
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
        }
        private Vector2 position;

        /// <summary>
        /// Constructs a new exit.
        /// </summary>
        public Entrance(Vector2 position, int entranceNumber)
        {
            this.position = position;
            this.entranceNumber = entranceNumber;
        }
    }
}
