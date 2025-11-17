namespace SenorGPT.GTAIV.ChittyInfoDisplay
{
    /// <summary>
    /// Constants for Windows Virtual Key Codes (VK_*)
    /// </summary>
    public static class VirtualKeyCodes
    {
        // Letters
        public const int VK_A = 0x41;
        public const int VK_B = 0x42;
        public const int VK_C = 0x43;
        public const int VK_D = 0x44;
        public const int VK_E = 0x45;
        public const int VK_F = 0x46;
        public const int VK_G = 0x47;
        public const int VK_H = 0x48;
        public const int VK_I = 0x49;
        public const int VK_J = 0x4A;
        public const int VK_K = 0x4B;
        public const int VK_L = 0x4C;
        public const int VK_M = 0x4D;
        public const int VK_N = 0x4E;
        public const int VK_O = 0x4F;
        public const int VK_P = 0x50;
        public const int VK_Q = 0x51;
        public const int VK_R = 0x52;
        public const int VK_S = 0x53;
        public const int VK_T = 0x54;
        public const int VK_U = 0x55;
        public const int VK_V = 0x56;
        public const int VK_W = 0x57;
        public const int VK_X = 0x58;
        public const int VK_Y = 0x59;
        public const int VK_Z = 0x5A;

        // Numbers
        public const int VK_0 = 0x30;
        public const int VK_1 = 0x31;
        public const int VK_2 = 0x32;
        public const int VK_3 = 0x33;
        public const int VK_4 = 0x34;
        public const int VK_5 = 0x35;
        public const int VK_6 = 0x36;
        public const int VK_7 = 0x37;
        public const int VK_8 = 0x38;
        public const int VK_9 = 0x39;

        // Arrow keys
        public const int VK_LEFT = 0x25;
        public const int VK_UP = 0x26;
        public const int VK_RIGHT = 0x27;
        public const int VK_DOWN = 0x28;

        // Special keys
        public const int VK_BACKSPACE = 0x08;
        public const int VK_TAB = 0x09;
        public const int VK_ENTER = 0x0D;
        public const int VK_SPACE = 0x20;
        public const int VK_ESCAPE = 0x1B;
        public const int VK_INSERT = 0x2D;
        public const int VK_DELETE = 0x2E;
        public const int VK_PAGE_UP = 0x21;
        public const int VK_PAGE_DOWN = 0x22;
        public const int VK_END = 0x23;
        public const int VK_HOME = 0x24;
        public const int VK_SHIFT = 0x10;
        public const int VK_LEFT_SHIFT = 0xA0;
        public const int VK_RIGHT_SHIFT = 0xA1;

        // Function keys
        public const int VK_F1 = 0x70;
        public const int VK_F2 = 0x71;
        public const int VK_F3 = 0x72;
        public const int VK_F4 = 0x73;
        public const int VK_F5 = 0x74;
        public const int VK_F6 = 0x75;
        public const int VK_F7 = 0x76;
        public const int VK_F8 = 0x77;
        public const int VK_F9 = 0x78;
        public const int VK_F10 = 0x79;
        public const int VK_F11 = 0x7A;
        public const int VK_F12 = 0x7B;

        // Numpad/Other keys
        public const int VK_PLUS = 0xBB;  // Plus/Equals (=/+)
        public const int VK_MINUS = 0xBD; // Minus/Underscore (-/_)
        public const int VK_COMMA = 0xBC;
        public const int VK_PERIOD = 0xBE;

        // Ranges
        public const int VK_LETTERS_START = 0x41; // A
        public const int VK_LETTERS_END = 0x5A;   // Z
        public const int VK_NUMBERS_START = 0x30; // 0
        public const int VK_NUMBERS_END = 0x39;   // 9
        public const int VK_FUNCTION_START = 0x70; // F1
        public const int VK_FUNCTION_END = 0x7B;   // F12
    }

    /// <summary>
    /// Constants for GTA IV key texture code mappings and offsets
    /// </summary>
    public static class GTAIVKeyTextureCodes
    {
        // Texture code offsets for conversion
        public const int LETTER_OFFSET = 0x40;        // A-Z: subtract this from VK to get texture code (A=1, B=2, etc.)
        public const int NUMBER_OFFSET = 0x09;         // 0-9: subtract this from VK to get texture code (0=27, 1=28, etc.)
        public const int FUNCTION_OFFSET = 0x1C;      // F1-F12: subtract this from VK to get texture code (F1=52, F2=53, etc.)

        // Base texture codes
        public const int TEXTURE_LETTER_START = 1;    // A maps to texture code 1
        public const int TEXTURE_NUMBER_START = 27;  // 0 maps to texture code 27
        public const int TEXTURE_FUNCTION_START = 52; // F1 maps to texture code 52

        // Arrow keys texture codes
        public const int TEXTURE_LEFT_ARROW = 37;
        public const int TEXTURE_UP_ARROW = 38;
        public const int TEXTURE_RIGHT_ARROW = 39;
        public const int TEXTURE_DOWN_ARROW = 40;

        // Special keys texture codes
        public const int TEXTURE_SPACE = 41;
        public const int TEXTURE_ENTER = 42;
        public const int TEXTURE_ESCAPE = 43;
        public const int TEXTURE_BACKSPACE = 44;
        public const int TEXTURE_TAB = 45;
        public const int TEXTURE_DELETE = 46;
        public const int TEXTURE_INSERT = 47;
        public const int TEXTURE_PAGE_UP = 48;
        public const int TEXTURE_PAGE_DOWN = 49;
        public const int TEXTURE_END = 50;
        public const int TEXTURE_HOME = 51;

        // Plus/Minus texture codes
        public const int TEXTURE_PLUS = 64;
        public const int TEXTURE_MINUS = 65;
    }
}

