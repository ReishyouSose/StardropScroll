using HarmonyLib;
using StardewValley;
using StardropScroll.IDs;

namespace StardropScroll.Content.Mission.MissionPatches
{
    [HarmonyPatch(typeof(Stats))]
    public static class MP_Stats
    {
        [HarmonyPatch(nameof(Stats.takeStep))]
        [HarmonyPostfix]
        private static void TakeStep()
        {
            MissionManager.Increase(MissionID.RunTrain);
        }
    }
}