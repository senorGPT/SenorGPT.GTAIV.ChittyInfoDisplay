using System;
using System.IO;
using IVSDKDotNet;

namespace SenorGPT.GTAIV.ChittyInfoDisplay
{
    /// <summary>
    /// Manages all configuration settings for the mod, including display positions, colors, key bindings, and display toggles.
    /// Handles loading from and saving to an INI configuration file.
    /// </summary>
    public class Config
    {
        #region Config Variable Definitions & Initializations        
        // time display settings
        public bool ShouldDisplayTime { get; set; } = true;
        public bool TwentyFourHourMode { get; set; } = false;

        // date display settings
        public bool ShouldDisplayDate { get; set; } = true;

        // days passed display settings
        public bool ShouldDisplayDaysPassed { get; set; } = true;

        // speedometer display settings
        public bool ShouldDisplaySpeedometer { get; set; } = false;
        public bool SpeedometerUseMPH { get; set; } = true; // true for MPH, false for KM/H
        public bool SpeedometerShowRPM { get; set; } = false; // true to show RPM alongside speed

        // input settings
        public int AdjustmentModeToggleKey { get; set; } = 0x46; // F key by default
        public int DisplaySelectorKey { get; set; } = 0x47; // G key by default
        public int LeftKey { get; set; } = 0x25; // Left Arrow by default
        public int UpKey { get; set; } = 0x26; // Up Arrow by default
        public int RightKey { get; set; } = 0x27; // Right Arrow by default
        public int DownKey { get; set; } = 0x28; // Down Arrow by default
        public int ScaleIncreaseKey { get; set; } = 0xBB; // Plus (=/+) key by default
        public int ScaleDecreaseKey { get; set; } = 0xBD; // Minus (-/_) key by default
        public int FontSwitchKey { get; set; } = 0x48; // H key by default
        public int DisplayToggleKey { get; set; } = 0x54; // T key by default

        // stamina settings
        public bool ShouldDisplayStaminaTextProgressBar { get; set; } = false;
        public bool ShouldDisplayStaminaRectangleProgressBar { get; set; } = true;
        public bool ShouldDisplayStaminaSimple {  get; set; } = false;
        public bool ShouldDisplayStaminaValue { get; set; } = true;
        public bool DisplayStaminaValueAsPercentage { get; set; } = true;

        public uint[] StaminaBarTextCantSprintRGBA { get; set; } = new uint[] {255, 200, 0, 255};
        public string StaminaSimpleFormat { get; set; } = "Stamina: {0}";

        // progress bar settings
        public ProgressBarTextConfig ProgressBar { get; set; } = new ProgressBarTextConfig();
        public ProgressBarRectangleConfig StaminaBarRectangle { get; set; } = new ProgressBarRectangleConfig();

        // display message positions and scales
        public DisplayTextConfig DisplayTime { get; set; } = new DisplayTextConfig
        {
            X = 0.0875f,
            Y = 0.69f,
            Scale = 0.2f
        };

        public DisplayTextConfig DisplayDate { get; set; } = new DisplayTextConfig
        {
            X = 0.04f,
            Y = 0.71f,
            Scale = 0.2f
        };

        public DisplayTextConfig DisplayPassed { get; set; } = new DisplayTextConfig
        {
            X = 0.07f,
            Y = 0.73f,
            Scale = 0.2f
        };

        public DisplayTextConfig DisplayStamina { get; set; } = new DisplayTextConfig
        {
            X = 0.02f,
            Y = 0.97f,
            Scale = 0.2f
        };

        public DisplayTextConfig DisplayStaminaValue { get; set; } = new DisplayTextConfig
        {
            X = 0.016f,
            Y = 0.955f,
            Scale = 0.175f
        };

        public DisplayTextConfig DisplayStaminaSimple { get; set; } = new DisplayTextConfig {
            X = 0.02f,
            Y = 0.93f,
            Scale = 0.2f
        };

        public DisplayTextConfig DisplaySpeedometer { get; set; } = new DisplayTextConfig
        {
            X = 0.5f,
            Y = 0.85f,
            Scale = 0.3f
        };
        #endregion

        #region Global Variables
        public const string scriptName = "SenorGPT.GTAIV.ChittyInfoDisplay";
        private static readonly string configFilePath = string.Format("{0}\\IVSDKDotNet\\scripts\\{1}.ini", IVGame.GameStartupPath, scriptName);
        private SettingsFile _settingsFile;

