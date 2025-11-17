using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using IVSDKDotNet;
using static IVSDKDotNet.Native.Natives;
using System.IO;

namespace SenorGPT.GTAIV.ChittyInfoDisplay
{
    public class InputHandler
    {
        [DllImport("user32.dll")] 
        private static extern short GetAsyncKeyState(int vKey);

        private bool _adjustmentMode = false;
        private int _selectedDisplayIndex = 0; // 0 = time, 1 = date, 2 = daysPassed, 3 = staminaTextBar, 4 = staminaValue, 5 = staminaSimple, 6 = staminaRectangle
        private readonly Config _config;
        private int _flashCounter = 0;

        public bool IsAdjustmentModeActive => _adjustmentMode;
        public int SelectedDisplayIndex => _selectedDisplayIndex;
        
        // returns true if the display should be visible (for flashing effect)
        // only returns false when in adjustment mode AND the display is selected AND we're in the "off" phase
        public bool ShouldDisplayFlash(int displayIndex)
        {
            // if not in adjustment mode, always visible
            if (!_adjustmentMode)
                return true;
            
            // if in adjustment mode but this display is not selected, always visible
            if (_selectedDisplayIndex != displayIndex)
                return true;
            
            // if in adjustment mode AND this display is selected, flash it
            _flashCounter++;
            return (_flashCounter / 30) % 2 == 0;
        }

        public InputHandler(Config config)
        {
            _config = config;
            InitializeKeys();
        }

        private Dictionary<string, Dictionary<string, object>> Keys;

        private void InitializeKeys()
        {
            Keys = new Dictionary<string, Dictionary<string, object>>()
            {
                ["AdjustmentModeToggle"] = new Dictionary<string, object>
                {
                    ["Name"]        = "AdjustmentModeToggle",
                    ["Key"]         = _config.AdjustmentModeToggleKey,
                    ["IsPressed"]   = false,
                    ["Pressed"]     = (Action)(() =>
                    {
                        if (_adjustmentMode)
                        {
                            _config.Save();
                            Utils.DisplayHelpText("~y~Settings Saved & Adjustment Mode Disabled~w~");
                        }
                        else
                            DisplayAdjustmentHelp();
                        _adjustmentMode = !_adjustmentMode;
                    })
                },
                ["DisplaySelector"] = new Dictionary<string, object>
                {
                    ["Name"]        = "DisplaySelector",
                    ["Key"]         = _config.DisplaySelectorKey,
                    ["IsPressed"]   = false,
                    ["Pressed"]     = (Action)(() =>
                    {
                        if (_adjustmentMode)
                            _config.Save();
                        _selectedDisplayIndex = GetNextEnabledDisplay();
                    })
                },
                ["Left"]    = new Dictionary<string, object>
                {
                    ["Name"]        = "Left",
                    ["Key"]         = _config.LeftKey,
                    ["IsPressed"]   = false,
                    ["Pressed"]     = (Action)(() =>
                    {
                        if (_selectedDisplayIndex == 6) // staminaRectangle
                        {
                            ProgressBarRectangleConfig rect = GetCurrentRectangleDisplay();
                            rect.X = Math.Max(0.0f, rect.X - 0.01f);
                        }
                        else
                        {
                            DisplayTextConfig display = GetCurrentDisplay();
                            display.X = Math.Max(0.0f, display.X - 0.01f);
                        }
                    })
                },
                ["Up"]      = new Dictionary<string, object>
                {
                    ["Name"]        = "Up",
                    ["Key"]         = _config.UpKey,
                    ["IsPressed"]   = false,
                    ["Pressed"]     = (Action)(() =>
                    {
                        if (_selectedDisplayIndex == 6) // staminaRectangle
                        {
                            ProgressBarRectangleConfig rect = GetCurrentRectangleDisplay();
                            rect.Y = Math.Max(0.0f, rect.Y - 0.01f);
                        }
                        else
                        {
                            DisplayTextConfig display = GetCurrentDisplay();
                            display.Y = Math.Max(0.0f, display.Y - 0.01f);
                        }
                    })
                },
                ["Right"]   = new Dictionary<string, object>
                {
                    ["Name"]        = "Right",
                    ["Key"]         = _config.RightKey,
                    ["IsPressed"]   = false,
                    ["Pressed"]     = (Action)(() =>
                    {
                        if (_selectedDisplayIndex == 6) // staminaRectangle
                        {
                            ProgressBarRectangleConfig rect = GetCurrentRectangleDisplay();
                            rect.X = Math.Min(1.0f, rect.X + 0.01f);
                        }
                        else
                        {
                            DisplayTextConfig display = GetCurrentDisplay();
                            display.X = Math.Min(1.0f, display.X + 0.01f);
                        }
                    })
                },
                ["Down"]    = new Dictionary<string, object>
                {
                    ["Name"]        = "Down",
                    ["Key"]         = _config.DownKey,
                    ["IsPressed"]   = false,
                    ["Pressed"]     = (Action)(() =>
                    {
                        if (_selectedDisplayIndex == 6) // staminaRectangle
                        {
                            ProgressBarRectangleConfig rect = GetCurrentRectangleDisplay();
                            rect.Y = Math.Min(1.0f, rect.Y + 0.01f);
                        }
                        else
                        {
                            DisplayTextConfig display = GetCurrentDisplay();
                            display.Y = Math.Min(1.0f, display.Y + 0.01f);
                        }
                    })
                },
                ["ScaleIncrease"] = new Dictionary<string, object>
                {
                    ["Name"]        = "ScaleIncrease",
                    ["Key"]         = _config.ScaleIncreaseKey,
                    ["IsPressed"]   = false,
                    ["Pressed"]     = (Action)(() =>
                    {
                        if (_selectedDisplayIndex == 6) // staminaRectangle - adjust width
                        {
                            ProgressBarRectangleConfig rect = GetCurrentRectangleDisplay();
                            rect.Width = Math.Min(1.0f, rect.Width + 0.01f);
                        }
                        else
                        {
                            DisplayTextConfig display = GetCurrentDisplay();
                            display.Scale = Math.Min(1.0f, display.Scale + 0.01f);
                        }
                    })
                },
                ["ScaleDecrease"] = new Dictionary<string, object>
                {
                    ["Name"]        = "ScaleDecrease",
                    ["Key"]         = _config.ScaleDecreaseKey,
                    ["IsPressed"]   = false,
                    ["Pressed"]     = (Action)(() =>
                    {
                        if (_selectedDisplayIndex == 6) // staminaRectangle - adjust width
                        {
                            ProgressBarRectangleConfig rect = GetCurrentRectangleDisplay();
                            rect.Width = Math.Max(0.01f, rect.Width - 0.01f);
                        }
                        else
                        {
                            DisplayTextConfig display = GetCurrentDisplay();
                            display.Scale = Math.Max(0.0f, display.Scale - 0.01f);
                        }
                    })
                },
                ["FontSwitch"] = new Dictionary<string, object>
                {
                    ["Name"]        = "FontSwitch",
                    ["Key"]         = _config.FontSwitchKey,
                    ["IsPressed"]   = false,
                    ["Pressed"]     = (Action)(() =>
                    {
                        if (_adjustmentMode && _selectedDisplayIndex != 6) // only for text displays
                        {
                            DisplayTextConfig display = GetCurrentDisplay();
                            // cycle through valid fonts: 0, 1, 3, 4, 5 (font 2 is not valid)
                            uint[] validFonts = { 0, 1, /*3,*/ 4, 5 }; // 3 doesnt seem to work for some reason
                            int currentIndex = Array.IndexOf(validFonts, display.Font);
                            if (currentIndex == -1) currentIndex = 0; // default to 0 if font not found
                            int nextIndex = (currentIndex + 1) % validFonts.Length;
                            display.Font = validFonts[nextIndex];
                        }
                    })
                },
                ["DisplayToggle"] = new Dictionary<string, object>
                {
                    ["Name"]        = "DisplayToggle",
                    ["Key"]         = _config.DisplayToggleKey,
                    ["IsPressed"]   = false,
                    ["Pressed"]     = (Action)(() =>
                    {
                        if (_adjustmentMode)
                            ToggleCurrentDisplay();
                    })
                }
            };
        }

