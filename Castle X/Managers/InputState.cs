#region File Description
//-----------------------------------------------------------------------------
// InputState.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
#endregion

namespace CastleX
{
    /// <summary>
    /// Helper for reading input from keyboard and gamepad. This class tracks both
    /// the current and previous state of both input devices, and implements query
    /// properties for high level input actions such as "move up through the menu"
    /// or "pause the game".
    /// </summary>
    public class InputState
    {
        #region Fields

        public const int MaxInputs = 4;


        public readonly GamePadState[] CurrentGamePadStates;
        public readonly GamePadState[] LastGamePadStates;
        public readonly GamePadState[] CurrentTouchPadStates;
        public readonly GamePadState[] LastTouchPadStates;
        public readonly KeyboardState[] CurrentKeyBoardStates;
        public readonly KeyboardState[] LastKeyBoardStates;

        private ScreenManager screenManager;
        int presstimer = 0;
        //bool hasTouchEnabled = false;
        const float FlickLength = 2.0f;

        #endregion

        #region Initialization

        /*
        /// <summary>
        /// Constructs a new input state.
        /// </summary>
        public InputState()
        {
#if WINDOWS
            CurrentKeyboardStates = new KeyboardState[MaxInputs];
            LastKeyboardStates = new KeyboardState[MaxInputs];
#endif
            CurrentGamePadStates = new GamePadState[MaxInputs];
            LastGamePadStates = new GamePadState[MaxInputs];
        }*/

        /// <summary>
        /// Constructs a new input state.
        /// </summary>
        public InputState(ScreenManager newScreenManager)
        {
            CurrentGamePadStates = new GamePadState[MaxInputs];
            LastGamePadStates = new GamePadState[MaxInputs];
            CurrentTouchPadStates = new GamePadState[MaxInputs];
            LastTouchPadStates = new GamePadState[MaxInputs];
            CurrentKeyBoardStates = new KeyboardState[MaxInputs];
            LastKeyBoardStates = new KeyboardState[MaxInputs];
            CurrentGamePadStates = new GamePadState[MaxInputs];
            LastGamePadStates = new GamePadState[MaxInputs];
            screenManager = newScreenManager;
        }


        #endregion

        #region Properties

        public bool isAnyButton
        {
            get { return Up_Held || Up_Pressed || Up_PressRelease || Down_Held || Down_Pressed || Down_PressRelease || Left_Held || Left_Pressed || Left_PressRelease || Right_Held || Right_Pressed || Right_PressRelease || A_Held || A_Pressed || A_PressRelease || B_Held || B_Pressed || B_PressRelease || Back_Held || Back_Pressed || Back_PressRelease; }
            set { isAnyButton = value; }

        }
        public bool isAnyMenuButton
        {
            get { return MenuUp || MenuDown || MenuLeft || MenuRight || MenuSelect || MenuCancel; }
            set { isAnyButton = value; }

        }

        public bool isAnyButtonHeld
        {
            get { return Up_Held || Down_Held || Left_Held || Right_Held || A_Held || B_Held || Back_Held; }
            set { isAnyButtonHeld = value; }

        }
        public bool isAnyButtonPressed
        {
            get { return Up_Pressed || Down_Pressed || Left_Pressed || Right_Pressed || A_Pressed || B_Pressed || Back_Pressed; }
            set { isAnyButtonPressed = value; }

        }
        public bool isAnyButtonPressRelease
        {
            get { return Up_PressRelease || Down_PressRelease || Left_PressRelease || Right_PressRelease || A_PressRelease || B_PressRelease || Back_PressRelease; }
            set { isAnyButtonPressRelease = value; }

        }

