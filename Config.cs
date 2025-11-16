using System;
using System.IO;
using System.Globalization;
using IVSDKDotNet;

namespace SenorGPT.GTAIV.ChittyInfoDisplay
{
    public class Config
    {
        #region Config Variable Definitions & Initializations
        public const string scriptName = "SenorGPT.GTAIV.ChittyInfoDisplay";
        
        // Time display settings
        public bool TwentyFourHourMode { get; set; } = false;

        // Stamina settings
        public bool ShouldDisplayStaminaTextProgressBar { get; set; } = true;
        public bool ShouldDisplayStaminaRectangleProgressBar { get; set; } = true;
        public bool ShouldDisplayStaminaSimple {  get; set; } = false;
        public bool ShouldDisplayStaminaValue { get; set; } = true;
        public bool DisplayStaminaValueAsPercentage { get; set; } = true;

        public uint[] StaminaBarTextCantSprintRGBA { get; set; } = new uint[] {255, 200, 0, 255};
        public string StaminaSimpleFormat { get; set; } = "Stamina: {0}";

        // Progress bar settings
        public ProgressBarTextConfig ProgressBar { get; set; } = new ProgressBarTextConfig();
        public ProgressBarRectangleConfig StaminaBarRectangle { get; set; } = new ProgressBarRectangleConfig();

        // Display message positions and scales
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

        #region Config Load Methods
        public static Config Load()
        {
            Config config = new Config();
            string configFilePath = string.Format("{0}\\IVSDKDotNet\\scripts\\{1}.ini", IVGame.GameStartupPath, scriptName);

            if (!File.Exists(configFilePath))
                File.WriteAllText(configFilePath, "");

            SettingsFile settings = new SettingsFile(configFilePath);
            config.Load(settings);
            return config;
        }

        public void Load(SettingsFile settings)
        {
            // Time settings (includes position and scale)
            LoadTimeSettings(settings);

            // Date settings (includes position and scale)
            LoadDateSettings(settings);

            // Days passed settings (includes position and scale)
            LoadDaysPassedSettings(settings);

            // Stamina settings (general display toggles)
            LoadStaminaSettings(settings);

            // Stamina text bar settings
            LoadStaminaTextBarSettings(settings);

            // Stamina rectangle bar settings
            LoadStaminaRectangleBarSettings(settings);

            // Stamina simple text settings (formatting and position)
            LoadStaminaSimpleSettings(settings);

            settings.Save();
        }
        #endregion

        #region Config Load Helpers
        private bool LoadBoolean(SettingsFile settings, string section, string key, bool defaultValue)
        {
            if (!settings.DoesKeyExists(section, key))
            {
                settings.AddKeyToSection(section, key);
                settings.SetValue(section, key, defaultValue.ToString());
            }
            return settings.GetBoolean(section, key, defaultValue);
        }

        private int LoadInteger(SettingsFile settings, string section, string key, int defaultValue)
        {
            if (!settings.DoesKeyExists(section, key))
            {
                settings.AddKeyToSection(section, key);
                settings.SetValue(section, key, defaultValue.ToString());
            }
            return settings.GetInteger(section, key, defaultValue);
        }

        private float LoadFloat(SettingsFile settings, string section, string key, float defaultValue)
        {
            if (!settings.DoesKeyExists(section, key))
            {
                settings.AddKeyToSection(section, key);
                settings.SetValue(section, key, defaultValue.ToString());
            }
            return settings.GetFloat(section, key, defaultValue);
        }

        private string LoadString(SettingsFile settings, string section, string key, string defaultValue)
        {
            if (!settings.DoesKeyExists(section, key))
            {
                settings.AddKeyToSection(section, key);
                settings.SetValue(section, key, defaultValue);
            }
            return settings.GetValue(section, key, defaultValue);
        }

        private void LoadTimeSettings(SettingsFile settings)
        {
            const string section = "Time";
            if (!settings.DoesSectionExists(section)) settings.AddSection(section);

            TwentyFourHourMode = LoadBoolean(settings, section, "TwentyFourHourMode", false);
            DisplayTime.X = LoadFloat(settings, section, "PositionX", 0.0875f);
            DisplayTime.Y = LoadFloat(settings, section, "PositionY", 0.69f);
            DisplayTime.Scale = LoadFloat(settings, section, "Scale", 0.2f);
        }