        private DisplayTextConfig GetCurrentDisplay()
        {
            switch (_selectedDisplayIndex)
            {
                case 0: // time
                    return _config.DisplayTime;
                case 1: // date
                    return _config.DisplayDate;
                case 2: // daysPassed
                    return _config.DisplayPassed;
                case 3: // staminaTextBar
                    return _config.DisplayStamina;
                case 4: // staminaValue
                    return _config.DisplayStaminaValue;
                case 5: // staminaSimple
                    return _config.DisplayStaminaSimple;
                default:
                    return _config.DisplayTime;
            }
        }

        private ProgressBarRectangleConfig GetCurrentRectangleDisplay()
        {
            return _config.StaminaBarRectangle;
        }

        private bool IsDisplayEnabled(int displayIndex)
        {
            switch (displayIndex)
            {
                case 0: // time
                    return _config.ShouldDisplayTime;
                case 1: // date
                    return _config.ShouldDisplayDate;
                case 2: // daysPassed
                    return _config.ShouldDisplayDaysPassed;
                case 3: // staminaTextBar
                    return _config.ShouldDisplayStaminaTextProgressBar;
                case 4: // staminaValue
                    return _config.ShouldDisplayStaminaValue;
                case 5: // staminaSimple
                    return _config.ShouldDisplayStaminaSimple;
                case 6: // staminaRectangle
                    return _config.ShouldDisplayStaminaRectangleProgressBar;
                default:
                    return false;
            }
        }

        // returns true if the display should be shown at 50% opacity (disabled in adjustment mode)
        public bool ShouldDisplayAtHalfOpacity(int displayIndex)
        {
            return _adjustmentMode && !IsDisplayEnabled(displayIndex);
        }

