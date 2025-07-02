using StardewModdingAPI.Utilities;

namespace StardropScroll.Config
{
    public class ModConfig
    {
        public KeybindList OpenMenu { get; set; } = new(StardewModdingAPI.SButton.Home);
    }
}
