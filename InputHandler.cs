using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using IVSDKDotNet;
using static IVSDKDotNet.Native.Natives;
using System.Linq;

namespace SenorGPT.GTAIV.ChittyInfoDisplay
{
    /// <summary>
    /// Handles keyboard input and manages adjustment mode for configuring display positions and settings.
    /// </summary>
    public class InputHandler
    {
        [DllImport("user32.dll")] 
        private static extern short GetAsyncKeyState(int vKey);

        private bool _adjustmentMode = false;
        private DisplayIndex _selectedDisplayIndex = DisplayIndex.Time;
        private readonly Config _config;
        private int _flashCounter = 0;
        private const int ShiftKeyVK = 0x10;

        // mapping dictionaries to replace switch statements
        private readonly Dictionary<DisplayIndex, Func<DisplayTextConfig>> _displayConfigMap;
        private readonly Dictionary<DisplayIndex, Func<bool>> _displayEnabledMap;
        private readonly Dictionary<DisplayIndex, Action> _displayToggleMap;
        
        // cached help text (built once during initialization)
        private readonly string _adjustmentHelpText;

        /// <summary>
        /// Gets a value indicating whether adjustment mode is currently active.
        /// </summary>
        public bool IsAdjustmentModeActive => _adjustmentMode;
        
        /// <summary>
        /// Gets the currently selected display index in adjustment mode.
        /// </summary>
        public DisplayIndex SelectedDisplayIndex => _selectedDisplayIndex;
        
        /// <summary>
        /// Determines if a display should be visible based on the flashing effect in adjustment mode.
        /// Returns false only when in adjustment mode AND the display is selected AND we're in the "off" phase of the flash cycle.
        /// </summary>
        /// <param name="displayIndex">The display index to check.</param>
        /// <returns>True if the display should be visible, false if it should be hidden (flashing off phase).</returns>
        public bool ShouldDisplayFlash(DisplayIndex displayIndex)
        {
            // if not in adjustment mode, always visible
            if (!_adjustmentMode)
                return true;
            
            // if in adjustment mode but this display is not selected, always visible
            if (_selectedDisplayIndex != displayIndex)
                return true;
            
            // if in adjustment mode AND this display is selected, flash it
            
            // only increment counter when needed (in adjustment mode with selected display)
            _flashCounter++;
            
            // prevent overflow by resetting counter when it reaches one complete flash cycle
            // flash cycle = FlashFrameInterval * 2 (on phase + off phase)
            const int flashCycleLength = DisplayConstants.FlashFrameInterval * 2;
            if (_flashCounter >= flashCycleLength)
                _flashCounter = 0;
            
            return (_flashCounter / DisplayConstants.FlashFrameInterval) % 2 == 0;
        }

        /// <summary>
        /// Initializes a new instance of the InputHandler class.
        /// </summary>
        /// <param name="config">The configuration object containing key bindings and display settings.</param>
        public InputHandler(Config config)
        {
            _config = config;
            
            // initialize mapping dictionaries to replace switch statements
            // map DisplayIndex to DisplayTextConfig getter
            _displayConfigMap = new Dictionary<DisplayIndex, Func<DisplayTextConfig>>
            {
                [DisplayIndex.Time] = () => _config.DisplayTime,
                [DisplayIndex.Date] = () => _config.DisplayDate,
                [DisplayIndex.DaysPassed] = () => _config.DisplayPassed,
                [DisplayIndex.StaminaTextBar] = () => _config.DisplayStamina,
                [DisplayIndex.StaminaValue] = () => _config.DisplayStaminaValue,
                [DisplayIndex.StaminaSimple] = () => _config.DisplayStaminaSimple
            };

            // map DisplayIndex to enabled state getter
            _displayEnabledMap = new Dictionary<DisplayIndex, Func<bool>>
            {
                [DisplayIndex.Time] = () => _config.ShouldDisplayTime,
                [DisplayIndex.Date] = () => _config.ShouldDisplayDate,
                [DisplayIndex.DaysPassed] = () => _config.ShouldDisplayDaysPassed,
                [DisplayIndex.StaminaTextBar] = () => _config.ShouldDisplayStaminaTextProgressBar,
                [DisplayIndex.StaminaValue] = () => _config.ShouldDisplayStaminaValue,
                [DisplayIndex.StaminaSimple] = () => _config.ShouldDisplayStaminaSimple,
                [DisplayIndex.StaminaRectangle] = () => _config.ShouldDisplayStaminaRectangleProgressBar
            };

            // map DisplayIndex to toggle action
            _displayToggleMap = new Dictionary<DisplayIndex, Action>
            {
                [DisplayIndex.Time] = () => _config.ShouldDisplayTime = !_config.ShouldDisplayTime,
                [DisplayIndex.Date] = () => _config.ShouldDisplayDate = !_config.ShouldDisplayDate,
                [DisplayIndex.DaysPassed] = () => _config.ShouldDisplayDaysPassed = !_config.ShouldDisplayDaysPassed,
                [DisplayIndex.StaminaTextBar] = () => _config.ShouldDisplayStaminaTextProgressBar = !_config.ShouldDisplayStaminaTextProgressBar,
                [DisplayIndex.StaminaValue] = () => _config.ShouldDisplayStaminaValue = !_config.ShouldDisplayStaminaValue,
                [DisplayIndex.StaminaSimple] = () => _config.ShouldDisplayStaminaSimple = !_config.ShouldDisplayStaminaSimple,
                [DisplayIndex.StaminaRectangle] = () => _config.ShouldDisplayStaminaRectangleProgressBar = !_config.ShouldDisplayStaminaRectangleProgressBar
            };
            
            InitializeKeys();
            
            // build and cache the adjustment help text once
            _adjustmentHelpText = BuildAdjustmentHelpText();
        }
        
