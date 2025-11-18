using System;
using CCL.GTAIV;
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
        private string _displayMessageSpeed = "0 MPH"; // default value for speedometer
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
        /// Handles the display of all text-based UI elements (time, date, days passed, speedometer).
        /// Updates the display strings and renders them if enabled.
        /// </summary>
        /// <param name="playerPed">The player ped for speedometer display.</param>
        public void HandleDisplayText(IVPed playerPed = null)
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

            // speedometer display 
            HandleSpeedometerDisplay(playerPed);
        }

        /// <summary>
        /// Updates and displays the speedometer if the player is in a vehicle.
        /// </summary>
        /// <param name="playerPed">The player ped to check for vehicle.</param>
        public void HandleSpeedometerDisplay(IVPed playerPed)
        {
            if (playerPed == null) return;
            
            bool showSpeedometer = _config.ShouldDisplaySpeedometer || _inputHandler.ShouldDisplayAtDisabledOpacity(DisplayIndex.Speedometer);
            if (!showSpeedometer || !_inputHandler.ShouldDisplayFlash(DisplayIndex.Speedometer))
                return;
            
            try
            {
                // default to 0 speed (especially useful in adjustment mode when not in vehicle)
                string unit = _config.SpeedometerUseMPH ? " MPH" : " KM/H";
                _displayMessageSpeed = "0" + unit + (_config.SpeedometerShowRPM ? " | 0 RPM" : "");

                // get the current vehicle of the player using IVSDKDotNet methods
                IVVehicle currentVehicle = IVVehicle.FromUIntPtr(playerPed.GetVehicle());
                
                // only skip vehicle check if in adjustment mode (to show speedometer even when not in vehicle)
                if (!_inputHandler.IsAdjustmentModeActive && (currentVehicle == null || !currentVehicle.IsDriver(playerPed) || !currentVehicle.VehicleFlags.EngineOn))
                    return;

                // if in a valid vehicle, update speed and RPM
                if (currentVehicle != null && currentVehicle.IsDriver(playerPed) && currentVehicle.VehicleFlags.EngineOn)
                {
                    float speedMS = currentVehicle.GetSpeed();
                    int speed = _config.SpeedometerUseMPH 
                        ? (int)Math.Round(speedMS * 2.23694f)    // convert m/s to MPH
                        : (int)Math.Round(speedMS * 3.6f);       // convert m/s to KM/H
                    string speedText = speed.ToString() + unit;

                    // add RPM if enabled (user will implement RPM retrieval)
                    if (_config.SpeedometerShowRPM)
                        speedText += $"  | {Math.Round(currentVehicle.EngineRPM * 10000)} RPM";

                    // only update if changed
                    if (_displayMessageSpeed != speedText)
                        _displayMessageSpeed = speedText;
                }

                // display the speedometer
                uint[] rgba = _inputHandler.ShouldDisplayAtDisabledOpacity(DisplayIndex.Speedometer) ? Utils.CreateDisabledOpacityRgba() : null;
                Utils.DisplayTextString(
                    _config.DisplaySpeedometer.Scale,
                    _config.DisplaySpeedometer.X,
                    _config.DisplaySpeedometer.Y,
                    _displayMessageSpeed,
                    _config.DisplaySpeedometer.Font,
                    rgba
                );
            }
            catch (Exception ex)
            {
                // if speedometer update fails, log error
                Utils.LogError("DisplayTextManager.HandleSpeedometerDisplay", ex);
            }
        }
    }
}
