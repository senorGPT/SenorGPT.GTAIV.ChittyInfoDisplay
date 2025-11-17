using System;
using System.Collections.Generic;
using System.Linq;
using IVSDKDotNet;
using static IVSDKDotNet.Native.Natives;

namespace SenorGPT.GTAIV.ChittyInfoDisplay
{
    /// <summary>
    /// Utility class providing helper methods for text display, date/time formatting, key handling, and error logging.
    /// </summary>
    public static class Utils
    {
        private static readonly string[] Months = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
        private static readonly string[] Days = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };

        private static readonly Dictionary<int, string> KeyNames = new Dictionary<int, string>
        {
            // Letters
            [VirtualKeyCodes.VK_A] = "A", [VirtualKeyCodes.VK_B] = "B", [VirtualKeyCodes.VK_C] = "C", [VirtualKeyCodes.VK_D] = "D", 
            [VirtualKeyCodes.VK_E] = "E", [VirtualKeyCodes.VK_F] = "F", [VirtualKeyCodes.VK_G] = "G",
            [VirtualKeyCodes.VK_H] = "H", [VirtualKeyCodes.VK_I] = "I", [VirtualKeyCodes.VK_J] = "J", [VirtualKeyCodes.VK_K] = "K", 
            [VirtualKeyCodes.VK_L] = "L", [VirtualKeyCodes.VK_M] = "M", [VirtualKeyCodes.VK_N] = "N",
            [VirtualKeyCodes.VK_O] = "O", [VirtualKeyCodes.VK_P] = "P", [VirtualKeyCodes.VK_Q] = "Q", [VirtualKeyCodes.VK_R] = "R", 
            [VirtualKeyCodes.VK_S] = "S", [VirtualKeyCodes.VK_T] = "T", [VirtualKeyCodes.VK_U] = "U",
            [VirtualKeyCodes.VK_V] = "V", [VirtualKeyCodes.VK_W] = "W", [VirtualKeyCodes.VK_X] = "X", 
            [VirtualKeyCodes.VK_Y] = "Y", [VirtualKeyCodes.VK_Z] = "Z",
            // Numbers
            [VirtualKeyCodes.VK_0] = "0", [VirtualKeyCodes.VK_1] = "1", [VirtualKeyCodes.VK_2] = "2", 
            [VirtualKeyCodes.VK_3] = "3", [VirtualKeyCodes.VK_4] = "4",
            [VirtualKeyCodes.VK_5] = "5", [VirtualKeyCodes.VK_6] = "6", [VirtualKeyCodes.VK_7] = "7", 
            [VirtualKeyCodes.VK_8] = "8", [VirtualKeyCodes.VK_9] = "9",
            // Arrow keys
            [VirtualKeyCodes.VK_LEFT] = "Left Arrow", [VirtualKeyCodes.VK_UP] = "Up Arrow", 
            [VirtualKeyCodes.VK_RIGHT] = "Right Arrow", [VirtualKeyCodes.VK_DOWN] = "Down Arrow",
            // Special keys
            [VirtualKeyCodes.VK_PLUS] = "+", [VirtualKeyCodes.VK_MINUS] = "-", 
            [VirtualKeyCodes.VK_COMMA] = ",", [VirtualKeyCodes.VK_PERIOD] = ".",
            [VirtualKeyCodes.VK_BACKSPACE] = "Backspace", [VirtualKeyCodes.VK_TAB] = "Tab", 
            [VirtualKeyCodes.VK_ENTER] = "Enter", [VirtualKeyCodes.VK_SPACE] = "Space",
            [VirtualKeyCodes.VK_ESCAPE] = "Escape", [VirtualKeyCodes.VK_INSERT] = "Insert", 
            [VirtualKeyCodes.VK_DELETE] = "Delete", [VirtualKeyCodes.VK_PAGE_UP] = "Page Up",
            [VirtualKeyCodes.VK_PAGE_DOWN] = "Page Down", [VirtualKeyCodes.VK_END] = "End", 
            [VirtualKeyCodes.VK_HOME] = "Home",
            [VirtualKeyCodes.VK_SHIFT] = "Shift", [VirtualKeyCodes.VK_LEFT_SHIFT] = "Left Shift", 
            [VirtualKeyCodes.VK_RIGHT_SHIFT] = "Right Shift",
            // Function keys
            [VirtualKeyCodes.VK_F1] = "F1", [VirtualKeyCodes.VK_F2] = "F2", [VirtualKeyCodes.VK_F3] = "F3", 
            [VirtualKeyCodes.VK_F4] = "F4", [VirtualKeyCodes.VK_F5] = "F5", [VirtualKeyCodes.VK_F6] = "F6",
            [VirtualKeyCodes.VK_F7] = "F7", [VirtualKeyCodes.VK_F8] = "F8", [VirtualKeyCodes.VK_F9] = "F9", 
            [VirtualKeyCodes.VK_F10] = "F10", [VirtualKeyCodes.VK_F11] = "F11", [VirtualKeyCodes.VK_F12] = "F12"
        };

