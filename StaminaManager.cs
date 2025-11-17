using System;
using static IVSDKDotNet.Native.Natives;

namespace SenorGPT.GTAIV.ChittyInfoDisplay
{
    public class StaminaManager
    {
        private bool _canPlayerSprint = true;
        private readonly Config _config;
        private readonly InputHandler _inputHandler;

        // public read only property for the canPlayerSprint variable
        public bool CanPlayerSprint => _canPlayerSprint;

        public StaminaManager(Config config, InputHandler inputHandler)
        {
            _config = config;
            _inputHandler = inputHandler;
        }

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

        public int GetStaminaProgressBarValue(float stamina, bool asPercentage = false)
        {
            float clampedStamina = Math.Max(_config.ProgressBar.MinStamina, Math.Min(stamina, _config.ProgressBar.MaxStamina));
            if (!asPercentage) return (int)clampedStamina;

            float staminaRange = _config.ProgressBar.MaxStamina - _config.ProgressBar.MinStamina;
            if (staminaRange <= 0) return 0;
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
                if (rgba != null && rgba.Length >= 4)
                    SET_TEXT_COLOUR(rgba[0], rgba[1], rgba[2], rgba[3]);
            }
        }

        private void DisplayStaminaRectangleProgressBar(float currentStamina, bool halfOpacity = false)
        {
            // stamina config data
            ProgressBarRectangleConfig rect = _config.StaminaBarRectangle;
            float normalizedValue = GetStaminaProgressBarValue(currentStamina, asPercentage: true) / 100.0f;
            
            // apply 35% opacity if disabled in adjustment mode
            int opacityMultiplier = halfOpacity ? 89 : 255;
            
            // draw border (larger rectangle)
            int[] borderRgba = rect.BorderColorRGBA;
            if (borderRgba != null && borderRgba.Length >= 4)
            {
                int borderR = borderRgba[0], borderG = borderRgba[1], borderB = borderRgba[2];
                int borderA = borderRgba[3] * opacityMultiplier / 255;
                float borderWidth = rect.Width + (rect.BorderThickness * 2);
                float borderHeight = rect.Height + (rect.BorderThickness * 2);
                DRAW_RECT(rect.X, rect.Y, borderWidth, borderHeight, borderR, borderG, borderB, borderA);
            }
            
            // draw empty background rectangle (full width)
            int[] emptyRgba = rect.EmptyColorRGBA;
            if (emptyRgba != null && emptyRgba.Length >= 4)
            {
                int emptyR = emptyRgba[0], emptyG = emptyRgba[1], emptyB = emptyRgba[2];
                int emptyA = emptyRgba[3] * opacityMultiplier / 255;
                DRAW_RECT(rect.X, rect.Y, rect.Width, rect.Height, emptyR, emptyG, emptyB, emptyA);
            }
            
            // draw filled portion based on stamina (partial width, left-aligned)
            if (normalizedValue > 0)
            {
                int[] filledRgba = _canPlayerSprint ? rect.FilledColorRGBA : rect.CantSprintColorRGBA;
                if (filledRgba != null && filledRgba.Length >= 4)
                {
                    int filledR = filledRgba[0], filledG = filledRgba[1], filledB = filledRgba[2];
                    int filledA = filledRgba[3] * opacityMultiplier / 255;
                    float filledWidth = rect.Width * normalizedValue;
                    float filledX = rect.X - (rect.Width / 2) + (filledWidth / 2);
                    DRAW_RECT(filledX, rect.Y, filledWidth, rect.Height, filledR, filledG, filledB, filledA);
                }
            }
        }

        public void HandleStaminaDisplay(float currentStamina)
        {
            // update sprint state
            UpdateSprintState(currentStamina);
            
            // apply color if sprint is disabled
            ApplyStaminaColor();
            
            // display stamina progress bar (index 3)
            bool showTextBar = _config.ShouldDisplayStaminaTextProgressBar || _inputHandler.ShouldDisplayAtHalfOpacity(3);
            if (showTextBar && _inputHandler.ShouldDisplayFlash(3))
            {
                string staminaBar = CreateStaminaTextProgressBar(currentStamina);
                uint[] rgba = _inputHandler.ShouldDisplayAtHalfOpacity(3) ? new uint[] { 255, 255, 255, 89 } : null;
                Utils.DisplayTextString(
                    _config.DisplayStamina.Scale, 
                    _config.DisplayStamina.X, 
                    _config.DisplayStamina.Y, 
                    staminaBar,
                    _config.DisplayStamina.Font,
                    rgba
                );
            }
            
            // display stamina value (index 4)
            bool showValue = _config.ShouldDisplayStaminaValue || _inputHandler.ShouldDisplayAtHalfOpacity(4);
            if (showValue && _inputHandler.ShouldDisplayFlash(4))
            {
                string staminaValue = GetStaminaProgressBarValue(currentStamina, _config.DisplayStaminaValueAsPercentage).ToString();
                if (_config.DisplayStaminaValueAsPercentage) staminaValue += "%";
                uint[] rgba = _inputHandler.ShouldDisplayAtHalfOpacity(4) 
                    ? new uint[] { 255, 255, 255, 89 } 
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

            // display stamina simple format (index 5)
            bool showSimple = _config.ShouldDisplayStaminaSimple || _inputHandler.ShouldDisplayAtHalfOpacity(5);
            if (showSimple && _inputHandler.ShouldDisplayFlash(5))
            {
                string staminaSimple = string.Format(_config.StaminaSimpleFormat, GetStaminaProgressBarValue(currentStamina, false));
                uint[] rgba = _inputHandler.ShouldDisplayAtHalfOpacity(5) ? new uint[] { 255, 255, 255, 89 } : null;
                Utils.DisplayTextString(
                    _config.DisplayStaminaSimple.Scale, 
                    _config.DisplayStaminaSimple.X, 
                    _config.DisplayStaminaSimple.Y, 
                    staminaSimple,
                    _config.DisplayStaminaSimple.Font,
                    rgba
                );
            }

            // display rectangle progress bar (index 6)
            bool showRectangle = _config.ShouldDisplayStaminaRectangleProgressBar || _inputHandler.ShouldDisplayAtHalfOpacity(6);
            if (showRectangle && _inputHandler.ShouldDisplayFlash(6))
                DisplayStaminaRectangleProgressBar(currentStamina, _inputHandler.ShouldDisplayAtHalfOpacity(6));
        }
    }
}
