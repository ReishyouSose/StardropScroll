using StardewValley;

namespace StardropScroll.Helper
{
    public static class RandomHelper
    {
        public static Random ByDayPlays(double fix) => Utility.CreateRandom(Game1.dayOfMonth, Game1.seasonIndex, Game1.year, Main.DayPlays, fix);
    }
}
