using System;
using IVSDKDotNet;
using static IVSDKDotNet.Native.Natives;

namespace SenorGPT.GTAIV.ChittyInfoDisplay
{
    /// <summary>
    /// Manages the display and calculation of player stamina information.
    /// </summary>
    public class StaminaManager
    {
        private bool _canPlayerSprint = true;
        private readonly Config _config;
        private readonly InputHandler _inputHandler;

        /// <summary>
        /// Gets a value indicating whether the player can currently sprint based on stamina level.
        /// </summary>
        public bool CanPlayerSprint => _canPlayerSprint;

        /// <summary>
        /// Initializes a new instance of the StaminaManager class.
        /// </summary>
        /// <param name="config">The configuration object containing display settings.</param>
        /// <param name="inputHandler">The input handler for checking display states and flashing effects.</param>
        public StaminaManager(Config config, InputHandler inputHandler)
        {
            _config = config;
            _inputHandler = inputHandler;
        }

        /// <summary>
        /// Creates a text-based progress bar representation of the current stamina level.
        /// </summary>
        /// <param name="stamina">The current stamina value.</param>
        /// <returns>A string representing the stamina progress bar (e.g., "[@@@@@-----]").</returns>
        public string CreateStaminaTextProgressBar(float stamina)
        {
            // clamp stamina between MinStamina and MaxStamina
            float clampedStamina = Math.Max(_config.ProgressBar.MinStamina, Math.Min(stamina, _config.ProgressBar.MaxStamina));

            // calculate percentage: normalize from [MinStamina, MaxStamina] to [0, 1] | formula: (value - min) / (max - min)
            float staminaRange = _config.ProgressBar.MaxStamina - _config.ProgressBar.MinStamina;
            if (staminaRange <= 0) return _config.ProgressBar.LeftBracket + new string(_config.ProgressBar.EmptyChar, _config.ProgressBar.Width) + _config.ProgressBar.RightBracket;
            float normalizedValue = (clampedStamina - _config.ProgressBar.MinStamina) / staminaRange;

            int filledChars = (int)Math.Round(normalizedValue * _config.ProgressBar.Width);
            string filled = new string(_config.ProgressBar.FilledChar, filledChars);
            string empty = new string(_config.ProgressBar.EmptyChar, _config.ProgressBar.Width - filledChars);

            return _config.ProgressBar.LeftBracket + filled + empty + _config.ProgressBar.RightBracket;
        }

        /// <summary>
        /// Gets the stamina value formatted as either a raw integer or a percentage.
        /// </summary>
        /// <param name="stamina">The current stamina value.</param>
        /// <param name="asPercentage">If true, returns the stamina as a percentage (0-100). If false, returns the raw clamped stamina value.</param>
        /// <returns>The stamina value as an integer. Returns 0 if the stamina range is invalid when asPercentage is true.</returns>
        public int GetStaminaProgressBarValue(float stamina, bool asPercentage = false)
        {
            float clampedStamina = Math.Max(_config.ProgressBar.MinStamina, Math.Min(stamina, _config.ProgressBar.MaxStamina));
            if (!asPercentage) return (int)clampedStamina;

            float staminaRange = _config.ProgressBar.MaxStamina - _config.ProgressBar.MinStamina;
            if (staminaRange <= 0) return 0;
            float normalizedValue = (clampedStamina - _config.ProgressBar.MinStamina) / staminaRange;
            return (int)Math.Round(normalizedValue * 100);
        }

        /// <summary>
        /// Updates the internal sprint state based on the current stamina level.
        /// </summary>
        /// <param name="currentStamina">The current stamina value.</param>
        public void UpdateSprintState(float currentStamina)
        {
            if (_canPlayerSprint && currentStamina <= _config.ProgressBar.MinStamina)
                _canPlayerSprint = false;
            if (!_canPlayerSprint && currentStamina >= 0)
                _canPlayerSprint = true;
        }

