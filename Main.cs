using IVSDKDotNet;
using System;
using static IVSDKDotNet.Native.Natives;

namespace SenorGPT.GTAIV.ChittyInfoDisplay
{
    /// <summary>
    /// Main script entry point for the GTA IV Chitty Info Display mod.
    /// Manages the game loop and coordinates all display managers.
    /// </summary>
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
                // get player ped pointer and check for zero before conversion
                UIntPtr playerPedPtr = IVPlayerInfo.FindThePlayerPed();
                if (playerPedPtr == UIntPtr.Zero)
                    return;
                
                playerPed = IVPed.FromUIntPtr(playerPedPtr);

                // check if the player object exists, player is alive, not in pause menu, and not in a cutscene
                if (playerPed == null || playerPed.Dead || IS_PAUSE_MENU_ACTIVE() || Utils.IsCutsceneActive())
                    return;

                // check if PlayerInfo exists before accessing Stamina
                if (playerPed.PlayerInfo == null)
                    return;

                // update the display text strings only if they've changed
                try
                {
                    displayTextManager.HandleDisplayText(playerPed);
                }
                catch (Exception ex)
                {
                    Utils.LogError("DisplayTextManager.HandleDisplayText", ex);
                }
                
                // handle keyboard input
                try
                {
                    inputHandler.HandleKeys();
                }
                catch (Exception ex)
                {
                    Utils.LogError("InputHandler.HandleKeys", ex);
                }
                
                // stamina display
                try
                {
                    staminaManager.HandleStaminaDisplay(playerPed.PlayerInfo.Stamina, playerPed);
                }
                catch (Exception ex)
                {
                    Utils.LogError("StaminaManager.HandleStaminaDisplay", ex);
                }
            }
            catch (Exception ex)
            {
                Utils.LogError("Main.OnTick", ex);
            }
        }

    }
}
