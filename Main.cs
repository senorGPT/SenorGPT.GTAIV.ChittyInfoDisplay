using IVSDKDotNet;
using System;
using static IVSDKDotNet.Native.Natives;

namespace SenorGPT.GTAIV.ChittyInfoDisplay
{
    public class Main : Script
    {
        #region Config Settings
        // Config is loaded from .ini file on first run, or defaults are written to .ini if it doesn't exist
        // TODO: For the X, Y, & SCALE for the DisplayTexts: have a debug mode where the player can use the arrow keys to position them as they want
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
            
            staminaManager = new StaminaManager(AppConfig);
            displayTextManager = new DisplayTextManager(AppConfig);
            inputHandler = new InputHandler();
        }
        #endregion

        public void OnTick(object sender, EventArgs e)
        {
            playerPed = IVPed.FromUIntPtr(IVPlayerInfo.FindThePlayerPed());

            // check if the player object exists, player is alive, not in pause menu, and not in a cutscene
            if (playerPed == null || playerPed.Dead || IS_PAUSE_MENU_ACTIVE() || Utils.IsCutsceneActive())
                return;

            // update the display text strings only if they've changed
            displayTextManager.HandleDisplayText();
            
            // handle keyboard input
            inputHandler.HandleKeys();
            
            // stamina display
            staminaManager.HandleStaminaDisplay(playerPed.PlayerInfo.Stamina);

            

            // debug display
            SET_TEXT_SCALE(0.2f, 0.2f);
            DISPLAY_TEXT_WITH_LITERAL_STRING(0.1f, 0.1f, "STRING", "X: " + InputHandler.GetDisplayMessageX().ToString() + " | Y: " + InputHandler.GetDisplayMessageY().ToString());

            // C:\Program Files (x86)\Steam\steamapps\common\Grand Theft Auto IV\GTAIV
            //IVGame.Console.Print(IVGame.GameStartupPath.ToString());

            //SET_TEXT_SCALE(0.2f, 0.2f);
            //DISPLAY_TEXT_WITH_LITERAL_STRING(0.1f, 0.12f, "STRING", "String Resource Folder: " + this.ScriptResourceFolder.ToString());
            //SET_TEXT_SCALE(0.2f, 0.2f);
            //DISPLAY_TEXT_WITH_LITERAL_STRING(0.1f, 0.14f, "STRING", "Game Startup Path: " + IVGame.GameStartupPath.ToString());
        }
    }
}
