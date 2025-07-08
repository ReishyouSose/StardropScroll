using HarmonyLib;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardropScroll.Helper;
using StardropScroll.IDs;
using System.Reflection.Emit;

namespace StardropScroll.Content.Mission.MissionPatches
{
    [HarmonyPatch(typeof(Tree))]
    public class MP_Tree
    {
        [HarmonyPatch(nameof(Tree.tickUpdate))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> TickUpdate(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            var codes = instructions.ToList();
            for (int i = 0; i < codes.Count; i++)
            {
                var code = codes[i];
                if (code.opcode != OpCodes.Call)
                    continue;
                if (!code.operand.ToString().Contains(nameof(Game1.createRadialDebris)))
                    continue;
                var list = new List<CodeInstruction>()
                {
                    ILHelper.Instance(),
                    ILHelper.Call(typeof(MissionBonus),(nameof(MissionBonus.ExtraWoodDrop))),
                };
                codes.InsertRange(i, list);
                break;
            }
            return codes;
        }

        [HarmonyPatch("performTreeFall")]
        [HarmonyTranspiler]
        private static List<CodeInstruction> PerformTreeFall(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            for (int i = 0; i < codes.Count; i++)
            {
                var code = codes[i];
                if (code.opcode == OpCodes.Callvirt && code.operand.ToString().Contains("Increment"))
                {
                    codes.MissionIncrease(ref i, MissionID.CutTrees);
                    break;
                }
            }
            return codes;
        }

        [HarmonyPatch(nameof(Tree.performToolAction))]
        [HarmonyTranspiler]
        private static List<CodeInstruction> PerformToolAction(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            for (int i = 0; i < codes.Count; i++)
            {
                var code = codes[i];
                if (code.opcode != OpCodes.Ldfld || !code.Contains("hasMoss"))
                    continue;
                if (codes[i + 1].opcode != OpCodes.Ldc_I4_0)
                    continue;
                if (!codes[i + 2].Contains("set_Value"))
                    continue;
                List<CodeInstruction> list = new()
                {
                    new(OpCodes.Ldloc_2),
                    ILHelper.Call(typeof(MP_Tree), nameof(IncreaseHarvestMoss))
                };
                codes.InsertRange(i + 3, list);
            }
            return codes;
        }

        [HarmonyPatch(nameof(Tree.dayUpdate))]
        [HarmonyPostfix]
        private static void DayUpdate(Tree __instance)
        {
            Tree t = __instance;
            if (t.destroy.Value)
                return;
            if (t.stump.Value)
                return;
            MissionBonus.ExtraTreeGrowChance(t);
            MissionBonus.ExtraTreeMossChance(t);
            MissionBonus.ExtraTreeImmediatelyTapper(t);
        }
        private static void IncreaseHarvestMoss(Item moss)
        {
            MissionManager.Increase(MissionID.HarvestMoss, moss.Stack);
        }
    }
}
