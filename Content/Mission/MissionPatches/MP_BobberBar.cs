using HarmonyLib;
using StardewValley.Menus;

namespace StardropScroll.Content.Mission.MissionPatches
{
    [HarmonyPatch(typeof(BobberBar))]
    public static class MP_BobberBar
    {
        [HarmonyPatch(MethodType.Constructor, new[] { typeof(string), typeof(float), typeof(bool), typeof(List<string>), typeof(string), typeof(bool), typeof(string), typeof(bool) })]
        [HarmonyPrefix]
        private static void BobberBar(ref bool treasure, ref bool goldenTreasure)
        {
            MissionBonus.ExtraFishTreasure(ref treasure, ref goldenTreasure);
        }
    }
}