        public void TraceInput()
        {
            Trace.Write("Up_Held: " + Up_Held.ToString() + "\n");
            Trace.Write("Up_Pressed: " + Up_Pressed.ToString() + "\n");
            Trace.Write("Up_PressRelease: " + Up_PressRelease.ToString() + "\n");
            Trace.Write("Down_Held: " + Down_Held.ToString() + "\n");
            Trace.Write("Down_Pressed: " + Down_Pressed.ToString() + "\n");
            Trace.Write("Down_PressRelease: " + Down_PressRelease.ToString() + "\n");
            Trace.Write("Left_Held: " + Left_Held.ToString() + "\n");
            Trace.Write("Left_Pressed: " + Left_Pressed.ToString() + "\n");
            Trace.Write("Left_PressRelease: " + Left_PressRelease.ToString() + "\n");
            Trace.Write("Right_Held: " + Right_Held.ToString() + "\n");
            Trace.Write("Right_Pressed: " + Right_Pressed.ToString() + "\n");
            Trace.Write("Right_PressRelease: " + Right_PressRelease.ToString() + "\n");
            Trace.Write("A_Held: " + A_Held.ToString() + "\n");
            Trace.Write("A_Pressed: " + A_Pressed.ToString() + "\n");
            Trace.Write("A_PressRelease: " + A_PressRelease.ToString() + "\n");
            Trace.Write("B_Held: " + A_Held.ToString() + "\n");
            Trace.Write("B_Pressed: " + A_Pressed.ToString() + "\n");
            Trace.Write("B_PressRelease: " + A_PressRelease.ToString() + "\n");
            Trace.Write("Back_Held: " + A_Held.ToString() + "\n");
            Trace.Write("Back_Pressed: " + A_Pressed.ToString() + "\n");
            Trace.Write("Back_PressRelease: " + A_PressRelease.ToString() + "\n");


        }


        /// <summary>
        /// Checks for a "menu up" input action, from any player,
        /// on either keyboard or gamepad.
        /// </summary>
        public bool MenuUp
        {
            get
            {
#if WINDOWS
                return IsNewKeyPress(Keys.Up) ||
                    IsNewButtonPress(Buttons.DPadUp);
#else
                return IsNewButtonPress(Buttons.DPadUp) ||
                    IsNewButtonTouch(Buttons.LeftThumbstickUp);
#endif

            }
        }


        /// <summary>
        /// Checks for a "menu down" input action, from any player,
        /// on either keyboard or gamepad.
        /// </summary>
        public bool MenuDown
        {
            get
            {
#if WINDOWS
                return IsNewKeyPress(Keys.Down) ||
                       IsNewButtonPress(Buttons.DPadDown);
#else
                return IsNewButtonPress(Buttons.DPadDown) ||
                    IsNewButtonTouch(Buttons.LeftThumbstickDown);
#endif
            }
        }

        /// <summary>
        /// Checks for a "menu left" input action, from any player,
        /// on either keyboard or gamepad.
        /// </summary>
        public bool MenuLeft
        {
            get
            {
#if WINDOWS
                return IsNewKeyPress(Keys.Left) ||
                       IsNewButtonPress(Buttons.DPadLeft);
#else
                return IsNewButtonPress(Buttons.DPadLeft) ||
                    IsNewButtonTouch(Buttons.LeftThumbstickLeft);
#endif
            }
        }

        /// <summary>
        /// Checks for a "menu right" input action, from any player,
        /// on either keyboard or gamepad.
        /// </summary>
        public bool MenuRight
        {
            get
            {
#if WINDOWS
                return IsNewKeyPress(Keys.Right) ||
                       IsNewButtonPress(Buttons.DPadRight);
#else
                return IsNewButtonPress(Buttons.DPadRight) ||
                    IsNewButtonTouch(Buttons.LeftThumbstickRight);
#endif
            }
        }
        /// <summary>
        /// Checks for a "menu select" input action, from any player,
        /// on either keyboard or gamepad.
        /// </summary>
        public bool MenuSelect
        {
            get
            {
#if WINDOWS
                return IsNewKeyPress(Keys.Space) ||
                       IsNewKeyPress(Keys.Enter) ||
                       IsNewButtonPress(Buttons.A) ||
                       IsNewButtonPress(Buttons.Start);
#else
                return IsNewButtonPress(Buttons.A) ||
                        IsNewButtonPress(Buttons.Start);
#endif
            }
        }

