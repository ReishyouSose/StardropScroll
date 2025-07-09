using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;
using StardropScroll.Helper;
using StardropScroll.IDs;
using System.Reflection.Emit;
using StardewO = StardewValley.Object;

namespace StardropScroll.Content.Mission.MissionPatches
{
    [HarmonyPatch(typeof(GameLocation))]
    public static class MP_GameLocation
    {
        /*[HarmonyPatch(nameof(GameLocation.OnStoneDestroyed))]
        [HarmonyPostfix]
        private static void MiningStones(GameLocation __instance, string stoneId, int x, int y, Farmer who)
        {
            switch (stoneId)
            {
                //宝石矿
                case VeinID.Diamond_2:
                case VeinID.Ruby_4:
                case VeinID.Jade_6:
                case VeinID.Amethyst_8:
                case VeinID.Topaz_10:
                case VeinID.Emerald_12:
                case VeinID.Aquamarine_14:
                    break;
                //宝石和神秘石
                case VeinID.Gem_44:
                case VeinID.Mystic_46:
                    break;
                //蚌
                case VeinID.Mussel_25:
                    break;
                //很多层都有的那两种
                case VeinID.Stone_668:
                case VeinID.Stone_670:
                    break;
                //露天的
                case VeinID.Stone_343:
                case VeinID.Stone_450:
                    break;
                //普通层
                case VeinID.Stone_32:
                case VeinID.Stone_34:
                case VeinID.Stone_36:
                case VeinID.Stone_38:
                case VeinID.Stone_40:
                case VeinID.Stone_42:
                    break;
                //冰雪层
                case VeinID.Snowy_48:
                case VeinID.Snowy_50:
                case VeinID.Snowy_52:
                case VeinID.Snowy_54:
                    break;
                //熔岩层
                case VeinID.Snowy_56:
                case VeinID.Snowy_58:
                case VeinID.Stone_760:
                case VeinID.Stone_762:
                    break;
                //晶球矿
                case VeinID.Geode_75:
                case VeinID.FrozenGeode_76:
                case VeinID.MagmaGeode_77:
                case VeinID.OmniGeode_819:
                    break;
                //矿石
                case VeinID.Copper_751:
                case VeinID.Iron_290:
                case VeinID.Gold_764:
                case VeinID.Iridium_765:
                case VeinID.Radioactive_95:
                case VeinID.CinderShard_843:
                case VeinID.CinderShard_844:
                //两骷髅矿洞的煤炭
                case VeinID.BasicCoalNode0:
                case VeinID.BasicCoalNode1:
                    break;
                //姜岛版本的矿石
                case VeinID.Copper_849:
                case VeinID.Iron_850:
                case VeinID.VolcanoGoldNode:
                case VeinID.VolcanoCoalNode0:
                case VeinID.VolcanoCoalNode1:
                //姜岛火山的
                case VeinID.Stone_845:
                case VeinID.Stone_846:
                case VeinID.Stone_847:
                    break;
                //姜岛采石场的
                case VeinID.Fossil_816:
                case VeinID.Fossil_817:
                case VeinID.Clay_818:
                    break;
                //沙漠节
                case VeinID.Calico0:
                case VeinID.Calico1:
                case VeinID.Calico2:
                    break;
            }
            MissionManager.Increase(MissionID.MineStones);
        }*/

        [HarmonyPatch(nameof(GameLocation.checkAction))]
        [HarmonyTranspiler]
        private static List<CodeInstruction> CheckAction(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            for (int i = 0; i < codes.Count; i++)
            {
                var code = codes[i];
                if (code.opcode == OpCodes.Callvirt && code.Contains("set_ItemsForaged"))
                {
                    codes.MissionIncrease(ref i, MissionID.ForageItems);
                    List<CodeInstruction> list = new()
                    {
                        new(OpCodes.Ldarg_0),
                        new(OpCodes.Ldloc_3),
                        new(OpCodes.Ldarg_1),
                        ILHelper.Call(typeof(MissionBonus),nameof(MissionBonus.ExtraForage))
                    };
                    codes.InsertRange(i + 1, list);
                    break;
                }
            }
            return codes;
        }

        [HarmonyPatch(nameof(GameLocation.CheckGarbage))]
        [HarmonyTranspiler]
        private static List<CodeInstruction> CheckGarbage(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            for (int i = 0; i < codes.Count; i++)
            {
                var code = codes[i];
                if (code.opcode != OpCodes.Callvirt || code.Contains("Increament"))
                    continue;
                codes.MissionIncrease(ref i, MissionID.CheckGarbages);
                //添加新的垃圾桶战利品
            }
            return codes;
        }

        [HarmonyPatch(nameof(GameLocation.checkForBuriedItem))]
        [HarmonyPostfix]
        private static void CheckForBuriedItem(this GameLocation __instance, int xLocation, int yLocation, bool explosion, bool detectOnly, Farmer who)
        {
            MissionBonus.ExtraClay(__instance, xLocation, yLocation, who);
        }

        [HarmonyPatch(nameof(GameLocation.digUpArtifactSpot))]
        [HarmonyPostfix]
        private static void DigUpArtifactSpot(int xLocation, int yLocation, Farmer who)
        {
            MissionManager.Increase(MissionID.DigArtifactSpots);
        }

        [HarmonyPatch("onMonsterKilled")]
        [HarmonyPrefix]
        private static void OnMonsterKilled_Prefix(GameLocation __instance, Farmer who, Monster monster, Rectangle monsterBox, bool killedByBomb)
        {
            MissionManager.Increase(MissionID.KillMonsters);
            if (monster.isHardModeMonster.Value)
            {
                MissionManager.Increase(MissionID.KillHardModeMonsters);
            }
        }

        [HarmonyPatch("onMonsterKilled")]
        [HarmonyTranspiler]
        private static List<CodeInstruction> OnMonsterKilled_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            for (int i = 0; i < codes.Count; i++)
            {
                var code = codes[i];
                if (code.opcode != OpCodes.Callvirt)
                    continue;
                if (!code.Contains("monsterDrop"))
                    continue;
                codes.RemoveAt(i);
                codes.Insert(i, ILHelper.Call(typeof(MissionBonus), nameof(MissionBonus.ExtraMonsterDrop)));
                break;
            }
            return codes;
        }

        [HarmonyPatch("breakStone")]
        [HarmonyPostfix]
        private static void BreakStone(GameLocation __instance, string stoneId, int x, int y, Farmer who, Random r)
        {
            MissionManager.Increase(MissionID.MineStones);
            MissionBonus.ExtraVine(__instance, stoneId, x, y, r);
        }
    }
}
