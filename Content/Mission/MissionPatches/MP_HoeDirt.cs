using HarmonyLib;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardropScroll.IDs;

namespace StardropScroll.Content.Mission.MissionPatches
{
    [HarmonyPatch(typeof(HoeDirt))]
    public static class MP_HoeDirt
    {
        [HarmonyPatch(typeof(HoeDirt), nameof(HoeDirt.plant))]
        [HarmonyPostfix]
        private static void PlantCrops(HoeDirt __instance, string itemId, Farmer who, bool isFertilizer, bool __result)
        {
            if (!isFertilizer && __result) // 确保是种子且种植成功
            {
                MissionManager.Increase(MissionID.PlantCrops);
            }
        }
    }
}