        #endregion

        #region Config Save Methods
        /// <summary>
        /// Saves all current configuration settings to the INI file.
        /// </summary>
        public void Save()
        {
            if (_settingsFile == null)
                _settingsFile = new SettingsFile(configFilePath);
            
            // save all settings (not just positions)
            SaveTimeSettings(_settingsFile);
            SaveDateSettings(_settingsFile);
            SaveDaysPassedSettings(_settingsFile);
            SaveSpeedometerSettings(_settingsFile);
            SaveInputSettings(_settingsFile);
            SaveStaminaSettings(_settingsFile);
            SaveStaminaTextBarSettings(_settingsFile);
            SaveStaminaRectangleBarSettings(_settingsFile);
            SaveStaminaSimpleSettings(_settingsFile);
            
            _settingsFile.Save();
        }

        private void SaveTimeSettings(SettingsFile settings)
        {
            const string section = "Time";
            if (!settings.DoesSectionExists(section)) settings.AddSection(section);
            
            SaveValue(settings, section, "EnableTime", ShouldDisplayTime.ToString());
            SaveValue(settings, section, "TwentyFourHourMode", TwentyFourHourMode.ToString());
            SaveValue(settings, section, "PositionX", DisplayTime.X.ToString());
            SaveValue(settings, section, "PositionY", DisplayTime.Y.ToString());
            SaveValue(settings, section, "Scale", DisplayTime.Scale.ToString());
            SaveValue(settings, section, "Font", DisplayTime.Font.ToString());
        }

        private void SaveDateSettings(SettingsFile settings)
        {
            const string section = "Date";
            if (!settings.DoesSectionExists(section)) settings.AddSection(section);
            
            SaveValue(settings, section, "EnableDate", ShouldDisplayDate.ToString());
            SaveValue(settings, section, "PositionX", DisplayDate.X.ToString());
            SaveValue(settings, section, "PositionY", DisplayDate.Y.ToString());
            SaveValue(settings, section, "Scale", DisplayDate.Scale.ToString());
            SaveValue(settings, section, "Font", DisplayDate.Font.ToString());
        }

        private void SaveDaysPassedSettings(SettingsFile settings)
        {
            const string section = "DaysPassed";
            if (!settings.DoesSectionExists(section)) settings.AddSection(section);
            
            SaveValue(settings, section, "EnableDaysPassed", ShouldDisplayDaysPassed.ToString());
            SaveValue(settings, section, "PositionX", DisplayPassed.X.ToString());
            SaveValue(settings, section, "PositionY", DisplayPassed.Y.ToString());
            SaveValue(settings, section, "Scale", DisplayPassed.Scale.ToString());
            SaveValue(settings, section, "Font", DisplayPassed.Font.ToString());
        }

        private void SaveSpeedometerSettings(SettingsFile settings)
        {
            const string section = "Speedometer";
            if (!settings.DoesSectionExists(section)) settings.AddSection(section);
            
            SaveValue(settings, section, "EnableSpeedometer", ShouldDisplaySpeedometer.ToString());
            SaveValue(settings, section, "UseMPH", SpeedometerUseMPH.ToString());
            SaveValue(settings, section, "ShowRPM", SpeedometerShowRPM.ToString());
            SaveValue(settings, section, "PositionX", DisplaySpeedometer.X.ToString());
            SaveValue(settings, section, "PositionY", DisplaySpeedometer.Y.ToString());
            SaveValue(settings, section, "Scale", DisplaySpeedometer.Scale.ToString());
            SaveValue(settings, section, "Font", DisplaySpeedometer.Font.ToString());
        }

        private void SaveInputSettings(SettingsFile settings)
        {
            const string section = "Input";
            if (!settings.DoesSectionExists(section)) settings.AddSection(section);
            
            SaveValue(settings, section, "AdjustmentModeToggleKey", "0x" + AdjustmentModeToggleKey.ToString("X"));
            SaveValue(settings, section, "DisplaySelectorKey", "0x" + DisplaySelectorKey.ToString("X"));
            SaveValue(settings, section, "LeftKey", "0x" + LeftKey.ToString("X"));
            SaveValue(settings, section, "UpKey", "0x" + UpKey.ToString("X"));
            SaveValue(settings, section, "RightKey", "0x" + RightKey.ToString("X"));
            SaveValue(settings, section, "DownKey", "0x" + DownKey.ToString("X"));
            SaveValue(settings, section, "ScaleIncreaseKey", "0x" + ScaleIncreaseKey.ToString("X"));
            SaveValue(settings, section, "ScaleDecreaseKey", "0x" + ScaleDecreaseKey.ToString("X"));
            SaveValue(settings, section, "FontSwitchKey", "0x" + FontSwitchKey.ToString("X"));
            SaveValue(settings, section, "DisplayToggleKey", "0x" + DisplayToggleKey.ToString("X"));
        }