        private void LoadDateSettings(SettingsFile settings)
        {
            const string section = "Date";
            if (!settings.DoesSectionExists(section)) settings.AddSection(section);

            DisplayDate.X = LoadFloat(settings, section, "PositionX", 0.04f);
            DisplayDate.Y = LoadFloat(settings, section, "PositionY", 0.71f);
            DisplayDate.Scale = LoadFloat(settings, section, "Scale", 0.2f);
        }

        private void LoadDaysPassedSettings(SettingsFile settings)
        {
            const string section = "DaysPassed";
            if (!settings.DoesSectionExists(section)) settings.AddSection(section);

            DisplayPassed.X = LoadFloat(settings, section, "PositionX", 0.07f);
            DisplayPassed.Y = LoadFloat(settings, section, "PositionY", 0.73f);
            DisplayPassed.Scale = LoadFloat(settings, section, "Scale", 0.2f);
        }

        private void LoadStaminaSettings(SettingsFile settings)
        {
            const string section = "Stamina";
            if (!settings.DoesSectionExists(section)) settings.AddSection(section);

            // Display toggles
            ShouldDisplayStaminaValue = LoadBoolean(settings, section, "EnableValueText", true);
            DisplayStaminaValueAsPercentage = LoadBoolean(settings, section, "ShowAsPercentage", true);

            // Stamina value text position and scale
            DisplayStaminaValue.X = LoadFloat(settings, section, "StaminaValueTextPositionX", 0.016f);
            DisplayStaminaValue.Y = LoadFloat(settings, section, "StaminaValueTextPositionY", 0.955f);
            DisplayStaminaValue.Scale = LoadFloat(settings, section, "StaminaValueTextScale", 0.175f);
        }

        private void LoadStaminaTextBarSettings(SettingsFile settings)
        {
            const string section = "StaminaTextBar";
            if (!settings.DoesSectionExists(section)) settings.AddSection(section);

            // Display toggle
            ShouldDisplayStaminaTextProgressBar = LoadBoolean(settings, section, "EnableTextBar", true);

            // Text bar settings
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

            // Text bar position and scale
            DisplayStamina.X = LoadFloat(settings, section, "PositionX", 0.02f);
            DisplayStamina.Y = LoadFloat(settings, section, "PositionY", 0.97f);
            DisplayStamina.Scale = LoadFloat(settings, section, "Scale", 0.2f);
        }

        private void LoadStaminaRectangleBarSettings(SettingsFile settings)
        {
            const string section = "StaminaRectangleBar";
            if (!settings.DoesSectionExists(section)) settings.AddSection(section);

            // Display toggle
            ShouldDisplayStaminaRectangleProgressBar = LoadBoolean(settings, section, "EnableRectangleBar", true);

            // Rectangle bar settings
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

            // Display toggle
            ShouldDisplayStaminaSimple = LoadBoolean(settings, section, "EnableSimpleText", false);

            // Text formatting
            StaminaSimpleFormat = LoadString(settings, section, "SimpleFormat", "Stamina: {0}");
            string staminaBarTextCantSprintRGBA = LoadString(settings, section, "StaminaBarTextCantSprintRGBA", "255,200,0,255");
            StaminaBarTextCantSprintRGBA = Utils.ParseArray<uint>(staminaBarTextCantSprintRGBA);

            // Simple text position and scale
            DisplayStaminaSimple.X = LoadFloat(settings, section, "SimpleTextPositionX", 0.02f);
            DisplayStaminaSimple.Y = LoadFloat(settings, section, "SimpleTextPositionY", 0.93f);
            DisplayStaminaSimple.Scale = LoadFloat(settings, section, "SimpleTextScale", 0.2f);
        }
        #endregion
    }

    #region Config Classes
    public class DisplayTextConfig
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Scale { get; set; }
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
