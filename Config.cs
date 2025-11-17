using System;
using System.IO;
using System.Globalization;
using IVSDKDotNet;

namespace SenorGPT.GTAIV.ChittyInfoDisplay
{
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
        public bool ShouldDisplayStaminaTextProgressBar { get; set; } = true;
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
        #endregion

        #region Global Variables
        public const string scriptName = "SenorGPT.GTAIV.ChittyInfoDisplay";
        private static readonly string configFilePath = string.Format("{0}\\IVSDKDotNet\\scripts\\{1}.ini", IVGame.GameStartupPath, scriptName);
        private SettingsFile _settingsFile;

        #endregion

        #region Config Load Methods
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

        public void Save()
        {
            if (_settingsFile == null)
                _settingsFile = new SettingsFile(configFilePath);
            
            // save all settings (not just positions)
            SaveTimeSettings(_settingsFile);
            SaveDateSettings(_settingsFile);
            SaveDaysPassedSettings(_settingsFile);
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

        public void Load(SettingsFile settings)
        {
            // time settings (includes position and scale)
            LoadTimeSettings(settings);

            // date settings (includes position and scale)
            LoadDateSettings(settings);

            // days passed settings (includes position and scale)
            LoadDaysPassedSettings(settings);

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

        #region Config Load Helpers
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
            DisplayTime.Font = (uint)LoadInteger(settings, section, "Font", 4);
        }

        private void LoadDateSettings(SettingsFile settings)
        {
            const string section = "Date";
            if (!settings.DoesSectionExists(section)) settings.AddSection(section);

            ShouldDisplayDate = LoadBoolean(settings, section, "EnableDate", true);
            DisplayDate.X = LoadFloat(settings, section, "PositionX", 0.04f);
            DisplayDate.Y = LoadFloat(settings, section, "PositionY", 0.71f);
            DisplayDate.Scale = LoadFloat(settings, section, "Scale", 0.2f);
            DisplayDate.Font = (uint)LoadInteger(settings, section, "Font", 4);
        }

        private void LoadDaysPassedSettings(SettingsFile settings)
        {
            const string section = "DaysPassed";
            if (!settings.DoesSectionExists(section)) settings.AddSection(section);

            ShouldDisplayDaysPassed = LoadBoolean(settings, section, "EnableDaysPassed", true);
            DisplayPassed.X = LoadFloat(settings, section, "PositionX", 0.07f);
            DisplayPassed.Y = LoadFloat(settings, section, "PositionY", 0.73f);
            DisplayPassed.Scale = LoadFloat(settings, section, "Scale", 0.2f);
            DisplayPassed.Font = (uint)LoadInteger(settings, section, "Font", 4);
        }

        private int LoadHexKey(SettingsFile settings, string section, string key, string defaultValue)
        {
            string hexValue = LoadString(settings, section, key, defaultValue);
            hexValue = hexValue.Replace("0x", "").Replace("0X", "");
            return Convert.ToInt32(hexValue, 16);
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
            DisplayStaminaValue.Font = (uint)LoadInteger(settings, section, "StaminaValueTextFont", 4);
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
            DisplayStamina.Font = (uint)LoadInteger(settings, section, "Font", 4);
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
            StaminaBarRectangle.BorderColorRGBA = Utils.ParseArray<int>(borderRgba);
            
            string filledRgba = LoadString(settings, section, "FilledColorRGBA", "255,255,255,255");
            StaminaBarRectangle.FilledColorRGBA = Utils.ParseArray<int>(filledRgba);
            
            string emptyRgba = LoadString(settings, section, "EmptyColorRGBA", "0,0,0,255");
            StaminaBarRectangle.EmptyColorRGBA = Utils.ParseArray<int>(emptyRgba);
            
            string cantSprintRgba = LoadString(settings, section, "CantSprintColorRGBA", "255,200,0,255");
            StaminaBarRectangle.CantSprintColorRGBA = Utils.ParseArray<int>(cantSprintRgba);
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
            StaminaBarTextCantSprintRGBA = Utils.ParseArray<uint>(staminaBarTextCantSprintRGBA);

            // simple text position and scale
            DisplayStaminaSimple.X = LoadFloat(settings, section, "SimpleTextPositionX", 0.02f);
            DisplayStaminaSimple.Y = LoadFloat(settings, section, "SimpleTextPositionY", 0.93f);
            DisplayStaminaSimple.Scale = LoadFloat(settings, section, "SimpleTextScale", 0.2f);
            DisplayStaminaSimple.Font = (uint)LoadInteger(settings, section, "SimpleTextFont", 4);
        }
        #endregion
    }

    #region Config Classes
    public class DisplayTextConfig
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Scale { get; set; }
        public uint Font { get; set; } = 4; // default font (shadow)
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
