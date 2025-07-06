using HarmonyLib;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardropScroll.IDs;

namespace StardropScroll.Content.Mission.MissionPatches
{
    [HarmonyPatch(typeof(HoeDirt))]
    public static class MP_HoeDirt
    {
        [HarmonyPatch(nameof(HoeDirt.plant))]
        [HarmonyPostfix]
        private static void PlantCrops(HoeDirt __instance, string itemId, Farmer who, bool isFertilizer, bool __result)
        {
            if (!isFertilizer && __result) // 确保是种子且种植成功
            {
                MissionManager.Increase(MissionID.PlantCrops);
            }
        }

        [HarmonyPatch(nameof(HoeDirt.dayUpdate))]
        [HarmonyPostfix]
        private static void DayUpdate(HoeDirt __instance)
        {
            int level = MissionManager.GetLevel(MissionID.PlantCrops);
            if (level <= 0)
                return;
            var dirt = __instance;
            if (dirt.crop == null)
                return;
            bool ignore = dirt.hasPaddyCrop() && dirt.paddyWaterCheck(true);
            if (ignore || dirt.state.Value == 1)
            {
                int amount = MissionManager.GetBonusTimes(level, 0.75, 0.9);
                for (int i = 0; i < amount; i++)
                    dirt.crop.newDay(1);
            }
        }
    }
}