        /// <summary>
        /// Applies the appropriate text color based on whether the player can sprint.
        /// Sets the text color to the "can't sprint" color if the player cannot sprint.
        /// </summary>
        public void ApplyStaminaColor()
        {
            if (!_canPlayerSprint)
            {
                var rgba = _config.StaminaBarTextCantSprintRGBA;
                if (Utils.IsValidRgbaArray(rgba))
                    SET_TEXT_COLOUR(rgba[0], rgba[1], rgba[2], rgba[3]);
            }
        }

        private void DisplayStaminaRectangleProgressBar(float currentStamina, bool disabledOpacity = false)
        {
            // stamina config data
            ProgressBarRectangleConfig rect = _config.StaminaBarRectangle;
            float normalizedValue = GetStaminaProgressBarValue(currentStamina, asPercentage: true) / 100.0f;
            
            // apply 35% opacity if disabled in adjustment mode
            int opacityMultiplier = disabledOpacity ? (int)DisplayConstants.DisabledOpacity : (int)DisplayConstants.FullOpacity;
            
            // draw border (larger rectangle)
            int[] borderRgba = rect.BorderColorRGBA;
            if (Utils.IsValidRgbaArray(borderRgba))
            {
                int borderR = borderRgba[0], borderG = borderRgba[1], borderB = borderRgba[2];
                int borderA = CalculateOpacity(borderRgba[3], opacityMultiplier);
                float borderWidth = rect.Width + (rect.BorderThickness * 2);
                float borderHeight = rect.Height + (rect.BorderThickness * 2);
                DRAW_RECT(rect.X, rect.Y, borderWidth, borderHeight, borderR, borderG, borderB, borderA);
            }
            
            // draw empty background rectangle (full width)
            int[] emptyRgba = rect.EmptyColorRGBA;
            if (Utils.IsValidRgbaArray(emptyRgba))
            {
                int emptyR = emptyRgba[0], emptyG = emptyRgba[1], emptyB = emptyRgba[2];
                int emptyA = CalculateOpacity(emptyRgba[3], opacityMultiplier);
                DRAW_RECT(rect.X, rect.Y, rect.Width, rect.Height, emptyR, emptyG, emptyB, emptyA);
            }
            
            // draw filled portion based on stamina (partial width, left-aligned)
            if (normalizedValue > 0)
            {
                int[] filledRgba = _canPlayerSprint ? rect.FilledColorRGBA : rect.CantSprintColorRGBA;
                if (Utils.IsValidRgbaArray(filledRgba))
                {
                    int filledR = filledRgba[0], filledG = filledRgba[1], filledB = filledRgba[2];
                    int filledA = CalculateOpacity(filledRgba[3], opacityMultiplier);
                    float filledWidth = rect.Width * normalizedValue;
                    float filledX = rect.X - (rect.Width / 2) + (filledWidth / 2);
                    DRAW_RECT(filledX, rect.Y, filledWidth, rect.Height, filledR, filledG, filledB, filledA);
                }
            }
        }

        /// <summary>
        /// Calculates the opacity value by applying the opacity multiplier to the base alpha value.
        /// </summary>
        /// <param name="baseAlpha">The base alpha value (0-255).</param>
        /// <param name="opacityMultiplier">The opacity multiplier (0-255, typically DisabledOpacity or FullOpacity).</param>
        /// <returns>The calculated opacity value (0-255).</returns>
        private int CalculateOpacity(int baseAlpha, int opacityMultiplier)
        {
            return baseAlpha * opacityMultiplier / (int)DisplayConstants.FullOpacity;
        }

