using HarmonyLib;
using StardewValley.TerrainFeatures;

namespace StardropScroll.Content.Mission.MissionPatches
{
    public static class MissionPatcher
    {
        public static void HarmonyPatch(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(typeof(Tree), nameof(Tree.tickUpdate)),
                transpiler: new HarmonyMethod(typeof(MP_Tree), nameof(MP_Tree.ModifyTreeDrop)));
        }
    }
}
