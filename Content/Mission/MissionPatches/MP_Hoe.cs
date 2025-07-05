using HarmonyLib;
using StardewValley.Tools;
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
                if (!code.operand.ToString().Contains("makeHoeDirt"))
                    continue;
                codes.MissionIncrease(ref i, MissionID.HoeDirts);
            }
            return codes;
        }
    }
}
