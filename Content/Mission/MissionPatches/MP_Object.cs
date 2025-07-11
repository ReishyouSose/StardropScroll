using HarmonyLib;
using StardewValley;
using StardropScroll.IDs;
using StardewO = StardewValley.Object;

namespace StardropScroll.Content.Mission.MissionPatches
{
    [HarmonyPatch(typeof(StardewO))]
    public static class MP_Object
    {
        [HarmonyPatch(nameof(StardewO.placementAction))]
        [HarmonyPostfix]
        private static void PlacementAction(StardewO __instance, GameLocation location, int x, int y, Farmer who, bool __result)
        {
            if (!__result)
                return;
            if (__instance.IsWildTreeSapling())
                MissionManager.Increase(MissionID.PlantWildTrees);
            if (__instance.IsFruitTreeSapling())
                MissionManager.Increase(MissionID.PlantWildTrees);
        }

        [HarmonyPatch("getPriceAfterMultipliers")]
        [HarmonyPostfix]
        private static void GetPriceAfterMultipliers(StardewO __instance, float startPrice, ref float __result)
        {
            float multiplier = __result / startPrice;
            MissionBonus.ExtraItemValue(__instance, ref multiplier);
            __result = startPrice * multiplier;
        }

        [HarmonyPatch(nameof(StardewO.cutWeed))]
        [HarmonyPrefix]
        private static void CutWeed(StardewO __instance, Farmer who)
        {
            MissionManager.Increase(MissionID.ClearWeeds);
            MissionBonus.ExtraWeedsDrop(__instance);
        }
    }
}
