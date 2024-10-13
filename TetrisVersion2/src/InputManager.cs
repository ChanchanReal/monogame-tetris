using Microsoft.Xna.Framework.Input;

namespace TetrisVersion2.src
{
    internal static class InputManager
    {
        public static KeyboardState currentKeyboardState;
        public static KeyboardState oldKeyboardState;

        public static void update()
        {
            oldKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
        }

        public static bool input(Keys key) 
        {
            return currentKeyboardState.IsKeyDown(key);
        }

        public static bool TapInput(Keys key)
        {
            return currentKeyboardState.IsKeyDown(key) && oldKeyboardState != currentKeyboardState;
        }
    }
}
