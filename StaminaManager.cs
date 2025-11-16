using System;
using static IVSDKDotNet.Native.Natives;

namespace SenorGPT.GTAIV.ChittyInfoDisplay
{
    public class StaminaManager
    {
        private bool _canPlayerSprint = true;
        private readonly Config _config;

        // public read only property for the canPlayerSprint variable
        public bool CanPlayerSprint => _canPlayerSprint;

        public StaminaManager(Config config)
        {
            _config = config;
        }

        public string CreateStaminaTextProgressBar(float stamina)
        {
            // clamp stamina between MinStamina and MaxStamina
            float clampedStamina = Math.Max(_config.ProgressBar.MinStamina, Math.Min(stamina, _config.ProgressBar.MaxStamina));

            // calculate percentage: normalize from [MinStamina, MaxStamina] to [0, 1]
            // formula: (value - min) / (max - min)
            float staminaRange = _config.ProgressBar.MaxStamina - _config.ProgressBar.MinStamina;
            float normalizedValue = (clampedStamina - _config.ProgressBar.MinStamina) / staminaRange;

            int filledChars = (int)Math.Round(normalizedValue * _config.ProgressBar.Width);
            string filled = new string(_config.ProgressBar.FilledChar, filledChars);
            string empty = new string(_config.ProgressBar.EmptyChar, _config.ProgressBar.Width - filledChars);

            return _config.ProgressBar.LeftBracket + filled + empty + _config.ProgressBar.RightBracket;
        }

        public int GetStaminaProgressBarValue(float stamina, bool asPercentage = false)
        {
            float clampedStamina = Math.Max(_config.ProgressBar.MinStamina, Math.Min(stamina, _config.ProgressBar.MaxStamina));
            if (!asPercentage) return (int)clampedStamina;

            float staminaRange = _config.ProgressBar.MaxStamina - _config.ProgressBar.MinStamina;
            float normalizedValue = (clampedStamina - _config.ProgressBar.MinStamina) / staminaRange;
            return (int)Math.Round(normalizedValue * 100);
        }

        public void UpdateSprintState(float currentStamina)
        {
            if (_canPlayerSprint && currentStamina <= _config.ProgressBar.MinStamina)
                _canPlayerSprint = false;
            if (!_canPlayerSprint && currentStamina >= 0)
                _canPlayerSprint = true;
        }

        public void ApplyStaminaColor()
        {
            if (!_canPlayerSprint)
            {
                var rgba = _config.StaminaBarTextCantSprintRGBA;
                SET_TEXT_COLOUR(rgba[0], rgba[1], rgba[2], rgba[3]);
            }
        }

        private void DisplayStaminaRectangleProgressBar(float currentStamina)
        {
            // stamina config data
            ProgressBarRectangleConfig rect = _config.StaminaBarRectangle;
            float normalizedValue = GetStaminaProgressBarValue(currentStamina, asPercentage: true) / 100.0f;
            
            // Draw border (larger rectangle)
            int[] borderRgba = rect.BorderColorRGBA;
            int borderR = borderRgba[0], borderG = borderRgba[1], borderB = borderRgba[2], borderA = borderRgba[3];
            float borderWidth = rect.Width + (rect.BorderThickness * 2);
            float borderHeight = rect.Height + (rect.BorderThickness * 2);
            DRAW_RECT(rect.X, rect.Y, borderWidth, borderHeight, borderR, borderG, borderB, borderA);
            
            // Draw empty background rectangle (full width)
            int[] emptyRgba = rect.EmptyColorRGBA;
            int emptyR = emptyRgba[0], emptyG = emptyRgba[1], emptyB = emptyRgba[2], emptyA = emptyRgba[3];
            DRAW_RECT(rect.X, rect.Y, rect.Width, rect.Height, emptyR, emptyG, emptyB, emptyA);
            
            // Draw filled portion based on stamina (partial width, left-aligned)
            if (normalizedValue > 0)
            {
                int[] filledRgba = _canPlayerSprint ? rect.FilledColorRGBA : rect.CantSprintColorRGBA;
                int filledR = filledRgba[0], filledG = filledRgba[1], filledB = filledRgba[2], filledA = filledRgba[3];
                float filledWidth = rect.Width * normalizedValue;
                float filledX = rect.X - (rect.Width / 2) + (filledWidth / 2);
                DRAW_RECT(filledX, rect.Y, filledWidth, rect.Height, filledR, filledG, filledB, filledA);
            }
        }

        public void HandleStaminaDisplay(float currentStamina)
        {
            // Update sprint state
            UpdateSprintState(currentStamina);
            
            // Apply color if sprint is disabled
            ApplyStaminaColor();
            
            // Display stamina progress bar
            if (_config.ShouldDisplayStaminaTextProgressBar)
            {
                string staminaBar = CreateStaminaTextProgressBar(currentStamina);
                Utils.DisplayTextString(
                    _config.DisplayStamina.Scale, 
                    _config.DisplayStamina.X, 
                    _config.DisplayStamina.Y, 
                    staminaBar
                );
            }
            
            // Display stamina value
            if (_config.ShouldDisplayStaminaValue)
            {
                string staminaValue = GetStaminaProgressBarValue(currentStamina, _config.DisplayStaminaValueAsPercentage).ToString();
                if (_config.DisplayStaminaValueAsPercentage) staminaValue += "%";
                Utils.DisplayTextString(
                    _config.DisplayStaminaValue.Scale, 
                    _config.DisplayStaminaValue.X, 
                    _config.DisplayStaminaValue.Y, 
                    staminaValue,
                    rgba: _canPlayerSprint ? null : _config.StaminaBarTextCantSprintRGBA
                );
            }

            // Display stamina simple format
            if (_config.ShouldDisplayStaminaSimple)
            {
                string staminaSimple = string.Format(_config.StaminaSimpleFormat, GetStaminaProgressBarValue(currentStamina, false));
                Utils.DisplayTextString(
                    _config.DisplayStaminaSimple.Scale, 
                    _config.DisplayStaminaSimple.X, 
                    _config.DisplayStaminaSimple.Y, 
                    staminaSimple
                );
            }

            // Display rectangle progress bar
            if (_config.ShouldDisplayStaminaRectangleProgressBar)
                DisplayStaminaRectangleProgressBar(currentStamina);
        }
    }
}
