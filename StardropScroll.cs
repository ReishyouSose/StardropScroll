using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardropScroll.Config;
using StardropScroll.Content.Mission;
using StardropScroll.Helper;
using StardropScroll.IDs;
using StardropScroll.UI;

namespace StardropScroll
{
    public class StardropScroll : Mod
    {
        internal static StardropScroll Ins;

        public ModConfig config;
        public StardropScroll() => Ins = this;
        public override void Entry(IModHelper helper)
        {
            config = helper.ReadConfig<ModConfig>();
            I18n.Init(helper.Translation);
            new Harmony(ModManifest.UniqueID).PatchAll();
            helper.Events.Input.ButtonPressed += Input_ButtonPressed;
            helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
            helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
            helper.Events.Multiplayer.ModMessageReceived += Multiplayer_ModMessageReceived;
            helper.Events.GameLoop.OneSecondUpdateTicked +=  MissionManager.CheckIncrease;
            ;
        }

        private void Multiplayer_ModMessageReceived(object? sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID == ModManifest.UniqueID)
            {
                if (!int.TryParse(e.Type, out int messageID))
                {
                    Main.LogWarn("Net receive error");
                    return;
                }
                switch (messageID)
                {
                    case NetMessageID.Mission:
                        MissionManager.NetReceive(e);
                        break;
                }
            }
        }

        private void GameLoop_SaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            MissionManager.LoadData();
        }

        private void GameLoop_GameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            MissionManager.LoadMissionData();
            var api = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (api == null)
                return;
            api.Register(ModManifest, reset: () => config = new(), save: () => Helper.WriteConfig(config));
            api.AddKeybindList(ModManifest, () => config.OpenMenu, v => config.OpenMenu = v,
                () => I18n.Config_OpenMenu(), () => I18n.Config_OpenMenuDesc(), nameof(config.OpenMenu));
        }

        private void Input_ButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (Context.CanPlayerMove && Game1.activeClickableMenu == null && config.OpenMenu.IsDown())
            {
                Game1.activeClickableMenu = new MissionUI();
            }
            else if (Game1.activeClickableMenu is MissionUI)
            {
                Game1.exitActiveMenu();
            }
        }
    }
}
