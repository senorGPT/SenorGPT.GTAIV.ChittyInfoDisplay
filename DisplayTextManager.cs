using System;
using IVSDKDotNet;
using static IVSDKDotNet.Native.Natives;

namespace SenorGPT.GTAIV.ChittyInfoDisplay
{
    /// <summary>
    /// Manages the display of time, date, and days passed information on the screen.
    /// </summary>
    public class DisplayTextManager
    {
        private string _displayMessageDate;
        private string _displayMessageTime;
        private readonly Config _config;
        private readonly InputHandler _inputHandler;

        /// <summary>
        /// Initializes a new instance of the DisplayTextManager class.
        /// </summary>
        /// <param name="config">The configuration object containing display settings.</param>
        /// <param name="inputHandler">The input handler for checking display states and flashing effects.</param>
        public DisplayTextManager(Config config, InputHandler inputHandler)
        {
            _config = config;
            _inputHandler = inputHandler;
        }

        /// <summary>
        /// Updates the internal date and time display strings.
        /// </summary>
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

                string temporaryMessage = $"{Utils.GetMonthName(month)} {day}{Utils.GetNumberSuffix(day)}, {Utils.GetDayName(currentDay + 1)}";

                if (_displayMessageDate != temporaryMessage)
                    _displayMessageDate = temporaryMessage;
            }
            catch (Exception ex)
            {
                // if date update fails, log error and keep previous message
                Utils.LogError("DisplayTextManager.UpdateDisplayTextDate", ex);
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

                string temporaryMessage = $"{Utils.GetPaddedNumber(hour)}:{Utils.GetPaddedNumber(minute)}{(!_config.TwentyFourHourMode ? timeSuffix : "")}";

                if (_displayMessageTime != temporaryMessage)
                    _displayMessageTime = temporaryMessage;
            }
            catch (Exception ex)
            {
                // if time update fails, log error and keep previous message
                Utils.LogError("DisplayTextManager.UpdateDisplayTextTime", ex);
            }
        }

        /// <summary>
        /// Handles the display of all text-based UI elements (time, date, days passed).
        /// Updates the display strings and renders them if enabled.
        /// </summary>
        public void HandleDisplayText()
        {
            // check if the values have changed, only update if they have
            UpdateDisplayText();

            // time display
            bool showTime = _config.ShouldDisplayTime || _inputHandler.ShouldDisplayAtDisabledOpacity(DisplayIndex.Time);
            if (showTime && _inputHandler.ShouldDisplayFlash(DisplayIndex.Time))
            {
                uint[] rgba = _inputHandler.ShouldDisplayAtDisabledOpacity(DisplayIndex.Time) ? Utils.CreateDisabledOpacityRgba() : null;
                Utils.DisplayTextString(
                    _config.DisplayTime.Scale,
                    _config.DisplayTime.X,
                    _config.DisplayTime.Y,
                    _displayMessageTime,
                    _config.DisplayTime.Font,
                    rgba
                );
            }

            // date display
            bool showDate = _config.ShouldDisplayDate || _inputHandler.ShouldDisplayAtDisabledOpacity(DisplayIndex.Date);
            if (showDate && _inputHandler.ShouldDisplayFlash(DisplayIndex.Date))
            {
                uint[] rgba = _inputHandler.ShouldDisplayAtDisabledOpacity(DisplayIndex.Date) ? Utils.CreateDisabledOpacityRgba() : null;
                Utils.DisplayTextString(
                    _config.DisplayDate.Scale,
                    _config.DisplayDate.X,
                    _config.DisplayDate.Y,
                    _displayMessageDate,
                    _config.DisplayDate.Font,
                    rgba
                );
            }

            // days passed display
            bool showDaysPassed = _config.ShouldDisplayDaysPassed || _inputHandler.ShouldDisplayAtDisabledOpacity(DisplayIndex.DaysPassed);
            if (showDaysPassed && _inputHandler.ShouldDisplayFlash(DisplayIndex.DaysPassed))
            {
                int days = GET_INT_STAT(DisplayConstants.StatIdDaysPassed);
                uint[] rgba = _inputHandler.ShouldDisplayAtDisabledOpacity(DisplayIndex.DaysPassed) ? Utils.CreateDisabledOpacityRgba() : null;
                Utils.DisplayTextString(
                    _config.DisplayPassed.Scale,
                    _config.DisplayPassed.X,
                    _config.DisplayPassed.Y,
                    $"Days Passed: {days}",
                    _config.DisplayPassed.Font,
                    rgba
                );
            }
        }
    }
}
