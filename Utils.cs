using System;
using System.Collections.Generic;
using IVSDKDotNet;
using static IVSDKDotNet.Native.Natives;

namespace SenorGPT.GTAIV.ChittyInfoDisplay
{
    public static class Utils
    {
        private static readonly string[] Months = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
        private static readonly string[] Days = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };

        private static readonly Dictionary<int, string> KeyNames = new Dictionary<int, string>
        {
            // Letters
            [0x41] = "A", [0x42] = "B", [0x43] = "C", [0x44] = "D", [0x45] = "E", [0x46] = "F", [0x47] = "G",
            [0x48] = "H", [0x49] = "I", [0x4A] = "J", [0x4B] = "K", [0x4C] = "L", [0x4D] = "M", [0x4E] = "N",
            [0x4F] = "O", [0x50] = "P", [0x51] = "Q", [0x52] = "R", [0x53] = "S", [0x54] = "T", [0x55] = "U",
            [0x56] = "V", [0x57] = "W", [0x58] = "X", [0x59] = "Y", [0x5A] = "Z",
            // Numbers
            [0x30] = "0", [0x31] = "1", [0x32] = "2", [0x33] = "3", [0x34] = "4",
            [0x35] = "5", [0x36] = "6", [0x37] = "7", [0x38] = "8", [0x39] = "9",
            // Arrow keys
            [0x25] = "Left Arrow", [0x26] = "Up Arrow", [0x27] = "Right Arrow", [0x28] = "Down Arrow",
            // Special keys
            [0xBB] = "+", [0xBD] = "-", [0xBC] = ",", [0xBE] = ".",
            [0x08] = "Backspace", [0x09] = "Tab", [0x0D] = "Enter", [0x20] = "Space",
            [0x1B] = "Escape", [0x2D] = "Insert", [0x2E] = "Delete", [0x21] = "Page Up",
            [0x22] = "Page Down", [0x23] = "End", [0x24] = "Home",
            [0x10] = "Shift", [0xA0] = "Left Shift", [0xA1] = "Right Shift",
            // Function keys
            [0x70] = "F1", [0x71] = "F2", [0x72] = "F3", [0x73] = "F4", [0x74] = "F5", [0x75] = "F6",
            [0x76] = "F7", [0x77] = "F8", [0x78] = "F9", [0x79] = "F10", [0x7A] = "F11", [0x7B] = "F12"
        };

        public static void DisplayTextString(float scale, float x, float y, string text, uint font = 4, uint[] rgba = null)
        {
            SET_TEXT_FONT(font);
            SET_TEXT_SCALE(scale, scale);

            if (rgba != null)
                SET_TEXT_COLOUR(rgba[0], rgba[1], rgba[2], rgba[3]);

            DISPLAY_TEXT_WITH_LITERAL_STRING(x, y, "STRING", text);
        }

        public static bool IsCutsceneActive()
        {
            if (HAS_CUTSCENE_LOADED())
                return !HAS_CUTSCENE_FINISHED();

            return false;
        }

        public static string GetMonthName(uint month)
        {
            if (month <= 0) return Months[0];
            if (month > Months.Length) return Months[Months.Length - 1];

            return Months[month - 1];
        }

        public static string GetDayName(uint day)
        {
            if (day <= 0) return Days[0];
            if (day > Days.Length) return Days[Days.Length - 1];

            return Days[day - 1];
        }

        public static string GetNumberSuffix(uint number)
        {
            var lastTwo = number % 100;
            if (lastTwo >= 11 && lastTwo <= 13) return "th";

            switch (number % 10)
            {
                case 1: return "st";
                case 2: return "nd";
                case 3: return "rd";
                default: return "th";
            }
        }

        public static string PadNumberWithZero(int number)
        {
            if (number < 10) return "0" + number.ToString();
            return number.ToString();
        }

        public static T[] ParseArray<T>(string value) where T : struct
        {
            string[] parts = value.Split(',');
            T[] result = new T[parts.Length];
            for (int i = 0; i < parts.Length; i++)
                result[i] = (T)Convert.ChangeType(parts[i].Trim(), typeof(T));
            return result;
        }

        public static string GetKeyName(int virtualKeyCode)
        {
            return KeyNames.TryGetValue(virtualKeyCode, out string name) ? name : "Key " + virtualKeyCode.ToString("X");
        }

        // converts virtual key code to GTA IV key texture code for use with ~k~ format
        public static int GetGTAIVKeyTextureCode(int virtualKeyCode)
        {
            // GTA IV key texture codes mapping
            // letters: A-Z map to 1-26
            if (virtualKeyCode >= 0x41 && virtualKeyCode <= 0x5A) // A-Z
                return virtualKeyCode - 0x40; // A=1, B=2, etc.
            
            // numbers: 0-9 map to 27-36
            if (virtualKeyCode >= 0x30 && virtualKeyCode <= 0x39) // 0-9
                return virtualKeyCode - 0x09; // 0=27, 1=28, etc.
            
            // arrow keys
            if (virtualKeyCode == 0x25) return 37; // Left Arrow
            if (virtualKeyCode == 0x26) return 38; // Up Arrow
            if (virtualKeyCode == 0x27) return 39; // Right Arrow
            if (virtualKeyCode == 0x28) return 40; // Down Arrow
            
            // special keys
            if (virtualKeyCode == 0x20) return 41; // Space
            if (virtualKeyCode == 0x0D) return 42; // Enter
            if (virtualKeyCode == 0x1B) return 43; // Escape
            if (virtualKeyCode == 0x08) return 44; // Backspace
            if (virtualKeyCode == 0x09) return 45; // Tab
            if (virtualKeyCode == 0x2E) return 46; // Delete
            if (virtualKeyCode == 0x2D) return 47; // Insert
            if (virtualKeyCode == 0x21) return 48; // Page Up
            if (virtualKeyCode == 0x22) return 49; // Page Down
            if (virtualKeyCode == 0x23) return 50; // End
            if (virtualKeyCode == 0x24) return 51; // Home
            
            // function keys: F1-F12 map to 52-63
            if (virtualKeyCode >= 0x70 && virtualKeyCode <= 0x7B) // F1-F12
                return virtualKeyCode - 0x1C; // F1=52, F2=53, etc.
            
            // plus/minus keys
            if (virtualKeyCode == 0xBB) return 64; // Plus/Equals
            if (virtualKeyCode == 0xBD) return 65; // Minus/Underscore
            
            // default: return the virtual key code as-is (may not work for all keys)
            return virtualKeyCode;
        }

        // returns a formatted string with key texture code for use in help text
        public static string GetKeyTextureString(int virtualKeyCode)
        {
            int keyCode = GetGTAIVKeyTextureCode(virtualKeyCode);
            return $"~k~{keyCode}~";
        }

        public static void DisplayHelpText(string helpText)
        {
            string targetReplacementGXT = "PLACEHOLDER_1";
            IVText.TheIVText.ReplaceTextOfTextLabel(targetReplacementGXT, helpText);
            PRINT_HELP(targetReplacementGXT);
        }
    }
}