        /// <summary>
        /// Displays a text string on the screen at the specified position with the given scale and font.
        /// </summary>
        /// <param name="scale">The scale of the text (0.0 to 1.0).</param>
        /// <param name="x">The X coordinate of the text position (0.0 to 1.0, where 0.0 is left edge).</param>
        /// <param name="y">The Y coordinate of the text position (0.0 to 1.0, where 0.0 is top edge).</param>
        /// <param name="text">The text string to display.</param>
        /// <param name="font">The font to use for the text. Defaults to DisplayConstants.DefaultFont.</param>
        /// <param name="rgba">Optional RGBA color array [R, G, B, A] for the text color. If null, uses default color.</param>
        public static void DisplayTextString(float scale, float x, float y, string text, uint font = DisplayConstants.DefaultFont, uint[] rgba = null)
        {
            SET_TEXT_FONT(font);
            SET_TEXT_SCALE(scale, scale);

            if (rgba != null)
                SET_TEXT_COLOUR(rgba[0], rgba[1], rgba[2], rgba[3]);

            DISPLAY_TEXT_WITH_LITERAL_STRING(x, y, "STRING", text);
        }

        /// <summary>
        /// Checks if a cutscene is currently active in the game.
        /// </summary>
        /// <returns>True if a cutscene is loaded and not finished, otherwise false.</returns>
        public static bool IsCutsceneActive()
        {
            if (HAS_CUTSCENE_LOADED())
                return !HAS_CUTSCENE_FINISHED();

            return false;
        }

        /// <summary>
        /// Gets the name of the month for the given month number (1-12).
        /// </summary>
        /// <param name="month">The month number (1 = January, 12 = December).</param>
        /// <returns>The name of the month. Returns "January" for invalid values (0 or out of range).</returns>
        public static string GetMonthName(uint month)
        {
            if (month <= 0) return Months[0];
            if (month > Months.Length) return Months[Months.Length - 1];

            return Months[month - 1];
        }

        /// <summary>
        /// Gets the name of the day of the week for the given day number (1-7).
        /// </summary>
        /// <param name="day">The day number (1 = Sunday, 7 = Saturday).</param>
        /// <returns>The name of the day. Returns "Sunday" for invalid values (0 or out of range).</returns>
        public static string GetDayName(uint day)
        {
            if (day <= 0) return Days[0];
            if (day > Days.Length) return Days[Days.Length - 1];

            return Days[day - 1];
        }

        /// <summary>
        /// Gets the ordinal suffix (st, nd, rd, th) for a given number.
        /// </summary>
        /// <param name="number">The number to get the suffix for.</param>
        /// <returns>The ordinal suffix string (st, nd, rd, or th).</returns>
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

        /// <summary>
        /// Formats a number as a string with a leading zero if it's less than 10.
        /// </summary>
        /// <param name="number">The number to format.</param>
        /// <returns>A string representation of the number with a leading zero if less than 10 (e.g., "09" for 9).</returns>
        public static string GetPaddedNumber(int number)
        {
            if (number < 10) return "0" + number.ToString();
            return number.ToString();
        }

        /// <summary>
        /// Parses a comma-separated string into an array of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array (must be a value type).</typeparam>
        /// <param name="value">The comma-separated string to parse (e.g., "255,200,0,255").</param>
        /// <returns>An array of type T containing the parsed values.</returns>
        public static T[] ParseArray<T>(string value) where T : struct
        {
            string[] parts = value.Split(',');
            T[] result = new T[parts.Length];
            for (int i = 0; i < parts.Length; i++)
                result[i] = (T)Convert.ChangeType(parts[i].Trim(), typeof(T));
            return result;
        }