        /// <summary>
        /// Checks for a "menu select" input action, from any player,
        /// on either keyboard or gamepad.
        /// </summary>
        public bool MenuSelect2
        {
            get
            {
#if WINDOWS
                return IsNewKeyPress(Keys.RightShift) ||
                       IsNewKeyPress(Keys.LeftShift) ||
                       IsNewButtonPress(Buttons.A) ||
                       IsNewButtonPress(Buttons.Start);
#else
                return IsNewButtonPress(Buttons.B) ||
                        IsNewButtonPress(Buttons.Start);
#endif
            }
        }

        /// <summary>
        /// Checks for a "menu cancel" input action, from any player,
        /// on either keyboard or gamepad.
        /// </summary>
        public bool MenuCancel
        {
            get
            {
#if WINDOWS
                return IsNewKeyPress(Keys.Escape) ||
                    IsNewKeyPress(Keys.Back) ||
                    IsNewButtonPress(Buttons.B) ||
                    IsNewButtonPress(Buttons.Back);
#else
                return IsNewButtonPress(Buttons.Back) ||
                        IsNewButtonPress(Buttons.Start);
#endif
            }
        }


        /// <summary>
        /// Checks for a "pause the game" input action, from any player,
        /// on either keyboard or gamepad.
        /// </summary>
        public bool PauseGame
        {
            get
            {
#if WINDOWS
                return IsNewKeyPress(Keys.Escape) ||
                       IsNewButtonPress(Buttons.Start) || !screenManager.Game.IsActive;
                
#else
                return IsNewButtonPress(Buttons.Start) || !screenManager.Game.IsActive;
#endif
            }
        }


        /// <summary>
        /// Checks if the user pressed Back.
        /// </summary>
        public bool Back_Pressed
        {
            get
            {
                return IsNewButtonPress(Buttons.Back) || IsNewKeyPress(Keys.Back);
            }
        }

        /// <summary>
        /// Checks if the user pressed A.
        /// </summary>
        public bool A_Pressed
        {
            get
            {
                return IsNewButtonPress(Buttons.A) || IsNewKeyPress(Keys.Space);
            }
        }

        /// <summary>
        /// Checks if the user pressed B.
        /// </summary>
        public bool B_Pressed
        {
            get
            {
                return IsNewButtonPress(Buttons.B) || IsNewKeyPress(Keys.Enter);
            }
        }

        /// <summary>
        /// Checks if the user is holding Back.
        /// </summary>
        public bool Back_Held
        {
            get
            {
                return IsButtonHeld(Buttons.Back) || IsKeyHeld(Keys.Back);
            }
        }

        /// <summary>
        /// Checks if the user is holding A.
        /// </summary>
        public bool A_Held
        {
            get
            {
                return IsButtonHeld(Buttons.A) || IsKeyHeld(Keys.Space);
            }
        }


        /// <summary>
        /// Checks if the user is holding B.
        /// </summary>
        public bool B_Held
        {
            get
            {
                return IsButtonHeld(Buttons.B) || IsKeyHeld(Keys.Enter);
            }
        }


        /// <summary>
        /// Checks if the user pressed and released Back.
        /// </summary>
        public bool Back_PressRelease
        {
            get
            {
                return IsNewButtonPressRelease(Buttons.Back) || IsNewKeyPressRelease(Keys.Back);
            }
        }

        /// <summary>
        /// Checks if the user pressed and released A.
        /// </summary>
        public bool A_PressRelease
        {
            get
            {
                return IsNewButtonPressRelease(Buttons.A) || IsNewKeyPressRelease(Keys.Space);
            }
        }

        /// <summary>
        /// Checks if the user pressed and released B.
        /// </summary>
        public bool B_PressRelease
        {
            get
            {
                return IsNewButtonPressRelease(Buttons.B) || IsNewKeyPressRelease(Keys.Enter);
            }
        }


