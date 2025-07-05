using HarmonyLib;
using StardewValley;
using StardewValley.Tools;
using StardropScroll.IDs;

namespace StardropScroll.Content.Mission.MissionPatches
{
    [HarmonyPatch(typeof(FishingRod))]
    public static class MP_FishingRod
    {
        [HarmonyPatch(nameof(FishingRod.doneHoldingFish))]
        [HarmonyPostfix]
        private static void DoneHoldingFish(FishingRod __instance, Farmer who, bool endOfNight = false)
        {
            MissionManager.Increase(MissionID.CatchFishes);
            if (__instance.treasureCaught)
            {
                MissionManager.Increase(__instance.goldenTreasure ?
                    MissionID.CatchGoldenFishTreasures : MissionID.CatchFishTreasures);
            }
        }
    }
}
