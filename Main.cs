using IVSDKDotNet;
using System;
using static IVSDKDotNet.Native.Natives;

namespace SenorGPT.GTAIV.ChittyInfoDisplay
{
    public class Main : Script
    {
        #region Config Settings
        // config is loaded from .ini file on first run, or defaults are written to .ini if it doesn't exist
        private static readonly Config AppConfig = Config.Load();
        #endregion

        #region Global Variables
        private IVPed playerPed;
        private readonly StaminaManager staminaManager;
        private readonly DisplayTextManager displayTextManager;
        private readonly InputHandler inputHandler;
        #endregion

        #region Constructor
        public Main()
        {
            Tick += OnTick;
            
            inputHandler = new InputHandler(AppConfig);
            staminaManager = new StaminaManager(AppConfig, inputHandler);
            displayTextManager = new DisplayTextManager(AppConfig, inputHandler);
        }
        #endregion

        public void OnTick(object sender, EventArgs e)
        {
            try
            {
                playerPed = IVPed.FromUIntPtr(IVPlayerInfo.FindThePlayerPed());

                // check if the player object exists, player is alive, not in pause menu, and not in a cutscene
                if (playerPed == null || playerPed.Dead || IS_PAUSE_MENU_ACTIVE() || Utils.IsCutsceneActive())
                    return;

                // check if PlayerInfo exists before accessing Stamina
                if (playerPed.PlayerInfo == null)
                    return;

                // update the display text strings only if they've changed
                try
                {
                    displayTextManager.HandleDisplayText();
                }
                catch (Exception ex)
                {
                    LogError("DisplayTextManager.HandleDisplayText", ex);
                }
                
                // handle keyboard input
                try
                {
                    inputHandler.HandleKeys();
                }
                catch (Exception ex)
                {
                    LogError("InputHandler.HandleKeys", ex);
                }
                
                // stamina display
                try
                {
                    staminaManager.HandleStaminaDisplay(playerPed.PlayerInfo.Stamina);
                }
                catch (Exception ex)
                {
                    LogError("StaminaManager.HandleStaminaDisplay", ex);
                }
            }
            catch (Exception ex)
            {
                LogError("Main.OnTick", ex);
            }
        }

        private void LogError(string location, Exception ex)
        {
            // write to a log file for debugging
            try
            {
                string logPath = string.Format("{0}\\IVSDKDotNet\\scripts\\{1}.log", IVGame.GameStartupPath, Config.scriptName);
                string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Error in {location}: {ex.GetType().Name} - {ex.Message}\nStack Trace: {ex.StackTrace}\n\n";
                System.IO.File.AppendAllText(logPath, logMessage);
            }
            catch
            {
                // if logging fails, at least try to display in-game
                try { PRINT_HELP("ERROR_LOG_FAILED"); }
                catch { }
            }
        }
    }
}