        private void SaveStaminaSettings(SettingsFile settings)
        {
            const string section = "Stamina";
            if (!settings.DoesSectionExists(section)) settings.AddSection(section);
            
            SaveValue(settings, section, "EnableValueText", ShouldDisplayStaminaValue.ToString());
            SaveValue(settings, section, "ShowAsPercentage", DisplayStaminaValueAsPercentage.ToString());
            SaveValue(settings, section, "StaminaValueTextPositionX", DisplayStaminaValue.X.ToString());
            SaveValue(settings, section, "StaminaValueTextPositionY", DisplayStaminaValue.Y.ToString());
            SaveValue(settings, section, "StaminaValueTextScale", DisplayStaminaValue.Scale.ToString());
            SaveValue(settings, section, "StaminaValueTextFont", DisplayStaminaValue.Font.ToString());
        }

        private void SaveStaminaTextBarSettings(SettingsFile settings)
        {
            const string section = "StaminaTextBar";
            if (!settings.DoesSectionExists(section)) settings.AddSection(section);
            
            SaveValue(settings, section, "EnableTextBar", ShouldDisplayStaminaTextProgressBar.ToString());
            SaveValue(settings, section, "Width", ProgressBar.Width.ToString());
            SaveValue(settings, section, "MinStamina", ProgressBar.MinStamina.ToString());
            SaveValue(settings, section, "MaxStamina", ProgressBar.MaxStamina.ToString());
            SaveValue(settings, section, "FilledChar", ProgressBar.FilledChar.ToString());
            SaveValue(settings, section, "EmptyChar", ProgressBar.EmptyChar.ToString());
            SaveValue(settings, section, "LeftBracket", ProgressBar.LeftBracket.ToString());
            SaveValue(settings, section, "RightBracket", ProgressBar.RightBracket.ToString());
            SaveValue(settings, section, "PositionX", DisplayStamina.X.ToString());
            SaveValue(settings, section, "PositionY", DisplayStamina.Y.ToString());
            SaveValue(settings, section, "Scale", DisplayStamina.Scale.ToString());
            SaveValue(settings, section, "Font", DisplayStamina.Font.ToString());
        }

        private void SaveStaminaRectangleBarSettings(SettingsFile settings)
        {
            const string section = "StaminaRectangleBar";
            if (!settings.DoesSectionExists(section)) settings.AddSection(section);
            
            SaveValue(settings, section, "EnableRectangleBar", ShouldDisplayStaminaRectangleProgressBar.ToString());
            SaveValue(settings, section, "X", StaminaBarRectangle.X.ToString());
            SaveValue(settings, section, "Y", StaminaBarRectangle.Y.ToString());
            SaveValue(settings, section, "Width", StaminaBarRectangle.Width.ToString());
            SaveValue(settings, section, "Height", StaminaBarRectangle.Height.ToString());
            SaveValue(settings, section, "BorderThickness", StaminaBarRectangle.BorderThickness.ToString());
            SaveValue(settings, section, "BorderColorRGBA", string.Join(",", StaminaBarRectangle.BorderColorRGBA));
            SaveValue(settings, section, "FilledColorRGBA", string.Join(",", StaminaBarRectangle.FilledColorRGBA));
            SaveValue(settings, section, "EmptyColorRGBA", string.Join(",", StaminaBarRectangle.EmptyColorRGBA));
            SaveValue(settings, section, "CantSprintColorRGBA", string.Join(",", StaminaBarRectangle.CantSprintColorRGBA));
        }