        /// <summary>
        /// Gets the human-readable name of a virtual key code.
        /// </summary>
        /// <param name="virtualKeyCode">The Windows virtual key code.</param>
        /// <returns>The name of the key (e.g., "A", "Left Arrow", "F1"). Returns "Key {hex}" if the key is not recognized.</returns>
        public static string GetKeyName(int virtualKeyCode)
        {
            return KeyNames.TryGetValue(virtualKeyCode, out string name) ? name : "Key " + virtualKeyCode.ToString("X");
        }

        /// <summary>
        /// Converts a Windows virtual key code to a GTA IV key texture code for use with the ~k~ format in help text.
        /// </summary>
        /// <param name="virtualKeyCode">The Windows virtual key code.</param>
        /// <returns>The GTA IV key texture code. Returns the original virtual key code if no mapping exists.</returns>
        public static int GetGTAIVKeyTextureCode(int virtualKeyCode)
        {
            // GTA IV key texture codes mapping
            // letters: A-Z map to 1-26
            if (virtualKeyCode >= VirtualKeyCodes.VK_LETTERS_START && virtualKeyCode <= VirtualKeyCodes.VK_LETTERS_END)
                return virtualKeyCode - GTAIVKeyTextureCodes.LETTER_OFFSET;
            
            // numbers: 0-9 map to 27-36
            if (virtualKeyCode >= VirtualKeyCodes.VK_NUMBERS_START && virtualKeyCode <= VirtualKeyCodes.VK_NUMBERS_END)
                return virtualKeyCode - GTAIVKeyTextureCodes.NUMBER_OFFSET;
            
            // arrow keys
            if (virtualKeyCode == VirtualKeyCodes.VK_LEFT) return GTAIVKeyTextureCodes.TEXTURE_LEFT_ARROW;
            if (virtualKeyCode == VirtualKeyCodes.VK_UP) return GTAIVKeyTextureCodes.TEXTURE_UP_ARROW;
            if (virtualKeyCode == VirtualKeyCodes.VK_RIGHT) return GTAIVKeyTextureCodes.TEXTURE_RIGHT_ARROW;
            if (virtualKeyCode == VirtualKeyCodes.VK_DOWN) return GTAIVKeyTextureCodes.TEXTURE_DOWN_ARROW;
            
            // special keys
            if (virtualKeyCode == VirtualKeyCodes.VK_SPACE) return GTAIVKeyTextureCodes.TEXTURE_SPACE;
            if (virtualKeyCode == VirtualKeyCodes.VK_ENTER) return GTAIVKeyTextureCodes.TEXTURE_ENTER;
            if (virtualKeyCode == VirtualKeyCodes.VK_ESCAPE) return GTAIVKeyTextureCodes.TEXTURE_ESCAPE;
            if (virtualKeyCode == VirtualKeyCodes.VK_BACKSPACE) return GTAIVKeyTextureCodes.TEXTURE_BACKSPACE;
            if (virtualKeyCode == VirtualKeyCodes.VK_TAB) return GTAIVKeyTextureCodes.TEXTURE_TAB;
            if (virtualKeyCode == VirtualKeyCodes.VK_DELETE) return GTAIVKeyTextureCodes.TEXTURE_DELETE;
            if (virtualKeyCode == VirtualKeyCodes.VK_INSERT) return GTAIVKeyTextureCodes.TEXTURE_INSERT;
            if (virtualKeyCode == VirtualKeyCodes.VK_PAGE_UP) return GTAIVKeyTextureCodes.TEXTURE_PAGE_UP;
            if (virtualKeyCode == VirtualKeyCodes.VK_PAGE_DOWN) return GTAIVKeyTextureCodes.TEXTURE_PAGE_DOWN;
            if (virtualKeyCode == VirtualKeyCodes.VK_END) return GTAIVKeyTextureCodes.TEXTURE_END;
            if (virtualKeyCode == VirtualKeyCodes.VK_HOME) return GTAIVKeyTextureCodes.TEXTURE_HOME;
            
            // function keys: F1-F12 map to 52-63
            if (virtualKeyCode >= VirtualKeyCodes.VK_FUNCTION_START && virtualKeyCode <= VirtualKeyCodes.VK_FUNCTION_END)
                return virtualKeyCode - GTAIVKeyTextureCodes.FUNCTION_OFFSET;
            
            // plus/minus keys
            if (virtualKeyCode == VirtualKeyCodes.VK_PLUS) return GTAIVKeyTextureCodes.TEXTURE_PLUS;
            if (virtualKeyCode == VirtualKeyCodes.VK_MINUS) return GTAIVKeyTextureCodes.TEXTURE_MINUS;
            
            // default: return the virtual key code as-is (may not work for all keys)
            return virtualKeyCode;
        }

