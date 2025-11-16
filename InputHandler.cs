using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SenorGPT.GTAIV.ChittyInfoDisplay
{
    public class InputHandler
    {
        [DllImport("user32.dll")] 
        private static extern short GetAsyncKeyState(int vKey);

        private static float DisplayMessageX = 0.12f, DisplayMessageY = 0.05f;

        public static float GetDisplayMessageX() => DisplayMessageX;
        public static float GetDisplayMessageY() => DisplayMessageY;

        private readonly Dictionary<string, Dictionary<string, object>> Keys = new Dictionary<string, Dictionary<string, object>>()
        {
            ["Left"]    = new Dictionary<string, object>
            {
                ["Name"]        = "Left",
                ["Key"]         = 0x25,
                ["IsPressed"]   = false,
                ["Pressed"]     = (Action)(() =>
                {
                    DisplayMessageX -= 0.01f;
                })
            },
            ["Up"]      = new Dictionary<string, object>
            {
                ["Name"]        = "Up",
                ["Key"]         = 0x26,
                ["IsPressed"]   = false,
                ["Pressed"]     = (Action)(() =>
                {
                    DisplayMessageY -= 0.01f;
                })
            },
            ["Right"]   = new Dictionary<string, object>
            {
                ["Name"]        = "Right",
                ["Key"]         = 0x27,
                ["IsPressed"]   = false,
                ["Pressed"]     = (Action)(() =>
                {
                    DisplayMessageX += 0.01f;
                })
            },
            ["Down"]    = new Dictionary<string, object>
            {
                ["Name"]        = "Down",
                ["Key"]         = 0x28,
                ["IsPressed"]   = false,
                ["Pressed"]     = (Action)(() =>
                {
                    DisplayMessageY += 0.01f;
                })
            }
        };

        private void HandleKeyPress(int key, ref bool isKeyPressed, Action pressed, Action held = null, Action released = null)
        {
            bool isKeyDown = (GetAsyncKeyState(key) & 0x8000) != 0;
            bool isKeyJustPressed = isKeyDown && !isKeyPressed;
            bool isKeyReleased = !isKeyDown && isKeyPressed;

            if (isKeyJustPressed) pressed();                        // on press
            if (isKeyDown && held != null) held();                  // while held
            if (isKeyReleased && released != null) released();      // on-release

            isKeyPressed = isKeyDown;
        }

        public void HandleKeys()
        {
            foreach (var data in Keys.Values)
            {
                // can't pass a dictionary indexer (data["IsPressed"]) by ref. It must be a local/field.
                bool isPressed = Convert.ToBoolean(data["IsPressed"]);
                HandleKeyPress((int)data["Key"], ref isPressed, (Action)data["Pressed"]);
                data["IsPressed"] = isPressed; // write the (possibly updated) value back
            }
        }
    }
}