        private void SaveStaminaSimpleSettings(SettingsFile settings)
        {
            const string section = "StaminaSimple";
            if (!settings.DoesSectionExists(section)) settings.AddSection(section);
            
            SaveValue(settings, section, "EnableSimpleText", ShouldDisplayStaminaSimple.ToString());
            SaveValue(settings, section, "SimpleFormat", StaminaSimpleFormat);
            SaveValue(settings, section, "StaminaBarTextCantSprintRGBA", string.Join(",", StaminaBarTextCantSprintRGBA));
            SaveValue(settings, section, "SimpleTextPositionX", DisplayStaminaSimple.X.ToString());
            SaveValue(settings, section, "SimpleTextPositionY", DisplayStaminaSimple.Y.ToString());
            SaveValue(settings, section, "SimpleTextScale", DisplayStaminaSimple.Scale.ToString());
            SaveValue(settings, section, "SimpleTextFont", DisplayStaminaSimple.Font.ToString());
        }
        #endregion

        #region Config Load Methods
        /// <summary>
        /// Loads the configuration from the INI file, or creates a new file with default values if it doesn't exist.
        /// </summary>
        /// <returns>A Config instance with loaded or default settings.</returns>
        public static Config Load()
        {
            Config config = new Config();

            if (!File.Exists(configFilePath))
            {
                File.WriteAllText(configFilePath, "");
                IVGame.Console.Print("Config file created");
            }

            config._settingsFile = new SettingsFile(configFilePath);
            
            // explicitly load the file to ensure existing values are read
            if (!config._settingsFile.Load())
                IVGame.Console.Print("Failed to load config file");
            
            config.Load(config._settingsFile);
            
            // save after loading to ensure any new keys get their default values written
            config._settingsFile.Save();
            
            return config;
        }

        public void Load(SettingsFile settings)
        {
            // time settings (includes position and scale)
            LoadTimeSettings(settings);

            // date settings (includes position and scale)
            LoadDateSettings(settings);

            // days passed settings (includes position and scale)
            LoadDaysPassedSettings(settings);

            // speedometer settings (includes position and scale)
            LoadSpeedometerSettings(settings);

            // input settings
            LoadInputSettings(settings);

            // stamina settings (general display toggles)
            LoadStaminaSettings(settings);

            // stamina text bar settings
            LoadStaminaTextBarSettings(settings);

            // stamina rectangle bar settings
            LoadStaminaRectangleBarSettings(settings);

            // stamina simple text settings (formatting and position)
            LoadStaminaSimpleSettings(settings);

            // only save if we're adding new keys (to write defaults for missing keys)
            // don't save here as it might overwrite existing values
            // settings.Save();
        }
        #endregion

        #region Config Load & Save Helpers
        private void SaveValue(SettingsFile settings, string section, string key, string value)
        {
            if (!settings.DoesKeyExists(section, key))
                settings.AddKeyToSection(section, key);
            settings.SetValue(section, key, value);
        }

        private bool LoadBoolean(SettingsFile settings, string section, string key, bool defaultValue)
        {
            if (!settings.DoesKeyExists(section, key))
            {
                settings.AddKeyToSection(section, key);
                settings.SetValue(section, key, defaultValue.ToString());
                return defaultValue;
            }
            return settings.GetBoolean(section, key, defaultValue);
        }

        private int LoadInteger(SettingsFile settings, string section, string key, int defaultValue)
        {
            if (!settings.DoesKeyExists(section, key))
            {
                settings.AddKeyToSection(section, key);
                settings.SetValue(section, key, defaultValue.ToString());
                return defaultValue;
            }
            return settings.GetInteger(section, key, defaultValue);
        }

        private float LoadFloat(SettingsFile settings, string section, string key, float defaultValue)
        {
            if (!settings.DoesKeyExists(section, key))
            {
                settings.AddKeyToSection(section, key);
                settings.SetValue(section, key, defaultValue.ToString());
                return defaultValue;
            }
            return settings.GetFloat(section, key, defaultValue);
        }

        private string LoadString(SettingsFile settings, string section, string key, string defaultValue)
        {
            if (!settings.DoesKeyExists(section, key))
            {
                settings.AddKeyToSection(section, key);
                settings.SetValue(section, key, defaultValue);
                return defaultValue;
            }
            return settings.GetValue(section, key, defaultValue);
        }