        private void ToggleCurrentDisplay()
        {
            switch (_selectedDisplayIndex)
            {
                case 0: // time
                    _config.ShouldDisplayTime = !_config.ShouldDisplayTime;
                    break;
                case 1: // date
                    _config.ShouldDisplayDate = !_config.ShouldDisplayDate;
                    break;
                case 2: // daysPassed
                    _config.ShouldDisplayDaysPassed = !_config.ShouldDisplayDaysPassed;
                    break;
                case 3: // staminaTextBar
                    _config.ShouldDisplayStaminaTextProgressBar = !_config.ShouldDisplayStaminaTextProgressBar;
                    break;
                case 4: // staminaValue
                    _config.ShouldDisplayStaminaValue = !_config.ShouldDisplayStaminaValue;
                    break;
                case 5: // staminaSimple
                    _config.ShouldDisplayStaminaSimple = !_config.ShouldDisplayStaminaSimple;
                    break;
                case 6: // staminaRectangle
                    _config.ShouldDisplayStaminaRectangleProgressBar = !_config.ShouldDisplayStaminaRectangleProgressBar;
                    break;
            }
        }

        private int GetNextEnabledDisplay()
        {
            int startIndex = _selectedDisplayIndex;
            int currentIndex = (_selectedDisplayIndex + 1) % 7;
            int attempts = 0;

            // cycle through displays until we find an enabled one, or we've checked all 7
            while (!IsDisplayEnabled(currentIndex) && attempts < 7)
            {
                currentIndex = (currentIndex + 1) % 7;
                attempts++;
            }

            // if no enabled display found, return the original index
            return attempts >= 7 ? startIndex : currentIndex;
        }

        private void DisplayAdjustmentHelp()
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
            string shiftKeyTexture = Utils.GetKeyName(0x10); // Shift key VK code

            // build arrow keys string
            string arrowKeysText = $"{upKeyTexture}/{downKeyTexture}/{leftKeyTexture}/{rightKeyTexture}";

            string scaleKeysText = $"{scaleIncreaseKeyTexture}/{scaleDecreaseKeyTexture}";

            string helpText = $"~y~Adjustment Mode Active~n~~b~{displaySelectorKeyTexture}~w~ - Cycle through displays~n~~b~{arrowKeysText}~w~ - Move position~n~~b~{scaleKeysText}~w~ - Adjust scale (or width for rectangle)~n~~b~{fontSwitchKeyTexture}~w~ - Switch font~n~~b~{displayToggleKeyTexture}~w~ - Toggle display on/off~n~~b~{shiftKeyTexture}+{toggleKeyTexture}~w~ - Exit and save~w~";

            Utils.DisplayHelpText(helpText);
        }


        private void HandleKeyPress(int key, ref bool isKeyPressed, Action pressed, Action held = null, Action released = null, int? modifierKey = null)
        {
            bool isKeyDown = (GetAsyncKeyState(key) & 0x8000) != 0;
            bool isModifierDown = modifierKey == null || (GetAsyncKeyState(modifierKey.Value) & 0x8000) != 0;
            bool isKeyJustPressed = isKeyDown && isModifierDown && !isKeyPressed;
            bool isKeyReleased = (!isKeyDown || !isModifierDown) && isKeyPressed;

            if (isKeyJustPressed) pressed();                            // on press
            if (isKeyDown && isModifierDown && held != null) held();    // while held
            if (isKeyReleased && released != null) released();          // on-release

            isKeyPressed = isKeyDown && isModifierDown;
        }

        public void HandleKeys()
        {
            if (Keys == null) return;

            foreach (var data in Keys.Values)
            {
                try
                {
                    if (data == null) continue;

                    // can't pass a dictionary indexer (data["IsPressed"]) by ref. It must be a local/field.
                    if (!data.ContainsKey("IsPressed") || !data.ContainsKey("Name") || !data.ContainsKey("Key") || !data.ContainsKey("Pressed"))
                        continue;

                    bool isPressed = Convert.ToBoolean(data["IsPressed"]);
                    string keyName = (string)data["Name"];
                    
                    // always handle adjustment mode toggle key (requires Shift modifier)
                    if (keyName == "AdjustmentModeToggle")
                    {
                        if (data["Pressed"] is Action pressedAction)
                            HandleKeyPress((int)data["Key"], ref isPressed, pressedAction, null, null, 0x10); // 0x10 is Shift key
                    }
                    // only handle adjustment keys (including display selector) if adjustment mode is active
                    else if (_adjustmentMode)
                    {
                        if (data["Pressed"] is Action pressedAction)
                            HandleKeyPress((int)data["Key"], ref isPressed, pressedAction);
                    }
                    
                    data["IsPressed"] = isPressed; // write the (possibly updated) value back
                }
                catch (Exception ex)
                {
                    // log error but continue processing other keys
                    try
                    {
                        string logPath = string.Format("{0}\\IVSDKDotNet\\scripts\\{1}.log", IVGame.GameStartupPath, Config.scriptName);
                        File.AppendAllText(logPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Error in InputHandler.HandleKeys: {ex.GetType().Name} - {ex.Message}\n");
                    }
                    catch { }
                }
            }
        }
    }
}
