using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GameStateManagement
{
    public enum MouseButtons
    {
        Left,
        Middle,
        Right
    }

    public class InputManager
    {
        //keyboard states
        KeyboardState lastKeyboardState = new KeyboardState();
        public KeyboardState currentKeyboardState = new KeyboardState();
        MouseState lastMouseState = new MouseState();
        public MouseState currentMouseState = new MouseState();
        GamePadState lastGamePadState = new GamePadState();
        private GamePadState currentGamePadState = new GamePadState();

        //start activated
        public bool activated = true;

        public InputManager()
        {
        }

        public void Update(TimeSpan timeElapsed)
        {
            //get keyboard state
            lastKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            //get mouse state
            //Mouse.WindowHandle = windowHandle;
            lastMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            //get gamepad state
            lastGamePadState = currentGamePadState;
            currentGamePadState = GamePad.GetState(PlayerIndex.One);
        }

        #region Helper Functions

        #region Keys and Buttons

        /// <summary>
        /// Checks if the Key or the Button is pressed for the first time.
        /// </summary>
        /// <param name="key">The Key to check.</param>
        /// <param name="button">The Button to check.</param>
        /// <returns>True if the Key or Button is pressed for the first time.</returns>
        public bool IsPressed(Keys key, Buttons button)
        {
            return (IsKeyPressed(key) || IsButtonPressed(button));
        }

        /// <summary>
        /// Checks if the Key or the Buttons are pressed for the first time.
        /// </summary>
        /// <param name="key">The Key to check.</param>
        /// <param name="button1">The first Button to check.</param>
        /// <param name="button2">The second Button to check.</param>
        /// <returns>True if the Key or Buttons are pressed for the first time.</returns>
        public bool IsPressed(Keys key, Buttons button1, Buttons button2)
        {
            return (IsKeyPressed(key) || IsButtonPressed(button1) || IsButtonPressed(button2));
        }

        /// <summary>
        /// Checks if the Key or Button is continuously held.
        /// </summary>
        /// <param name="key">The Key to check.</param>
        /// <param name="button">The Button to check.</param>
        /// <returns>True if the Key or Button is continuously held.</returns>
        public bool IsHeld(Keys key, Buttons button)
        {
            return (IsKeyHeld(key) || IsButtonHeld(button));
        }

        /// <summary>
        /// Checks if the Key or the Buttons are continuously held.
        /// </summary>
        /// <param name="key">The Key to check.</param>
        /// <param name="button1">The first Button to check.</param>
        /// <param name="button2">The second Button to check.</param>
        /// <returns>True if the Key or Buttons are continuously held.</returns>
        public bool IsHeld(Keys key, Buttons button1, Buttons button2)
        {
            return (IsKeyHeld(key) || IsButtonHeld(button1) || IsButtonHeld(button2));
        }

        /// <summary>
        /// Checks if the Key or the Buttons are continuously held.
        /// </summary>
        /// <param name="key1">The first Key to check.</param>
        /// <param name="key2">The second Key to check.</param>
        /// <param name="button1">The first Button to check.</param>
        /// <param name="button2">The second Button to check.</param>
        /// <returns>True if the Key or Buttons are continuously held.</returns>
        public bool IsHeld(Keys key1, Keys key2, Buttons button1, Buttons button2)
        {
            return (IsKeyHeld(key1) || IsKeyHeld(key2) || IsButtonHeld(button1) || IsButtonHeld(button2));
        }

        /// <summary>
        /// Checks if the Key or the Button is just released.
        /// </summary>
        /// <param name="key">The Key to check.</param>
        /// <param name="button">The Button to check.</param>
        /// <returns>True if the Key or the Button is just released.</returns>
        public bool IsReleased(Keys key, Buttons button)
        {
            return (IsKeyReleased(key) || IsButtonReleased(button));
        }

        /// <summary>
        /// Checks if the Key or the Buttons are just released.
        /// </summary>
        /// <param name="key">The Key to check.</param>
        /// <param name="button1">The first Button to check.</param>
        /// <param name="button2">The second Button to check.</param>
        /// <returns>True if the Key or buttons are just released.</returns>
        public bool IsReleased(Keys key, Buttons button1, Buttons button2)
        {
            return (IsKeyReleased(key) || IsButtonReleased(button1) || IsButtonReleased(button2));
        }

        #endregion

        #region Buttons
        /// <summary>
        /// Checks to see whether the button is pressed for the first time.
        /// </summary>
        /// <param name="button">The button being pressed.</param>
        /// <returns>True if the button is pressed for the first time.</returns>
        public bool IsButtonPressed(Buttons button)
        {
            return (currentGamePadState.IsButtonDown(button) && lastGamePadState.IsButtonUp(button));
        }

        /// <summary>
        /// Checks to see whether the button is being held down.
        /// </summary>
        /// <param name="button">The button being held.</param>
        /// <returns>True if the button is being held down.</returns>
        public bool IsButtonHeld(Buttons button)
        {
            return (currentGamePadState.IsButtonDown(button) && lastGamePadState.IsButtonDown(button));
        }

        /// <summary>
        /// Checks to see whether the button has just been released.
        /// </summary>
        /// <param name="button">The button just released.</param>
        /// <returns>True if the button has just been released.</returns>
        public bool IsButtonReleased(Buttons button)
        {
            return (currentGamePadState.IsButtonUp(button) && lastGamePadState.IsButtonDown(button));
        }
        #endregion

        #region Keys
        /// <summary>
        /// Checks to see whether the key is pressed for the first time.
        /// </summary>
        /// <param name="key">The key being pressed.</param>
        /// <returns>True if the key is pressed for the first time.</returns>
        public bool IsKeyPressed(Keys key)
        {
            return (currentKeyboardState.IsKeyDown(key) && lastKeyboardState.IsKeyUp(key));
        }

        /// <summary>
        /// Checks to see whether the key is being held down.
        /// </summary>
        /// <param name="key">The key being held.</param>
        /// <returns>True if the key is being held down.</returns>
        public bool IsKeyHeld(Keys key)
        {
            return (currentKeyboardState.IsKeyDown(key) && lastKeyboardState.IsKeyDown(key));
        }

        /// <summary>
        /// Checks to see whether the key has just been released.
        /// </summary>
        /// <param name="key">The key just released.</param>
        /// <returns>True if the key has just been released.</returns>
        public bool IsKeyReleased(Keys key)
        {
            return (currentKeyboardState.IsKeyUp(key) && lastKeyboardState.IsKeyDown(key));
        }
        #endregion

        #region Mouse
        /// <summary>
        /// Checks to see whether the mouse button is pressed for the first time.
        /// </summary>
        /// <param name="mouseButton">The mouse button being pressed.</param>
        /// <returns>True if the mouse button is pressed for the first time.</returns>
        public bool IsMousePressed(MouseButtons mouseButton)
        {
            switch (mouseButton)
            {
                case MouseButtons.Left:
                    return (currentMouseState.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Released);
                case MouseButtons.Middle:
                    return (currentMouseState.MiddleButton == ButtonState.Pressed && lastMouseState.MiddleButton == ButtonState.Released);
                case MouseButtons.Right:
                    return (currentMouseState.RightButton == ButtonState.Pressed && lastMouseState.RightButton == ButtonState.Released);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Checks to see whether the mouse button is being held down.
        /// </summary>
        /// <param name="mouseButton">The mouse button being held.</param>
        /// <returns>True if the mouse button is being held down.</returns>
        public bool IsMouseHeld(MouseButtons mouseButton)
        {
            switch (mouseButton)
            {
                case MouseButtons.Left:
                    return (currentMouseState.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Pressed);
                case MouseButtons.Middle:
                    return (currentMouseState.MiddleButton == ButtonState.Pressed && lastMouseState.MiddleButton == ButtonState.Pressed);
                case MouseButtons.Right:
                    return (currentMouseState.RightButton == ButtonState.Pressed && lastMouseState.RightButton == ButtonState.Pressed);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Checks to see whether the mouse button has just been released.
        /// </summary>
        /// <param name="mouseButton">The mouse button just released.</param>
        /// <returns>True if the mouse button has just been released.</returns>
        public bool IsMouseReleased(MouseButtons mouseButton)
        {
            switch (mouseButton)
            {
                case MouseButtons.Left:
                    return (currentMouseState.LeftButton == ButtonState.Released && lastMouseState.LeftButton == ButtonState.Pressed);
                case MouseButtons.Middle:
                    return (currentMouseState.MiddleButton == ButtonState.Released && lastMouseState.MiddleButton == ButtonState.Pressed);
                case MouseButtons.Right:
                    return (currentMouseState.RightButton == ButtonState.Released && lastMouseState.RightButton == ButtonState.Pressed);
                default:
                    return false;
            }
        }
        #endregion

        #endregion
    }
}