        private string BuildAdjustmentHelpText()
        {
            string displaySelectorKeyTexture = Utils.GetKeyName(_config.DisplaySelectorKey);
            string leftKeyTexture = Utils.GetKeyName(_config.LeftKey);
            string rightKeyTexture = Utils.GetKeyName(_config.RightKey);
            string upKeyTexture = Utils.GetKeyName(_config.UpKey);
            string downKeyTexture = Utils.GetKeyName(_config.DownKey);
            string scaleIncreaseKeyTexture = Utils.GetKeyName(_config.ScaleIncreaseKey);
            string scaleDecreaseKeyTexture = Utils.GetKeyName(_config.ScaleDecreaseKey);
            string fontSwitchKeyTexture = Utils.GetKeyName(_config.FontSwitchKey);
            string displayToggleKeyTexture = Utils.GetKeyName(_config.DisplayToggleKey);
            string toggleKeyTexture = Utils.GetKeyName(_config.AdjustmentModeToggleKey);
            string shiftKeyTexture = Utils.GetKeyName(ShiftKeyVK);

            // build arrow keys string
            string arrowKeysText = $"{upKeyTexture}/{downKeyTexture}/{leftKeyTexture}/{rightKeyTexture}";

            string scaleKeysText = $"{scaleIncreaseKeyTexture}/{scaleDecreaseKeyTexture}";

            return $"~y~Adjustment Mode Active~n~~b~{displaySelectorKeyTexture}~w~ - Cycle through displays~n~~b~{arrowKeysText}~w~ - Move position~n~~b~{scaleKeysText}~w~ - Adjust scale (or width for rectangle)~n~~b~{fontSwitchKeyTexture}~w~ - Switch font~n~~b~{displayToggleKeyTexture}~w~ - Toggle display on/off~n~~b~{shiftKeyTexture}+{toggleKeyTexture}~w~ - Exit and save~w~";
        }

        private Dictionary<string, KeyBinding> Keys;

        private void InitializeKeys()
        {
            Keys = new Dictionary<string, KeyBinding>();
            InitializeModeKeys();
            InitializeMovementKeys();
            InitializeScaleKeys();
            InitializeDisplayKeys();
        }

        private void InitializeModeKeys()
        {
            Keys["AdjustmentModeToggle"] = new KeyBinding
            {
                Name = "AdjustmentModeToggle",
                Key = _config.AdjustmentModeToggleKey,
                IsPressed = false,
                ModifierKey = ShiftKeyVK,
                Pressed = () =>
                {
                    if (_adjustmentMode)
                    {
                        _config.Save();
                        Utils.DisplayHelpText("~y~Settings Saved & Adjustment Mode Disabled~w~");
                    }
                    else
                        Utils.DisplayHelpText(_adjustmentHelpText);
                    _adjustmentMode = !_adjustmentMode;
                }
            };

            Keys["DisplaySelector"] = new KeyBinding
            {
                Name = "DisplaySelector",
                Key = _config.DisplaySelectorKey,
                IsPressed = false,
                Pressed = () =>
                {
                    if (_adjustmentMode)
                        _config.Save();
                    _selectedDisplayIndex = GetNextEnabledDisplay();
                }
            };
        }