        /// <summary>
        /// Handles the display of all stamina-related UI elements based on the current stamina value.
        /// Updates sprint state, applies colors, and displays all enabled stamina displays.
        /// </summary>
        /// <param name="currentStamina">The current stamina value from the player.</param>
        /// <param name="playerPed">The player ped to check if in a vehicle.</param>
        public void HandleStaminaDisplay(float currentStamina, IVPed playerPed)
        {
            // only skip stamina display if in a vehicle AND not in adjustment mode
            if (!_inputHandler.IsAdjustmentModeActive && IVVehicle.FromUIntPtr(playerPed.GetVehicle()) != null)
                return;
            
            // update sprint state
            UpdateSprintState(currentStamina);
            
            // apply color if sprint is disabled
            ApplyStaminaColor();
            
            // display stamina progress bar
            bool textBarDisabledOpacity = _inputHandler.ShouldDisplayAtDisabledOpacity(DisplayIndex.StaminaTextBar);
            bool textBarShouldFlash = _inputHandler.ShouldDisplayFlash(DisplayIndex.StaminaTextBar);
            bool showTextBar = _config.ShouldDisplayStaminaTextProgressBar || textBarDisabledOpacity;
            if (showTextBar && textBarShouldFlash)
            {
                string staminaBar = CreateStaminaTextProgressBar(currentStamina);
                uint[] rgba = textBarDisabledOpacity ? Utils.CreateDisabledOpacityRgba() : null;
                Utils.DisplayTextString(
                    _config.DisplayStamina.Scale, 
                    _config.DisplayStamina.X, 
                    _config.DisplayStamina.Y, 
                    staminaBar,
                    _config.DisplayStamina.Font,
                    rgba
                );
            }
            
            // display stamina value
            bool valueDisabledOpacity = _inputHandler.ShouldDisplayAtDisabledOpacity(DisplayIndex.StaminaValue);
            bool valueShouldFlash = _inputHandler.ShouldDisplayFlash(DisplayIndex.StaminaValue);
            bool showValue = _config.ShouldDisplayStaminaValue || valueDisabledOpacity;
            if (showValue && valueShouldFlash)
            {
                string staminaValue = GetStaminaProgressBarValue(currentStamina, _config.DisplayStaminaValueAsPercentage).ToString();
                if (_config.DisplayStaminaValueAsPercentage) staminaValue += "%";
                uint[] rgba = valueDisabledOpacity 
                    ? Utils.CreateDisabledOpacityRgba() 
                    : (_canPlayerSprint ? null : _config.StaminaBarTextCantSprintRGBA);
                Utils.DisplayTextString(
                    _config.DisplayStaminaValue.Scale, 
                    _config.DisplayStaminaValue.X, 
                    _config.DisplayStaminaValue.Y, 
                    staminaValue,
                    _config.DisplayStaminaValue.Font,
                    rgba
                );
            }

            // display stamina simple format
            bool simpleDisabledOpacity = _inputHandler.ShouldDisplayAtDisabledOpacity(DisplayIndex.StaminaSimple);
            bool simpleShouldFlash = _inputHandler.ShouldDisplayFlash(DisplayIndex.StaminaSimple);
            bool showSimple = _config.ShouldDisplayStaminaSimple || simpleDisabledOpacity;
            if (showSimple && simpleShouldFlash)
            {
                string staminaSimple = string.Format(_config.StaminaSimpleFormat, GetStaminaProgressBarValue(currentStamina, false));
                uint[] rgba = simpleDisabledOpacity 
                    ? Utils.CreateDisabledOpacityRgba() 
                    : (_canPlayerSprint ? null : _config.StaminaBarTextCantSprintRGBA);
                Utils.DisplayTextString(
                    _config.DisplayStaminaSimple.Scale, 
                    _config.DisplayStaminaSimple.X, 
                    _config.DisplayStaminaSimple.Y, 
                    staminaSimple,
                    _config.DisplayStaminaSimple.Font,
                    rgba
                );
            }

            // display rectangle progress bar
            bool rectangleDisabledOpacity = _inputHandler.ShouldDisplayAtDisabledOpacity(DisplayIndex.StaminaRectangle);
            bool rectangleShouldFlash = _inputHandler.ShouldDisplayFlash(DisplayIndex.StaminaRectangle);
            bool showRectangle = _config.ShouldDisplayStaminaRectangleProgressBar || rectangleDisabledOpacity;
            if (showRectangle && rectangleShouldFlash)
                DisplayStaminaRectangleProgressBar(currentStamina, rectangleDisabledOpacity);
        }
    }
}
