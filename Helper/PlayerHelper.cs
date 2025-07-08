using StardewValley;
using StardewValley.Constants;

namespace StardropScroll.Helper
{
    public static class PlayerHelper
    {
        public static bool HasProfession(this Farmer player, int id) => player.professions.Contains(id);
        public static bool MasteryFarming(this Farmer player) => player.stats.Get(StatKeys.Mastery(0)) > 0U;
        public static bool MasteryFishing(this Farmer player) => player.stats.Get(StatKeys.Mastery(1)) > 0U;
        public static bool MasteryForaging(this Farmer player) => player.stats.Get(StatKeys.Mastery(2)) > 0U;
        public static bool MasteryMining(this Farmer player) => player.stats.Get(StatKeys.Mastery(3)) > 0U;
        public static bool MasteryCombat(this Farmer player) => player.stats.Get(StatKeys.Mastery(4)) > 0U;

    }
}