        /// <summary>
        /// Checks if the user pressed Up.
        /// </summary>
        public bool Up_Pressed
        {
            get
            {
                return (!screenManager.IsRotated && IsNewButtonPress(Buttons.DPadUp)) || (screenManager.IsRotated && IsNewButtonPress(Buttons.DPadRight)) || IsNewKeyPress(Keys.Up);
            }


        }

        /// <summary>
        /// Checks if the user pressed Down.
        /// </summary>
        public bool Down_Pressed
        {
            get
            {
                return (!screenManager.IsRotated && IsNewButtonPress(Buttons.DPadDown)) || (screenManager.IsRotated && IsNewButtonPress(Buttons.DPadLeft)) || IsNewKeyPress(Keys.Down);
            }
        }

        /// <summary>
        /// Checks if the user pressed Left.
        /// </summary>
        public bool Left_Pressed
        {
            get
            {
                return (!screenManager.IsRotated && IsNewButtonPress(Buttons.DPadLeft)) || (screenManager.IsRotated && IsNewButtonPress(Buttons.DPadUp)) || IsNewKeyPress(Keys.Left);
            }
        }

        /// <summary>
        /// Checks if the user pressed Right.
        /// </summary>
        public bool Right_Pressed
        {
            get
            {
                return (!screenManager.IsRotated && IsNewButtonPress(Buttons.DPadRight)) || (screenManager.IsRotated && IsNewButtonPress(Buttons.DPadDown)) || IsNewKeyPress(Keys.Right);
            }
        }


        /// <summary>
        /// Checks if the user pressed and released Up.
        /// </summary>
        public bool Up_PressRelease
        {
            get
            {
                return (!screenManager.IsRotated && IsNewButtonPressRelease(Buttons.DPadUp)) || (screenManager.IsRotated && IsNewButtonPressRelease(Buttons.DPadRight)) || IsNewKeyPressRelease(Keys.Up);
            }


        }

        /// <summary>
        /// Checks if the user pressed and released Down.
        /// </summary>
        public bool Down_PressRelease
        {
            get
            {
                return (!screenManager.IsRotated && IsNewButtonPressRelease(Buttons.DPadDown)) || (screenManager.IsRotated && IsNewButtonPressRelease(Buttons.DPadLeft)) || IsNewKeyPressRelease(Keys.Down);
            }
        }

        /// <summary>
        /// Checks if the user pressed and released Left.
        /// </summary>
        public bool Left_PressRelease
        {
            get
            {
                return (!screenManager.IsRotated && IsNewButtonPressRelease(Buttons.DPadLeft)) || (screenManager.IsRotated && IsNewButtonPressRelease(Buttons.DPadUp)) || IsNewKeyPressRelease(Keys.Left);
            }
        }

        /// <summary>
        /// Checks if the user pressed and released Right.
        /// </summary>
        public bool Right_PressRelease
        {
            get
            {
                return (!screenManager.IsRotated && IsNewButtonPressRelease(Buttons.DPadRight)) || (screenManager.IsRotated && IsNewButtonPressRelease(Buttons.DPadDown)) || IsNewKeyPressRelease(Keys.Right);
            }
        }




        /// <summary>
        /// Checks if the user is holding Up.
        /// </summary>
        public bool Up_Held
        {
            get
            {
                return (!screenManager.IsRotated && IsButtonHeld(Buttons.DPadUp)) || (screenManager.IsRotated && IsButtonHeld(Buttons.DPadRight)) || IsKeyHeld(Keys.Up);
            }


        }

        /// <summary>
        /// Checks if the user is holding Down.
        /// </summary>
        public bool Down_Held
        {
            get
            {
                return (!screenManager.IsRotated && IsButtonHeld(Buttons.DPadDown)) || (screenManager.IsRotated && IsButtonHeld(Buttons.DPadLeft)) || IsKeyHeld(Keys.Down);
            }
        }

