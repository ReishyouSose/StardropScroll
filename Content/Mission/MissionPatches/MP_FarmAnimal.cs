using HarmonyLib;
using StardewValley;
using StardropScroll.Helper;
using StardropScroll.IDs;
using System.Reflection.Emit;
using static StardropScroll.Helper.ILHelper;

namespace StardropScroll.Content.Mission.MissionPatches
{
    [HarmonyPatch(typeof(FarmAnimal))]
    public static class MP_FarmAnimal
    {
        [HarmonyPatch(nameof(FarmAnimal.pet))]
        [HarmonyTranspiler]
        private static List<CodeInstruction> Pet(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            for (int i = 0; i < codes.Count; i++)
            {
                var code = codes[i];
                if (code.opcode != OpCodes.Ldfld)
                    continue;
                if (!code.operand.ToString().Contains("wasPet"))
                    continue;
                if (codes[i + 1].opcode != OpCodes.Ldc_I4_1)
                    continue;
                if (!codes[i + 2].Contains("set_Value"))
                    continue;
                i += 2;
                codes.MissionIncrease(ref i, MissionID.PetAnimals);
                //增加额外好感
            }
            return codes;
        }
    }
}