        private void LoadTimeSettings(SettingsFile settings)
        {
            const string section = "Time";
            if (!settings.DoesSectionExists(section)) settings.AddSection(section);

            ShouldDisplayTime = LoadBoolean(settings, section, "EnableTime", true);
            TwentyFourHourMode = LoadBoolean(settings, section, "TwentyFourHourMode", false);
            DisplayTime.X = LoadFloat(settings, section, "PositionX", 0.0875f);
            DisplayTime.Y = LoadFloat(settings, section, "PositionY", 0.69f);
            DisplayTime.Scale = LoadFloat(settings, section, "Scale", 0.2f);
            DisplayTime.Font = Utils.ValidateFont((uint)LoadInteger(settings, section, "Font", (int)DisplayConstants.DefaultFont));
        }

        private void LoadDateSettings(SettingsFile settings)
        {
            const string section = "Date";
            if (!settings.DoesSectionExists(section)) settings.AddSection(section);

            ShouldDisplayDate = LoadBoolean(settings, section, "EnableDate", true);
            DisplayDate.X = LoadFloat(settings, section, "PositionX", 0.04f);
            DisplayDate.Y = LoadFloat(settings, section, "PositionY", 0.71f);
            DisplayDate.Scale = LoadFloat(settings, section, "Scale", 0.2f);
            DisplayDate.Font = Utils.ValidateFont((uint)LoadInteger(settings, section, "Font", (int)DisplayConstants.DefaultFont));
        }

        private void LoadDaysPassedSettings(SettingsFile settings)
        {
            const string section = "DaysPassed";
            if (!settings.DoesSectionExists(section)) settings.AddSection(section);

            ShouldDisplayDaysPassed = LoadBoolean(settings, section, "EnableDaysPassed", true);
            DisplayPassed.X = LoadFloat(settings, section, "PositionX", 0.07f);
            DisplayPassed.Y = LoadFloat(settings, section, "PositionY", 0.73f);
            DisplayPassed.Scale = LoadFloat(settings, section, "Scale", 0.2f);
            DisplayPassed.Font = Utils.ValidateFont((uint)LoadInteger(settings, section, "Font", (int)DisplayConstants.DefaultFont));
        }

        private void LoadSpeedometerSettings(SettingsFile settings)
        {
            const string section = "Speedometer";
            if (!settings.DoesSectionExists(section)) settings.AddSection(section);

            ShouldDisplaySpeedometer = LoadBoolean(settings, section, "EnableSpeedometer", false);
            SpeedometerUseMPH = LoadBoolean(settings, section, "UseMPH", true);
            SpeedometerShowRPM = LoadBoolean(settings, section, "ShowRPM", false);
            DisplaySpeedometer.X = LoadFloat(settings, section, "PositionX", 0.5f);
            DisplaySpeedometer.Y = LoadFloat(settings, section, "PositionY", 0.85f);
            DisplaySpeedometer.Scale = LoadFloat(settings, section, "Scale", 0.3f);
            DisplaySpeedometer.Font = Utils.ValidateFont((uint)LoadInteger(settings, section, "Font", (int)DisplayConstants.DefaultFont));
        }

        private int LoadHexKey(SettingsFile settings, string section, string key, string defaultValue)
        {
            string hexValue = LoadString(settings, section, key, defaultValue);
            hexValue = hexValue.Replace("0x", "").Replace("0X", "");
            
            // validate and parse hex string
            try
            {
                return Convert.ToInt32(hexValue, 16);
            }
            catch (Exception ex) when (ex is FormatException || ex is OverflowException)
            {
                // if hex parsing fails or value is too large, return the default value (which we control and know is valid)
                string defaultHex = defaultValue.Replace("0x", "").Replace("0X", "");
                return Convert.ToInt32(defaultHex, 16);
            }
        }

        /// <summary>
        /// Parses an RGBA array string with error handling, falling back to default values if parsing fails.
        /// </summary>
        /// <typeparam name="T">The numeric type for the array elements (int or uint).</typeparam>
        /// <param name="rgbaString">The comma-separated RGBA string to parse (e.g., "255,200,0,255").</param>
        /// <param name="defaultValue">The default RGBA array to use if parsing fails.</param>
        /// <returns>The parsed RGBA array, or the default value if parsing fails.</returns>
        private T[] ParseRgbaArray<T>(string rgbaString, T[] defaultValue) where T : struct
        {
            try
            {
                return Utils.ParseArray<T>(rgbaString);
            }
            catch (Exception ex)
            {
                // if parsing fails, log error and return default value
                Utils.LogError($"Config.ParseRgbaArray parsing '{rgbaString}'", ex);
                return defaultValue;
            }
        }

