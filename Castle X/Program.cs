using System;
// Copyright and stuff
// ICON:    Carl Johan Rehbinder - Rehbinder MultiArt Productions
//          E-mail: cjr@telia.com
//          Homepage: http://www.multiart.nu/cci
// Sounds and Graphics: * Remake of Maze of Galious from Brain Games
//                      http://www.braingames.getput.com/mog/
//                      * "Romeo" sample game from Click Team's Multimedia Fusion 
//                      http://www.clickteam.com 
// Code: Platformer Expanded, expansions to XNA Platformer Starter Kit by LordKtulu
//      http://forums.create.msdn.com/forums/t/34901.aspx, https://users.create.msdn.com/Profile/LordKtulu
//

namespace CastleX
{
    static class Program
    {        
        static bool debugging = false;
        // TO FORCE THE ERROR ENGINE TO BELIEVE IT ISNT IN DEBUG MODE, CHANGE THE VARIABLE BELOW TO TRUE
        static bool manualdebugdisable = false;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
#if WINDOWS
     [STAThreadAttribute]    
#endif
        static void Main(string[] args)
        {        
#if DEBUG
            if (!manualdebugdisable && System.Diagnostics.Debugger.IsAttached)
               debugging = true;
#endif

            if (debugging)
            {
                using (CastleXGame game = new CastleXGame())
                   game.Run();
            }
            else
            {
                while (true)
                {
                    try
                    {
                        using (CastleXGame game = new CastleXGame())
                        {
                            game.Run();
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        if (!e.ToString().Contains("RestartRequired"))
                        {
                            try
                            {
                                using (Error bsod = new Error(e))
                                    bsod.Run();
                            }
                            catch
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}
