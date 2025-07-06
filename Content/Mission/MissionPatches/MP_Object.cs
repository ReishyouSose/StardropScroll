using HarmonyLib;
using StardewValley;
using StardropScroll.Helper;
using StardropScroll.IDs;
using System.Reflection.Emit;
using SObject = StardewValley.Object;

namespace StardropScroll.Content.Mission.MissionPatches
{
    [HarmonyPatch(typeof(SObject))]
    public static class MP_Object
    {
        [HarmonyPatch(nameof(SObject.placementAction))]
        [HarmonyTranspiler]
        private static List<CodeInstruction> PlacementAction(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            for (int i = 0; i < codes.Count; i++)
            {
                var code = codes[i];
                if (code.opcode != OpCodes.Ldstr || code.Contains("wildtreesplanted"))
                    continue;
                bool find = false;
                for (int j = i + 1; j < codes.Count; j++)
                {
                    code = codes[j];
                    if (code.opcode != OpCodes.Callvirt || code.Contains("Increment"))
                        continue;
                    if (codes[j + 1].opcode != OpCodes.Pop)
                        continue;
                    codes.MissionIncrease(ref j, MissionID.PlantTrees);
                    find = true;
                    break;
                }
                if (find)
                    break;
            }
            return codes;
        }
    }
}