        /// <summary>
        /// Checks if the user is holding Left.
        /// </summary>
        public bool Left_Held
        {
            get
            {
                return (!screenManager.IsRotated && IsButtonHeld(Buttons.DPadLeft)) || (screenManager.IsRotated && IsButtonHeld(Buttons.DPadUp)) || IsKeyHeld(Keys.Left); ;
            }
        }

        /// <summary>
        /// Checks if the user is holding Right.
        /// </summary>
        public bool Right_Held
        {
            get
            {
                return (!screenManager.IsRotated && IsButtonHeld(Buttons.DPadRight)) || (screenManager.IsRotated && IsButtonHeld(Buttons.DPadRight)) || IsKeyHeld(Keys.Right);
            }
        }



        #endregion

        #region Methods


        /// <summary>
        /// Reads the latest state of the keyboard and gamepad.
        /// </summary>
        public void Update()
        {
            for (int i = 0; i < MaxInputs; i++)
            {
#if WINDOWS
                LastKeyBoardStates[i] = CurrentKeyBoardStates[i];
                CurrentKeyBoardStates[i] = Keyboard.GetState((PlayerIndex)i);
#endif
                LastGamePadStates[i] = CurrentGamePadStates[i];
                CurrentGamePadStates[i] = GamePad.GetState((PlayerIndex)i);
            }
        }
        /// <summary>
        /// Helper for checking if a button was newly pressed during this update,
        /// by any player.
        /// </summary>
        public bool IsNewButtonPress(Buttons button)
        {
            for (int i = 0; i < MaxInputs; i++)
            {
                if (IsNewButtonPress(button, (PlayerIndex)i))
                {
                    presstimer += 1;
                    return true;

                }
            }

            return false;
        }

        /// <summary>
        /// Helper for checking if a button was newly pressed during this update,
        /// by any player.
        /// </summary>
        public bool IsNewKeyPress(Keys button)
        {
            for (int i = 0; i < MaxInputs; i++)
            {
                if (IsNewKeyPress(button, (PlayerIndex)i))
                {
                    presstimer += 1;
                    return true;

                }
            }

            return false;
        }

        /// <summary>
        /// Helper for checking if a button was newly pressed and released during this update,
        /// by any player. to clear the released input, after a certain time the function will set to false
        /// </summary>
        public bool IsNewButtonPressRelease(Buttons button)
        {
            for (int i = 0; i < MaxInputs; i++)
            {
                if ((IsNewButtonPressRelease(button, (PlayerIndex)i)) && presstimer < 5)
                {
                    presstimer += 1;
                    return true;
                }

            }

            return false;
        }

        /// <summary>
        /// Helper for checking if a button was newly pressed and released during this update,
        /// by any player. to clear the released input, after a certain time the function will set to false
        /// </summary>
        public bool IsNewKeyPressRelease(Keys button)
        {
            for (int i = 0; i < MaxInputs; i++)
            {
                if ((IsNewKeyPressRelease(button, (PlayerIndex)i)) && presstimer < 5)
                {
                    presstimer += 1;
                    return true;
                }

            }

            return false;
        }

        public bool IsButtonHeld(Buttons button)
        {
            for (int i = 0; i < MaxInputs; i++)
            {
                if (IsNewButtonHeld(button, (PlayerIndex)i))
                {
                    presstimer += 1;
                    if (presstimer > 40)
                    {
                        presstimer = 0;
                        return true;
                    }
                }
                if (IsNewButtonReleased(button, (PlayerIndex)i))
                {
                    if (presstimer > 40)
                    {
                        presstimer = 0;
                        return true;
                    }
                    presstimer = 0;
                }

            }

            return false;
        }
        public bool IsKeyHeld(Keys button)
        {
            for (int i = 0; i < MaxInputs; i++)
            {
                if (IsNewKeyHeld(button, (PlayerIndex)i))
                {
                    presstimer += 1;
                    if (presstimer > 40)
                    {
                        presstimer = 0;
                        return true;
                    }
                }
                if (IsNewKeyReleased(button, (PlayerIndex)i))
                {
                    if (presstimer > 40)
                    {
                        presstimer = 0;
                        return true;
                    }
                    presstimer = 0;
                }

            }

            return false;
        }