        private void InitializeMovementKeys()
        {
            Keys["Left"] = new KeyBinding
            {
                Name = "Left",
                Key = _config.LeftKey,
                IsPressed = false,
                Pressed = () => AdjustPositionX(-DisplayConstants.MovementIncrement)
            };

            Keys["Up"] = new KeyBinding
            {
                Name = "Up",
                Key = _config.UpKey,
                IsPressed = false,
                Pressed = () => AdjustPositionY(-DisplayConstants.MovementIncrement)
            };

            Keys["Right"] = new KeyBinding
            {
                Name = "Right",
                Key = _config.RightKey,
                IsPressed = false,
                Pressed = () => AdjustPositionX(DisplayConstants.MovementIncrement)
            };

            Keys["Down"] = new KeyBinding
            {
                Name = "Down",
                Key = _config.DownKey,
                IsPressed = false,
                Pressed = () => AdjustPositionY(DisplayConstants.MovementIncrement)
            };
        }

        private void InitializeScaleKeys()
        {
            Keys["ScaleIncrease"] = new KeyBinding
            {
                Name = "ScaleIncrease",
                Key = _config.ScaleIncreaseKey,
                IsPressed = false,
                Pressed = () => AdjustScale(DisplayConstants.MovementIncrement)
            };

            Keys["ScaleDecrease"] = new KeyBinding
            {
                Name = "ScaleDecrease",
                Key = _config.ScaleDecreaseKey,
                IsPressed = false,
                Pressed = () => AdjustScale(-DisplayConstants.MovementIncrement)
            };
        }

        private void InitializeDisplayKeys()
        {
            Keys["FontSwitch"] = new KeyBinding
            {
                Name = "FontSwitch",
                Key = _config.FontSwitchKey,
                IsPressed = false,
                Pressed = () =>
                {
                    if (_adjustmentMode && _selectedDisplayIndex != DisplayIndex.StaminaRectangle)
                    {
                        DisplayTextConfig display = GetCurrentDisplay();
                        // cycle through valid fonts
                        int currentIndex = Array.IndexOf(DisplayConstants.ValidFonts, display.Font);
                        if (currentIndex == -1) currentIndex = 0; // default to 0 if font not found
                        int nextIndex = (currentIndex + 1) % DisplayConstants.ValidFonts.Length;
                        display.Font = DisplayConstants.ValidFonts[nextIndex];
                    }
                }
            };

            Keys["DisplayToggle"] = new KeyBinding
            {
                Name = "DisplayToggle",
                Key = _config.DisplayToggleKey,
                IsPressed = false,
                Pressed = () =>
                {
                    if (_adjustmentMode)
                        ToggleCurrentDisplay();
                }
            };
        }

        private DisplayTextConfig GetCurrentDisplay()
        {
            if (_displayConfigMap.TryGetValue(_selectedDisplayIndex, out var getter))
                return getter();
            
            // default fallback
            return _config.DisplayTime;
        }

        private ProgressBarRectangleConfig GetCurrentRectangleDisplay()
        {
            return _config.StaminaBarRectangle;
        }

        private void AdjustPositionX(float delta)
        {
            if (_selectedDisplayIndex == DisplayIndex.StaminaRectangle)
            {
                ProgressBarRectangleConfig rect = GetCurrentRectangleDisplay();
                rect.X = Math.Max(DisplayConstants.MinPosition, Math.Min(DisplayConstants.MaxPosition, rect.X + delta));
            }
            else
            {
                DisplayTextConfig display = GetCurrentDisplay();
                display.X = Math.Max(DisplayConstants.MinPosition, Math.Min(DisplayConstants.MaxPosition, display.X + delta));
            }
        }

