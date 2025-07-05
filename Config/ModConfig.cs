using StardewModdingAPI.Utilities;

namespace StardropScroll.Config
{
    public class ModConfig
    {
        public bool DebugMode { get; set; } = true;
        public KeybindList OpenMenu { get; set; } = new(StardewModdingAPI.SButton.Home);
    }
}