        /// <summary>
        /// Helper for checking if a button was previously pressed during this update,
        /// by any player.
        /// </summary>
        public bool IsPrevButtonPress(Buttons button)
        {
            for (int i = 0; i < MaxInputs; i++)
            {
                if (IsPrevButtonPress(button, (PlayerIndex)i))
                    return true;

            }

            return false;
        }
        /// <summary>
        /// Helper for checking if a button was previously pressed during this update,
        /// by any player.
        /// </summary>
        public bool IsPrevKeyPress(Keys button)
        {
            for (int i = 0; i < MaxInputs; i++)
            {
                if (IsPrevKeyPress(button, (PlayerIndex)i))
                    return true;

            }

            return false;
        }



        /// <summary>
        /// Helper for checking if a button was newly pressed during this update,
        /// by the specified player.
        /// </summary>
        public bool IsNewButtonPress(Buttons button, PlayerIndex playerIndex)
        {
            return (CurrentGamePadStates[(int)playerIndex].IsButtonDown(button) &&
                    LastGamePadStates[(int)playerIndex].IsButtonUp(button));
        }
        /// <summary>
        /// Helper for checking if a button was newly pressed during this update,
        /// by the specified player.
        /// </summary>
        public bool IsNewButtonTouch(Buttons button, PlayerIndex playerIndex)
        {
            return (CurrentGamePadStates[(int)playerIndex].IsButtonDown(button) &&
                    LastGamePadStates[(int)playerIndex].IsButtonUp(button));
        }

        /// <summary>
        /// Helper for checking if a button was newly pressed during this update,
        /// by the specified player.
        /// </summary>
        public bool IsNewButtonPressRelease(Buttons button, PlayerIndex playerIndex)
        {
            return (CurrentGamePadStates[(int)playerIndex].IsButtonUp(button) &&
                    LastGamePadStates[(int)playerIndex].IsButtonDown(button));
        }

        /// <summary>
        /// Helper for checking if a button was newly held during this update,
        /// by the specified player.
        /// </summary>
        public bool IsNewButtonHeld(Buttons button, PlayerIndex playerIndex)
        {
            return (CurrentGamePadStates[(int)playerIndex].IsButtonDown(button) &&
                    LastGamePadStates[(int)playerIndex].IsButtonDown(button));
        }

        /// <summary>
        /// Helper for checking if a button was newly released during this update,
        /// by the specified player.
        /// </summary>
        public bool IsNewButtonReleased(Buttons button, PlayerIndex playerIndex)
        {
            return (CurrentGamePadStates[(int)playerIndex].IsButtonUp(button) &&
                    LastGamePadStates[(int)playerIndex].IsButtonDown(button));
        }

        /// <summary>
        /// Helper for checking if a button was previously pressed during this update,
        /// by the specified player.
        /// </summary>
        public bool IsPrevButtonPress(Buttons button, PlayerIndex playerIndex)
        {
            return (CurrentGamePadStates[(int)playerIndex].IsButtonDown(button) &&
                    LastGamePadStates[(int)playerIndex].IsButtonDown(button));
        }


        //KEYBOARD

        /// <summary>
        /// Helper for checking if a button was newly pressed during this update,
        /// by the specified player.
        /// </summary>
        public bool IsNewKeyPress(Keys button, PlayerIndex playerIndex)
        {
            return (CurrentKeyBoardStates[(int)playerIndex].IsKeyDown(button) &&
                    LastKeyBoardStates[(int)playerIndex].IsKeyUp(button));
        }

        /// <summary>
        /// Helper for checking if a button was newly pressed during this update,
        /// by the specified player.
        /// </summary>
        public bool IsNewKeyPressRelease(Keys button, PlayerIndex playerIndex)
        {
            return (CurrentKeyBoardStates[(int)playerIndex].IsKeyUp(button) &&
                    LastKeyBoardStates[(int)playerIndex].IsKeyUp(button));
        }

