using IVSDKDotNet;
using static IVSDKDotNet.Native.Natives;

namespace SenorGPT.GTAIV.ChittyInfoDisplay
{
    public class DisplayTextManager
    {
        private string _displayMessageDate;
        private string _displayMessageTime;
        private readonly Config _config;
        private readonly InputHandler _inputHandler;

        public DisplayTextManager(Config config, InputHandler inputHandler)
        {
            _config = config;
            _inputHandler = inputHandler;
        }

        public void UpdateDisplayText()
        {
            UpdateDisplayTextDate();
            UpdateDisplayTextTime();
        }

        private void UpdateDisplayTextDate()
        {
            try
            {
                GET_CURRENT_DATE(out uint day, out uint month);
                uint currentDay = GET_CURRENT_DAY_OF_WEEK();

                string temporaryMessage = Utils.GetMonthName(month) + " " + day.ToString() + 
                                         Utils.GetNumberSuffix(day) + ", " + 
                                         Utils.GetDayName(currentDay + 1);

                if (_displayMessageDate != temporaryMessage)
                    _displayMessageDate = temporaryMessage;
            }
            catch
            {
                // if date update fails, keep previous message
            }
        }

        private void UpdateDisplayTextTime()
        {
            try
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
            catch
            {
                // if time update fails, keep previous message
            }
        }

        public void HandleDisplayText()
        {
            // check if the values have changed, only update if they have
            UpdateDisplayText();

            // time display (index 0)
            bool showTime = _config.ShouldDisplayTime || _inputHandler.ShouldDisplayAtHalfOpacity(0);
            if (showTime && _inputHandler.ShouldDisplayFlash(0))
            {
                uint[] rgba = _inputHandler.ShouldDisplayAtHalfOpacity(0) ? new uint[] { 255, 255, 255, 89 } : null;
                Utils.DisplayTextString(
                    _config.DisplayTime.Scale,
                    _config.DisplayTime.X,
                    _config.DisplayTime.Y,
                    _displayMessageTime,
                    _config.DisplayTime.Font,
                    rgba
                );
            }

            // date display (index 1)
            bool showDate = _config.ShouldDisplayDate || _inputHandler.ShouldDisplayAtHalfOpacity(1);
            if (showDate && _inputHandler.ShouldDisplayFlash(1))
            {
                uint[] rgba = _inputHandler.ShouldDisplayAtHalfOpacity(1) ? new uint[] { 255, 255, 255, 89 } : null;
                Utils.DisplayTextString(
                    _config.DisplayDate.Scale,
                    _config.DisplayDate.X,
                    _config.DisplayDate.Y,
                    _displayMessageDate,
                    _config.DisplayDate.Font,
                    rgba
                );
            }

            // days passed display (index 2)
            bool showDaysPassed = _config.ShouldDisplayDaysPassed || _inputHandler.ShouldDisplayAtHalfOpacity(2);
            if (showDaysPassed && _inputHandler.ShouldDisplayFlash(2))
            {
                int days = GET_INT_STAT(260);
                uint[] rgba = _inputHandler.ShouldDisplayAtHalfOpacity(2) ? new uint[] { 255, 255, 255, 89 } : null;
                Utils.DisplayTextString(
                    _config.DisplayPassed.Scale,
                    _config.DisplayPassed.X,
                    _config.DisplayPassed.Y,
                    "Days Passed: " + days.ToString(),
                    _config.DisplayPassed.Font,
                    rgba
                );
            }
        }
    }
}
