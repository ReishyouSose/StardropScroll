using HarmonyLib;
using StardewValley;
using StardropScroll.Helper;
using StardropScroll.IDs;
using System.Reflection.Emit;
using StardewO = StardewValley.Object;

namespace StardropScroll.Content.Mission.MissionPatches
{
    [HarmonyPatch(typeof(StardewO))]
    public static class MP_Object
    {
        [HarmonyPatch(nameof(StardewO.placementAction))]
        [HarmonyPostfix]
        private static void PlacementAction(StardewO __instance, GameLocation location, int x, int y, Farmer who, bool __result)
        {
            if (!__result)
                return;
            if (__instance.IsWildTreeSapling())
                MissionManager.Increase(MissionID.PlantWildTrees);
            if (__instance.IsFruitTreeSapling())
                MissionManager.Increase(MissionID.PlantFruitTrees);
        }

        [HarmonyPatch("getPriceAfterMultipliers")]
        [HarmonyPostfix]
        private static void GetPriceAfterMultipliers(StardewO __instance, float startPrice, ref float __result)
        {
            float multiplier = __result / startPrice;
            MissionBonus.ExtraItemValue(__instance, ref multiplier);
            __result = startPrice * multiplier;
        }

        [HarmonyPatch(nameof(StardewO.onExplosion))]
        [HarmonyTranspiler]
        private static List<CodeInstruction> OnExplosion(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            for (int i = 0; i < codes.Count; i++)
            {
                var code = codes[i];
                if (code.opcode != OpCodes.Callvirt)
                    continue;
                if (!code.Contains("cutWeed"))
                    continue;
                codes.MissionIncrease(ref i, MissionID.ClearWeeds);
                List<CodeInstruction> list = new()
                {
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldarg_1),
                    ILHelper.Call(typeof(MissionBonus), nameof(MissionBonus.ExtraWeedsDrop))
                };
                codes.InsertRange(i + 1, list);
                break;
            }
            return codes;
        }

        [HarmonyPatch(nameof(StardewO.performToolAction))]
        [HarmonyTranspiler]
        private static List<CodeInstruction> PerformToolAction(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            for (int i = 0; i < codes.Count; i++)
            {
                var code = codes[i];
                if (code.opcode != OpCodes.Callvirt)
                    continue;
                if (!code.Contains("cutWeed"))
                    continue;
                codes.MissionIncrease(ref i, MissionID.ClearWeeds);
                List<CodeInstruction> list = new()
                {
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldarg_1),
                    ILHelper.Call(typeof(Tool), nameof(Tool.getLastFarmerToUse), code: OpCodes.Callvirt),
                    ILHelper.Call(typeof(MissionBonus), nameof(MissionBonus.ExtraWeedsDrop))
                };
                codes.InsertRange(i + 1, list);
                break;
            }
            return codes;
        }
    }
}
