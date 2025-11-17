using System;

namespace SenorGPT.GTAIV.ChittyInfoDisplay
{
    public static class DisplayConstants
    {
        // total number of displays (calculated from DisplayIndex enum, cached once at initialization)
        public static readonly int TotalDisplays = Enum.GetValues(typeof(DisplayIndex)).Length;
        
        // opacity values (35% = 89/255)
        public const uint DisabledOpacity = 89;
        public const uint FullOpacity = 255;
        
        // adjustment mode settings
        public const float MovementIncrement = 0.01f;
        public const int FlashFrameInterval = 30;
        
        // bounds
        public const float MinPosition = 0.0f;
        public const float MaxPosition = 1.0f;
        public const float MinScale = 0.0f;
        public const float MaxScale = 1.0f;
        public const float MinRectangleWidth = 0.01f;
        
        // valid fonts (font 2 and 3 are not valid)
        public static readonly uint[] ValidFonts = { 0, 1, 4, 5 };
        public const uint DefaultFont = 4;

        // GTA IV stat IDs
        public const int StatIdDaysPassed = 260;
    }
}

