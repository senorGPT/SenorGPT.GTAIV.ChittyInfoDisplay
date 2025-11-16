using IVSDKDotNet;
using static IVSDKDotNet.Native.Natives;

namespace SenorGPT.GTAIV.ChittyInfoDisplay
{
    public class DisplayTextManager
    {
        private string _displayMessageDate;
        private string _displayMessageTime;
        private readonly Config _config;

        public DisplayTextManager(Config config)
        {
            _config = config;
        }

        public void UpdateDisplayText()
        {
            UpdateDisplayTextDate();
            UpdateDisplayTextTime();
        }

        private void UpdateDisplayTextDate()
        {
            GET_CURRENT_DATE(out uint day, out uint month);
            uint currentDay = GET_CURRENT_DAY_OF_WEEK();

            string temporaryMessage = Utils.GetMonthName(month) + " " + day.ToString() + 
                                     Utils.GetNumberSuffix(day) + ", " + 
                                     Utils.GetDayName(currentDay + 1);

            if (_displayMessageDate != temporaryMessage)
                _displayMessageDate = temporaryMessage;
        }

        private void UpdateDisplayTextTime()
        {
            GET_TIME_OF_DAY(out int hour, out int minute);

            string timeSuffix = hour >= 12 ? "PM" : "AM";
            if (!_config.TwentyFourHourMode && hour > 12) 
                hour -= 12;

            string temporaryMessage = Utils.PadNumberWithZero(hour) + ":" + Utils.PadNumberWithZero(minute) + 
                                     (!_config.TwentyFourHourMode ? timeSuffix : "");

            if (_displayMessageTime != temporaryMessage)
                _displayMessageTime = temporaryMessage;
        }

        public void HandleDisplayText()
        {
            UpdateDisplayText();

            // TODO - flags for toggling  date/time/dayspassed display

            // date display
            SET_TEXT_SCALE(_config.DisplayDate.Scale, _config.DisplayDate.Scale);
            DISPLAY_TEXT_WITH_LITERAL_STRING(_config.DisplayDate.X, _config.DisplayDate.Y, "STRING", _displayMessageDate);

            // time display
            SET_TEXT_SCALE(_config.DisplayTime.Scale, _config.DisplayTime.Scale);
            DISPLAY_TEXT_WITH_LITERAL_STRING(_config.DisplayTime.X, _config.DisplayTime.Y, "STRING", _displayMessageTime);

            // days passed display
            int days = GET_INT_STAT(260);
            SET_TEXT_SCALE(_config.DisplayPassed.Scale, _config.DisplayPassed.Scale);
            DISPLAY_TEXT_WITH_LITERAL_STRING(_config.DisplayPassed.X, _config.DisplayPassed.Y, "STRING", "Days Passed: " + days.ToString());
        }
    }
}