        private void LoadInputSettings(SettingsFile settings)
        {
            const string section = "Input";
            if (!settings.DoesSectionExists(section)) settings.AddSection(section);

            AdjustmentModeToggleKey = LoadHexKey(settings, section, "AdjustmentModeToggleKey", "0x46");
            DisplaySelectorKey = LoadHexKey(settings, section, "DisplaySelectorKey", "0x47");
            LeftKey = LoadHexKey(settings, section, "LeftKey", "0x25");
            UpKey = LoadHexKey(settings, section, "UpKey", "0x26");
            RightKey = LoadHexKey(settings, section, "RightKey", "0x27");
            DownKey = LoadHexKey(settings, section, "DownKey", "0x28");
            ScaleIncreaseKey = LoadHexKey(settings, section, "ScaleIncreaseKey", "0xBB");
            ScaleDecreaseKey = LoadHexKey(settings, section, "ScaleDecreaseKey", "0xBD");
            FontSwitchKey = LoadHexKey(settings, section, "FontSwitchKey", "0x48");
            DisplayToggleKey = LoadHexKey(settings, section, "DisplayToggleKey", "0x54");
        }

        private void LoadStaminaSettings(SettingsFile settings)
        {
            const string section = "Stamina";
            if (!settings.DoesSectionExists(section)) settings.AddSection(section);

            // display toggles
            ShouldDisplayStaminaValue = LoadBoolean(settings, section, "EnableValueText", true);
            DisplayStaminaValueAsPercentage = LoadBoolean(settings, section, "ShowAsPercentage", true);

            // stamina value text position and scale
            DisplayStaminaValue.X = LoadFloat(settings, section, "StaminaValueTextPositionX", 0.016f);
            DisplayStaminaValue.Y = LoadFloat(settings, section, "StaminaValueTextPositionY", 0.955f);
            DisplayStaminaValue.Scale = LoadFloat(settings, section, "StaminaValueTextScale", 0.175f);
            DisplayStaminaValue.Font = Utils.ValidateFont((uint)LoadInteger(settings, section, "StaminaValueTextFont", (int)DisplayConstants.DefaultFont));
        }

        private void LoadStaminaTextBarSettings(SettingsFile settings)
        {
            const string section = "StaminaTextBar";
            if (!settings.DoesSectionExists(section)) settings.AddSection(section);

            // display toggle
            ShouldDisplayStaminaTextProgressBar = LoadBoolean(settings, section, "EnableTextBar", true);

            // text bar settings
            ProgressBar.Width = LoadInteger(settings, section, "Width", 20);
            ProgressBar.MinStamina = LoadFloat(settings, section, "MinStamina", -150f);
            ProgressBar.MaxStamina = LoadFloat(settings, section, "MaxStamina", 350f);
            
            string filledChar = LoadString(settings, section, "FilledChar", "@");
            ProgressBar.FilledChar = filledChar.Length > 0 ? filledChar[0] : '@';
            
            string emptyChar = LoadString(settings, section, "EmptyChar", "-");
            ProgressBar.EmptyChar = emptyChar.Length > 0 ? emptyChar[0] : '-';
            
            string leftBracket = LoadString(settings, section, "LeftBracket", "[");
            ProgressBar.LeftBracket = leftBracket.Length > 0 ? leftBracket[0] : '[';
            
            string rightBracket = LoadString(settings, section, "RightBracket", "]");
            ProgressBar.RightBracket = rightBracket.Length > 0 ? rightBracket[0] : ']';

            // text bar position and scale
            DisplayStamina.X = LoadFloat(settings, section, "PositionX", 0.02f);
            DisplayStamina.Y = LoadFloat(settings, section, "PositionY", 0.97f);
            DisplayStamina.Scale = LoadFloat(settings, section, "Scale", 0.2f);
            DisplayStamina.Font = Utils.ValidateFont((uint)LoadInteger(settings, section, "Font", (int)DisplayConstants.DefaultFont));
        }

