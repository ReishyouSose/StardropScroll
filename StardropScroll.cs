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
            helper.Events.GameLoop.OneSecondUpdateTicked += MissionManager.CheckIncrease;
            helper.Events.GameLoop.Saving += GameLoop_Saving;
            helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
        }

        private void GameLoop_DayStarted(object? sender, DayStartedEventArgs e)
        {
            MissionManager.GrantRewards();
        }

        private void GameLoop_Saving(object? sender, SavingEventArgs e)
        {
            MissionManager.SaveData();
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
            api.AddBoolOption(ModManifest, () => config.DebugMode, v => config.DebugMode = v, () => I18n.Config_DebugMod());
            api.AddBoolOption(ModManifest, () => config.ReplaceUnStackable, v => config.ReplaceUnStackable = v,
                () => I18n.Config_ReplaceUnstackable(), () => I18n.Config_ReplaceUnstackableDesc());
            api.AddKeybindList(ModManifest, () => config.OpenMenu, v => config.OpenMenu = v,
                () => I18n.Config_OpenMenu(), () => I18n.Config_OpenMenuDesc(), nameof(config.OpenMenu));
        }

        private void Input_ButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (config.OpenMenu.IsDown())
            {
                if (Context.CanPlayerMove && Game1.activeClickableMenu == null)
                {
                    Game1.activeClickableMenu = new MissionMenu();
                }
                else if (Game1.activeClickableMenu is MissionMenu)
                {
                    Game1.exitActiveMenu();
                }
            }
        }
    }
}
