using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Audio;

namespace CastleX
{
    /// <summary>
    /// The application settings.
    /// </summary>
    public class PESettings
    {

        bool showlevelintro = false;  // Fixed in code, developer can change for specific levels
        public bool ShowLevelIntro
        {
            get { return showlevelintro; }
            set { showlevelintro = value; }
        }

        bool showfps = true;
        public bool DebugMode
        {
            get { return showfps; }
            set { showfps = value; }
        }

        bool customlevelnote = false;
        public bool customLevelNote
        {
            get { return customlevelnote; }
            set { customlevelnote = value; }
        }

        bool inGameMusic = true;
        public bool InGameMusic
        {
            get { return inGameMusic; }
            set { inGameMusic = value; }
        }
        float soundVolumeAmount = 0.6f;
        public float SoundVolumeAmount
        {
            get { return soundVolumeAmount; }
            set { soundVolumeAmount = value; }
        }
        int soundVolumeNumber = 3;
        public int SoundVolumeNumber
        {
            get { return soundVolumeNumber; }
            set { soundVolumeNumber = value; }
        }
        string soundVolumeString = "Medium";
        public string SoundVolumeString
        {
            get { return soundVolumeString; }
            set { soundVolumeString = value; }
        }

        float musicVolumeAmount = 1.0f;
        public float MusicVolumeAmount
        {
            get { return musicVolumeAmount; }
            set { musicVolumeAmount = value; }
        }
        int musicVolumeNumber = 4;
        public int MusicVolumeNumber
        {
            get { return musicVolumeNumber; }
            set { musicVolumeNumber = value; }
        }
        string musicVolumeString = "High";
        public string MusicVolumeString
        {
            get { return musicVolumeString; }
            set { musicVolumeString = value; }
        }

        bool levelJumpUnlocked = false;
        public bool LevelJumpUnlocked
        {
            get { return levelJumpUnlocked; }
            set { levelJumpUnlocked = value; }
        }
        bool levelJumpEnabled = false;
        public bool LevelJumpEnabled
        {
            get { return levelJumpEnabled; }
            set { levelJumpEnabled = value; }
        }

        string levelFolderPath = "";
        public string LevelFolderPath
        {
            get { return levelFolderPath; }
            set { levelFolderPath = value; }
        }

        int skin = 0;
        public int Skin
        {
            get { return skin; }
            set { skin = value; }
        }

        bool customFolder = true;
        public bool CustomFolder
        {
            get { return customFolder; }
            set { customFolder = value; }
        }

        int loadLevelsFrom = 0;
        /// <summary>
        /// loadLevelsFrom tells where to load the levels when using the pc version. 
        /// If set to 0, the game will load the levels from the interior games folder.
        /// If set to 1, the game will load levels from the directory folder on the users computer
        /// (My Documents\SavedGames\Castle_X\Content\Levels).
        /// If set to 2, the game will load levels from a directory the user specifies.
        /// </summary>
        public int LoadLevelsFrom
        {
            get { return loadLevelsFrom; }
            set { loadLevelsFrom = value; }
        }


    }
}