        private void AdjustPositionY(float delta)
        {
            if (_selectedDisplayIndex == DisplayIndex.StaminaRectangle)
            {
                ProgressBarRectangleConfig rect = GetCurrentRectangleDisplay();
                rect.Y = Math.Max(DisplayConstants.MinPosition, Math.Min(DisplayConstants.MaxPosition, rect.Y + delta));
            }
            else
            {
                DisplayTextConfig display = GetCurrentDisplay();
                display.Y = Math.Max(DisplayConstants.MinPosition, Math.Min(DisplayConstants.MaxPosition, display.Y + delta));
            }
        }

        private void AdjustScale(float delta)
        {
            if (_selectedDisplayIndex == DisplayIndex.StaminaRectangle)
            {
                ProgressBarRectangleConfig rect = GetCurrentRectangleDisplay();
                rect.Width = Math.Max(DisplayConstants.MinRectangleWidth, Math.Min(DisplayConstants.MaxPosition, rect.Width + delta));
            }
            else
            {
                DisplayTextConfig display = GetCurrentDisplay();
                display.Scale = Math.Max(DisplayConstants.MinScale, Math.Min(DisplayConstants.MaxScale, display.Scale + delta));
            }
        }

        private bool IsDisplayEnabled(DisplayIndex displayIndex)
        {
            if (_displayEnabledMap.TryGetValue(displayIndex, out var getter))
                return getter();
            
            // default fallback
            return false;
        }

        /// <summary>
        /// Determines if a display should be shown at 35% opacity (disabled state) in adjustment mode.
        /// </summary>
        /// <param name="displayIndex">The display index to check.</param>
        /// <returns>True if the display should be shown at disabled opacity (35%), false otherwise.</returns>
        public bool ShouldDisplayAtDisabledOpacity(DisplayIndex displayIndex)
        {
            return _adjustmentMode && !IsDisplayEnabled(displayIndex);
        }

        private void ToggleCurrentDisplay()
        {
            if (_displayToggleMap.TryGetValue(_selectedDisplayIndex, out var toggleAction))
                toggleAction();
        }

        private DisplayIndex GetNextEnabledDisplay()
        {
            DisplayIndex startIndex = _selectedDisplayIndex;
            int totalDisplays = DisplayConstants.TotalDisplays;
            int currentIntIndex = ((int)_selectedDisplayIndex + 1) % totalDisplays;
            int attempts = 0;

            // cycle through displays until we find an enabled one, or we've checked all displays
            while (!IsDisplayEnabled((DisplayIndex)currentIntIndex) && attempts < totalDisplays)
            {
                currentIntIndex = (currentIntIndex + 1) % totalDisplays;
                attempts++;
            }

            // if no enabled display found, return the original index
            return attempts >= totalDisplays ? startIndex : (DisplayIndex)currentIntIndex;
        }

        private void HandleKeyPress(KeyBinding binding)
        {
            bool isKeyDown = (GetAsyncKeyState(binding.Key) & 0x8000) != 0;
            bool isModifierDown = binding.ModifierKey == null || (GetAsyncKeyState(binding.ModifierKey.Value) & 0x8000) != 0;
            bool isKeyJustPressed = isKeyDown && isModifierDown && !binding.IsPressed;
            bool isKeyReleased = (!isKeyDown || !isModifierDown) && binding.IsPressed;

            if (isKeyJustPressed && binding.Pressed != null) 
                binding.Pressed();
            if (isKeyDown && isModifierDown && binding.Held != null) 
                binding.Held();
            if (isKeyReleased && binding.Released != null) 
                binding.Released();

            binding.IsPressed = isKeyDown && isModifierDown;
        }

        /// <summary>
        /// Processes keyboard input and handles key press events for adjustment mode and display controls.
        /// </summary>
        public void HandleKeys()
        {
            if (Keys == null) return;

            foreach (var binding in Keys.Values)
            {
                try
                {
                    // always handle adjustment mode toggle key (requires Shift modifier)
                    if (binding.Name == "AdjustmentModeToggle")
                        HandleKeyPress(binding);
                    // only handle adjustment keys (including display selector) if adjustment mode is active
                    else if (_adjustmentMode)
                        HandleKeyPress(binding);
                }
                catch (Exception ex)
                {
                    // log error but continue processing other keys
                    Utils.LogError($"InputHandler.HandleKeys processing key '{binding?.Name}'", ex);
                }
            }
        }
    }
}
