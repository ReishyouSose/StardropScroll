using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace StardropScroll.Config
{
    public class ModConfig
    {
        public bool DebugMode { get; set; } = true;
        public bool ReplaceUnStackable { get; set; } = false;
        public KeybindList OpenMenu { get; set; } = new(SButton.Home);
    }
}
