using System;
using System.Collections.Generic;
using System.Linq; 
using System.Text;

namespace CastleX
{
    public class SkinSettings
    {
        public string skinTitle
        {
            get;
            set;
        }
        public bool hasBackgrounds
        {
            get;
            set;
        }
        public bool hasOverlays
        {
            get;
            set;
        }
        public bool hasSprites
        {
            get;
            set;
        }
        public bool hasTiles
        {
            get;
            set;
        }
        public bool hasSounds
        {
            get;
            set;
        }
        public bool hasMusic
        {
            get;
            set;
        }
        public int FrameWidth_Player_Celebrate
        {
            get;
            set;
        }
        public int FrameWidth_Player_Die
        {
            get;
            set;
        }
        public int FrameWidth_Player_Idle
        {
            get;
            set;
        }
        public int FrameWidth_Player_Jump
        {
            get;
            set;
        }
        public int FrameWidth_Player_Run
        {
            get;
            set;
        }
        public int FrameWidth_Player_Climbing
        {
            get;
            set;
        }

        // Enemies frame widths
        public int[] FrameWidth_Monster_Run = new int[4];
        public int[] FrameWidth_Monster_Die = new int[4];
        public int[] FrameWidth_Monster_Idle = new int[4];

        public int[] FrameWidth_Ghost_Run = new int[4];
        public int[] FrameWidth_Ghost_Die = new int[4];
        public int[] FrameWidth_Ghost_Idle = new int[4];

        public int[] FrameWidth_Flying_Run = new int[4];
        public int[] FrameWidth_Flying_Die = new int[4];
        public int[] FrameWidth_Flying_Idle = new int[4];
    }
}