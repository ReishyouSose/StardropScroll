using HarmonyLib;
using StardewValley;
using StardewValley.Tools;
using StardropScroll.Helper;
using StardropScroll.IDs;
using System.Reflection.Emit;

namespace StardropScroll.Content.Mission.MissionPatches
{
    [HarmonyPatch(typeof(Hoe))]
    public static class MP_Hoe
    {
        [HarmonyPatch(nameof(Hoe.DoFunction))]
        [HarmonyTranspiler]
        private static List<CodeInstruction> Hoeing(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            for (int i = 0; i < codes.Count; i++)
            {
                var code = codes[i];
                if (code.opcode != OpCodes.Callvirt)
                    continue;
                if (!code.Contains("makeHoeDirt"))
                    continue;
                if (codes[i + 1].opcode != OpCodes.Brfalse)
                    continue;
                i++;
                codes.MissionIncrease(ref i, MissionID.HoeDirts);
            }
            return codes;
        }
    }
}