        private void LoadStaminaRectangleBarSettings(SettingsFile settings)
        {
            const string section = "StaminaRectangleBar";
            if (!settings.DoesSectionExists(section)) settings.AddSection(section);

            // display toggle
            ShouldDisplayStaminaRectangleProgressBar = LoadBoolean(settings, section, "EnableRectangleBar", true);

            // rectangle bar settings
            StaminaBarRectangle.X = LoadFloat(settings, section, "X", 0.11f);
            StaminaBarRectangle.Y = LoadFloat(settings, section, "Y", 0.96f);
            StaminaBarRectangle.Width = LoadFloat(settings, section, "Width", 0.12f);
            StaminaBarRectangle.Height = LoadFloat(settings, section, "Height", 0.01f);
            StaminaBarRectangle.BorderThickness = LoadFloat(settings, section, "BorderThickness", 0.002f);
            
            string borderRgba = LoadString(settings, section, "BorderColorRGBA", "140,140,140,255");
            StaminaBarRectangle.BorderColorRGBA = ParseRgbaArray(borderRgba, StaminaBarRectangle.BorderColorRGBA);
            
            string filledRgba = LoadString(settings, section, "FilledColorRGBA", "255,255,255,255");
            StaminaBarRectangle.FilledColorRGBA = ParseRgbaArray(filledRgba, StaminaBarRectangle.FilledColorRGBA);
            
            string emptyRgba = LoadString(settings, section, "EmptyColorRGBA", "0,0,0,255");
            StaminaBarRectangle.EmptyColorRGBA = ParseRgbaArray(emptyRgba, StaminaBarRectangle.EmptyColorRGBA);
            
            string cantSprintRgba = LoadString(settings, section, "CantSprintColorRGBA", "255,200,0,255");
            StaminaBarRectangle.CantSprintColorRGBA = ParseRgbaArray(cantSprintRgba, StaminaBarRectangle.CantSprintColorRGBA);
        }

        private void LoadStaminaSimpleSettings(SettingsFile settings)
        {
            const string section = "StaminaSimple";
            if (!settings.DoesSectionExists(section)) settings.AddSection(section);

            // display toggle
            ShouldDisplayStaminaSimple = LoadBoolean(settings, section, "EnableSimpleText", false);

            // text formatting
            StaminaSimpleFormat = LoadString(settings, section, "SimpleFormat", "Stamina: {0}");
            string staminaBarTextCantSprintRGBA = LoadString(settings, section, "StaminaBarTextCantSprintRGBA", "255,200,0,255");
            StaminaBarTextCantSprintRGBA = ParseRgbaArray(staminaBarTextCantSprintRGBA, StaminaBarTextCantSprintRGBA);

            // simple text position and scale
            DisplayStaminaSimple.X = LoadFloat(settings, section, "SimpleTextPositionX", 0.02f);
            DisplayStaminaSimple.Y = LoadFloat(settings, section, "SimpleTextPositionY", 0.93f);
            DisplayStaminaSimple.Scale = LoadFloat(settings, section, "SimpleTextScale", 0.2f);
            DisplayStaminaSimple.Font = Utils.ValidateFont((uint)LoadInteger(settings, section, "SimpleTextFont", (int)DisplayConstants.DefaultFont));
        }
        #endregion
    }

    #region Config Classes
    public class DisplayTextConfig
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Scale { get; set; }
        public uint Font { get; set; } = DisplayConstants.DefaultFont; // default font (shadow)
    }

    public class ProgressBarTextConfig
    {
        public int Width { get; set; } = 20;
        public float MinStamina { get; set; } = -150f;
        public float MaxStamina { get; set; } = 350f;
        public char FilledChar { get; set; } = '@';
        public char EmptyChar { get; set; } = '-';
        public char LeftBracket { get; set; } = '[';
        public char RightBracket { get; set; } = ']';
    }

    public class ProgressBarRectangleConfig
    {
        public float X { get; set; } = 0.11f;
        public float Y { get; set; } = 0.96f;
        public float Width { get; set; } = 0.12f;
        public float Height { get; set; } = 0.01f;
        public float BorderThickness { get; set; } = 0.002f;
        public int[] BorderColorRGBA { get; set; } = new int[] {140, 140, 140, 255};
        public int[] FilledColorRGBA { get; set; } = new int[] {255, 255, 255, 255};
        public int[] EmptyColorRGBA { get; set; } = new int[] {0, 0, 0, 255};
        public int[] CantSprintColorRGBA { get; set; } = new int[] {255, 200, 0, 255};
    }
    #endregion
}