        /// <summary>
        /// Helper for checking if a button was newly held during this update,
        /// by the specified player.
        /// </summary>
        public bool IsNewKeyHeld(Keys button, PlayerIndex playerIndex)
        {
            return (CurrentKeyBoardStates[(int)playerIndex].IsKeyDown(button) &&
                    LastKeyBoardStates[(int)playerIndex].IsKeyDown(button));
        }

        /// <summary>
        /// Helper for checking if a button was newly released during this update,
        /// by the specified player.
        /// </summary>
        public bool IsNewKeyReleased(Keys button, PlayerIndex playerIndex)
        {
            return (CurrentKeyBoardStates[(int)playerIndex].IsKeyUp(button) &&
                    LastKeyBoardStates[(int)playerIndex].IsKeyDown(button));
        }

        /// <summary>
        /// Helper for checking if a button was previously pressed during this update,
        /// by the specified player.
        /// </summary>
        public bool IsPrevKeyPress(Keys button, PlayerIndex playerIndex)
        {
            return (CurrentKeyBoardStates[(int)playerIndex].IsKeyDown(button) &&
                    LastKeyBoardStates[(int)playerIndex].IsKeyDown(button));
        }




        /// <summary>
        /// Checks for a "menu select" input action from the specified player.
        /// </summary>
        public bool IsMenuSelect(PlayerIndex playerIndex)
        {
#if WINDOWS
            return IsNewKeyPress(Keys.Space, playerIndex) ||
                IsNewKeyPress(Keys.Enter, playerIndex) ||
                   IsNewButtonPress(Buttons.A, playerIndex) ||
                   IsNewButtonPress(Buttons.Start, playerIndex);
#else
            return IsNewButtonPress(Buttons.A, playerIndex) ||
                    IsNewButtonPress(Buttons.Start, playerIndex);
#endif
        }


        /// <summary>
        /// Checks for a "menu select" input action from the specified player.
        /// </summary>
        public bool IsMenuLeft(PlayerIndex playerIndex)
        {
#if WINDOWS
            return IsNewKeyPress(Keys.Left, playerIndex) ||
                   IsNewButtonPress(Buttons.DPadLeft, playerIndex) ||
                   IsNewButtonPress(Buttons.LeftThumbstickLeft, playerIndex);
#else
            return IsNewButtonPress(Buttons.DPadLeft, playerIndex) ||
                    IsNewButtonPress(Buttons.LeftThumbstickLeft, playerIndex);
#endif
        }
        /// <summary>
        /// Checks for a "menu select" input action from the specified player.
        /// </summary>
        public bool IsMenuRight(PlayerIndex playerIndex)
        {
#if WINDOWS
            return IsNewKeyPress(Keys.Right, playerIndex) ||
                   IsNewButtonPress(Buttons.DPadRight, playerIndex) ||
                   IsNewButtonPress(Buttons.LeftThumbstickRight, playerIndex);
#else
            return IsNewButtonPress(Buttons.DPadRight, playerIndex) ||
                    IsNewButtonPress(Buttons.LeftThumbstickRight, playerIndex);
#endif
        }

        /// <summary>
        /// Checks for a "menu cancel" input action from the specified player.
        /// </summary>
        public bool IsMenuCancel(PlayerIndex playerIndex)
        {
#if WINDOWS
            return IsNewKeyPress(Keys.Escape, playerIndex) ||
                IsNewKeyPress(Keys.Back, playerIndex) ||
                IsNewButtonPress(Buttons.B, playerIndex) ||
                IsNewButtonPress(Buttons.Back, playerIndex);
#else
            return IsNewButtonPress(Buttons.B, playerIndex) ||
                   IsNewButtonPress(Buttons.Back, playerIndex);
#endif
        }



        #endregion
    }
}
