using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using StardropScroll.Helper;
using StardropScroll.IDs;
using System.Reflection.Emit;
using StardewObject = StardewValley.Object;

namespace StardropScroll.Content.Mission.MissionPatches
{
    [HarmonyPatch]
    public class MissionListener
    {
        internal static void Increase(string name, int amount = 1) => MissionManager.Increase(name, amount);
        internal static CodeInstruction Call() => ILHelper.Call<MissionListener>("Increase");

        [HarmonyPatch(typeof(Tree), "performTreeFall")]
        [HarmonyTranspiler]
        internal static List<CodeInstruction> CutTree(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            for (int i = 0; i < codes.Count; i++)
            {
                var code = codes[i];
                if (code.opcode == OpCodes.Callvirt && code.operand.ToString().Contains("Increment"))
                {
                    List<CodeInstruction> list = new()
                    {
                        new(OpCodes.Ldstr, MissionID.CutTrees),
                        new(OpCodes.Ldc_I4_1),
                        Call(),
                    };
                    codes.InsertRange(i + 2, list);
                    break;
                }
            }
            return codes;
        }

        [HarmonyPatch(typeof(GameLocation),nameof(GameLocation.OnStoneDestroyed))]
        [HarmonyPostfix]
        internal static void OnStoneDestoryed(GameLocation __instance, string stoneId, int x, int y, Farmer who)
        {
            Increase(MissionID.MiningStones);
        }
    }
}