        /// <summary>
        /// Gets a formatted string with GTA IV key texture code for use in help text (e.g., "~k~1~" for the A key).
        /// </summary>
        /// <param name="virtualKeyCode">The Windows virtual key code.</param>
        /// <returns>A formatted string with the key texture code wrapped in ~k~ tags.</returns>
        public static string GetKeyTextureString(int virtualKeyCode)
        {
            int keyCode = GetGTAIVKeyTextureCode(virtualKeyCode);
            return $"~k~{keyCode}~";
        }

        /// <summary>
        /// Displays help text on the screen using GTA IV's help text system.
        /// </summary>
        /// <param name="helpText">The help text to display. Supports GXT formatting codes (e.g., ~y~ for yellow, ~n~ for newline).</param>
        /// <param name="gxtPlaceholder">The GXT placeholder string to use for text replacement. Defaults to "PLACEHOLDER_1".</param>
        public static void DisplayHelpText(string helpText, string gxtPlaceholder = "PLACEHOLDER_1")
        {
            IVText.TheIVText.ReplaceTextOfTextLabel(gxtPlaceholder, helpText);
            PRINT_HELP(gxtPlaceholder);
        }
        
        /// <summary>
        /// Logs an error to a file for debugging purposes.
        /// </summary>
        /// <param name="location">A string describing where the error occurred (e.g., "Main.OnTick", "InputHandler.HandleKeys").</param>
        /// <param name="ex">The exception that was thrown.</param>
        public static void LogError(string location, Exception ex)
        {
            // write to a log file for debugging
            try
            {
                string logPath = string.Format("{0}\\IVSDKDotNet\\scripts\\{1}.log", IVGame.GameStartupPath, Config.scriptName);
                string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Error in {location}: {ex.GetType().Name} - {ex.Message}\nStack Trace: {ex.StackTrace}\n\n";
                System.IO.File.AppendAllText(logPath, logMessage);
            }
            catch
            {
                // if logging fails, at least try to display in-game
                try { PRINT_HELP("ERROR_LOG_FAILED"); }
                catch { }
            }
        }

        /// <summary>
        /// Creates an RGBA color array for disabled display opacity (35%).
        /// </summary>
        /// <returns>An RGBA array [255, 255, 255, 89] representing white at 35% opacity.</returns>
        public static uint[] CreateDisabledOpacityRgba()
        {
            return new uint[] { DisplayConstants.FullOpacity, DisplayConstants.FullOpacity, DisplayConstants.FullOpacity, DisplayConstants.DisabledOpacity };
        }

        /// <summary>
        /// Validates that an RGBA array has at least 4 elements (R, G, B, A).
        /// </summary>
        /// <typeparam name="T">The type of elements in the array (must be a value type).</typeparam>
        /// <param name="rgba">The RGBA array to validate.</param>
        /// <returns>True if the array is not null and has at least 4 elements, otherwise false.</returns>
        public static bool IsValidRgbaArray<T>(T[] rgba) where T : struct
        {
            return rgba != null && rgba.Length >= 4;
        }

        /// <summary>
        /// Validates and ensures a font value is within the valid range of fonts.
        /// </summary>
        /// <param name="font">The font value to validate.</param>
        /// <returns>The font value if it's valid, otherwise returns DisplayConstants.DefaultFont.</returns>
        public static uint ValidateFont(uint font)
        {
            if (DisplayConstants.ValidFonts.Contains(font))
                return font;
            return DisplayConstants.DefaultFont;
        }
    }
}
