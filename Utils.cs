using System;
using static IVSDKDotNet.Native.Natives;

namespace SenorGPT.GTAIV.ChittyInfoDisplay
{
    public static class Utils
    {
        private static readonly string[] Months = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
        private static readonly string[] Days = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };

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
    }
}
