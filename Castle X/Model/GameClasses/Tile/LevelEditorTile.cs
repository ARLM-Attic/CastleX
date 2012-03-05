using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CastleX
{
    enum EditorTileObject
    {
        Blank = 1,
        Block = 2,
        Coin = 3,
        Player = 4,
        BlockPassable = 5,
        Platform = 6,
        MonsterA = 7,
        MonsterB = 8,
        MonsterC = 9,
        MonsterD = 10,
        FallingTile = 11,
        Boss = 12,
        Checkpoint = 13,
        DeathTile = 14,
        Exit = 15,
        Heart = 16,
        HiddenTile = 17,
        HiddenTilePassable = 18,
        JumperTile = 19,
        Ladder = 20,
        Life = 21,
        PowerUp = 22,
        Timer = 23,
        VanishingTile = 24,
    }


    /// <summary>
    /// Stores the appearance and collision behavior of a tile.
    /// </summary>
    class LevelEditorTile
    {

        public bool isSelected(Vector2 cursor)
        {
            if (cursor == Location)
                return true;
            else
                return false;
        }

        public EditorTileObject ItemType;
        public Vector2 Location;
        public int Number;
        public Color Color;
        public Texture2D texture;
        public ScreenManager screenManager;
        public static int Width = 20;
        public static int Height = 20;
        public char ItemCharacter;

        /// <summary>
        /// Constructs a new tile.
        /// </summary>
        public LevelEditorTile(ScreenManager screenManager, char itemChar, int itemnumber, int x, int y)
        {
            Color = Color.White;
            Location = new Vector2(x, y);
            Number = itemnumber;
            this.screenManager = screenManager;
            this.ItemCharacter = itemChar;

            #region initial item selector
            switch (itemChar)
            {
                case '.':
                    ItemType = EditorTileObject.Blank;
                    break;
                case 'X':
                    ItemType = EditorTileObject.Exit;
                    break;
                case 'R':
                    ItemType = EditorTileObject.FallingTile;
                    break;
                case 'L':
                    ItemType = EditorTileObject.Ladder;
                    break;
                case 'G':
                    ItemType = EditorTileObject.Coin;
                    break;
                case 'P':
                    ItemType = EditorTileObject.PowerUp;
                    break;
                case 'l':
                    ItemType = EditorTileObject.Life;
                    break;
                case '+':
                    ItemType = EditorTileObject.Heart;
                    break;
                case 'F':
                    ItemType = EditorTileObject.Checkpoint;
                    break;
                case 'T':
                    ItemType = EditorTileObject.Timer;
                    break;
                case '-':
                    ItemType = EditorTileObject.Platform;
                    break;
                case 'J':
                    ItemType = EditorTileObject.JumperTile;
                    break;
                case 'V':
                    ItemType = EditorTileObject.VanishingTile;
                    break;
                case 'H':
                    ItemType = EditorTileObject.HiddenTile;
                    break;
                case 'h':
                    ItemType = EditorTileObject.HiddenTilePassable;
                    break;
                case '*':
                    ItemType = EditorTileObject.DeathTile;
                    break;
                case 'A':
                    ItemType = EditorTileObject.MonsterA;
                    break;
                case 'B':
                    ItemType = EditorTileObject.MonsterB;
                    break;
                case 'C':
                    ItemType = EditorTileObject.MonsterC;
                    break;
                case 'D':
                    ItemType = EditorTileObject.MonsterD;
                    break;
                case 'S':
                    ItemType = EditorTileObject.Boss;
                    break;
                case '~':
                    ItemType = EditorTileObject.Platform;
                    break;
                case ':':
                    ItemType = EditorTileObject.BlockPassable;
                    break;
                case '1':
                    ItemType = EditorTileObject.Player;
                    break;
                case '#':
                    ItemType = EditorTileObject.Block;
                    break;
                // If a section is unknown, then we will replace it with a blank spot
                default:
                    ItemType = EditorTileObject.Blank;
                    break;
            }
            #endregion
            RefreshTileTexture();
        }
        public void Toggle()
        {
            if ((int)ItemType < 24)
                ItemType++;
            else
                ItemType = EditorTileObject.Blank;
            RefreshTileTexture();
        }
        public void RefreshTileTexture()
        {
            //switch (ItemType)
            //{
            //    case EditorTileObject.Blank:
            //        texture = screenManager.BlankTexture;
            //        Color = Color.White;
            //        ItemCharacter = '.';
            //        break;

            //    case EditorTileObject.Exit:
            //        texture = screenManager.ExitTexture;
            //        Color = Color.White;
            //        ItemCharacter = 'X';
            //        break;

            //    case EditorTileObject.FallingTile:
            //        texture = screenManager.FallingTileTexture;
            //        Color = Color.White;
            //        ItemCharacter = 'R';
            //        break;

            //    case EditorTileObject.Ladder:
            //        texture = screenManager.LadderTexture;
            //        Color = Color.White;
            //        ItemCharacter = 'L';
            //        break;

            //    case EditorTileObject.Gem:
            //        texture = screenManager.GemTexture;
            //        Color = Color.White;
            //        ItemCharacter = 'G';
            //        break;

            //    case EditorTileObject.PowerUp:
            //        texture = screenManager.GemTexture;
            //        Color = Color.Red;
            //        ItemCharacter = 'P';
            //        break;

            //    case EditorTileObject.Life:
            //        texture = screenManager.LifeTexture;
            //        Color = Color.White;
            //        ItemCharacter = 'l';
            //        break;

            //    case EditorTileObject.Heart:
            //        texture = screenManager.HeartIconTexture;
            //        Color = Color.White;
            //        ItemCharacter = '+';
            //        break;

            //    case EditorTileObject.Checkpoint:
            //        texture = screenManager.Checkpoint1Texture;
            //        Color = Color.White;
            //        ItemCharacter = 'F';
            //        break;

            //    case EditorTileObject.Timer:
            //        texture = screenManager.TimeClockTexture;
            //        Color = Color.White;
            //        ItemCharacter = 'T';
            //        break;

            //    case EditorTileObject.Platform:
            //        texture = screenManager.PlatformTexture;
            //        Color = Color.White;
            //        ItemCharacter = '-';
            //        break;

            //    case EditorTileObject.JumperTile:
            //        texture = screenManager.BlockATexture[0];
            //        Color = Color.Blue;
            //        ItemCharacter = 'J';
            //        break;

            //    case EditorTileObject.VanishingTile:
            //        texture = screenManager.BlockATexture[0];
            //        Color = Color.DarkGray;
            //        ItemCharacter = 'V';
            //        break;

            //    case EditorTileObject.HiddenTile:
            //        texture = screenManager.BlockATexture[0];
            //        Color = Color.LightGray;
            //        ItemCharacter = 'H';
            //        break;

            //    case EditorTileObject.HiddenTilePassable:
            //        texture = screenManager.BlockATexture[0];
            //        Color = Color.LightGreen;
            //        ItemCharacter = 'h';
            //        break;

            //    case EditorTileObject.DeathTile:
            //        texture = screenManager.BlockATexture[0];
            //        Color = Color.Red;
            //        ItemCharacter = '*';
            //        break;

            //    case EditorTileObject.MonsterA:
            //        texture = screenManager.Content.Load<Texture2D>("Mapeditor/monstera");
            //        Color = Color.White;
            //        ItemCharacter = 'A';
            //        break;

            //    case EditorTileObject.MonsterB:
            //        texture = screenManager.Content.Load<Texture2D>("Mapeditor/monsterb");
            //        Color = Color.White;
            //        ItemCharacter = 'B';
            //        break;

            //    case EditorTileObject.MonsterC:
            //        texture = screenManager.Content.Load<Texture2D>("Mapeditor/monsterc");
            //        Color = Color.White;
            //        ItemCharacter = 'C';
            //        break;

            //    case EditorTileObject.MonsterD:
            //        texture = screenManager.Content.Load<Texture2D>("Mapeditor/monsterd");
            //        Color = Color.White;
            //        ItemCharacter = 'D';
            //        break;

            //    case EditorTileObject.Boss:
            //        texture = screenManager.PlayerIdleTexture;
            //        Color = Color.Red;
            //        ItemCharacter = 'S';
            //        break;

            //    case EditorTileObject.BlockPassable:
            //        texture = screenManager.BlockBTexture[0];
            //        Color = Color.Gray;
            //        ItemCharacter = ':';
            //        break;

            //    case EditorTileObject.Player:
            //        texture = screenManager.PlayerIdleTexture;
            //        Color = Color.White;
            //        ItemCharacter = '1';
            //        break;

            //    case EditorTileObject.Block:
            //        texture = screenManager.BlockATexture[0];
            //        Color = Color.White;
            //        ItemCharacter = '#';
            //        break;

            //    default:
            //        break;

            //}            
        }
    }
}
