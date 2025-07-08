using HarmonyLib;
using StardewValley.TerrainFeatures;

namespace StardropScroll.Content.Mission.MissionPatches
{
    [HarmonyPatch(typeof(FruitTree))]
    public static class MP_FruitTree
    {
        [HarmonyPatch(nameof(FruitTree.dayUpdate))]
        [HarmonyPrefix]
        private static void DayUpdate(FruitTree __instance)
        {
            FruitTree t = __instance;
            if (t.destroy)
                return;
            if (t.stump.Value)
                return;
            MissionBonus.ExtraFruitTreeGrow(t);
        }
    }
}
