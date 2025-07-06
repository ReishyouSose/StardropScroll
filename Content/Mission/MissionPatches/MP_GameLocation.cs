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
                    break;
                    //补充额外采集物
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
            ExtraClay(__instance, xLocation, yLocation, who);
        }

        [HarmonyPatch(nameof(GameLocation.digUpArtifactSpot))]
        [HarmonyPostfix]
        private static void DigUpArtifactSpot(int xLocation, int yLocation, Farmer who)
        {
            MissionManager.Increase(MissionID.DigArtifactSpots);
        }

        [HarmonyPatch("onMonsterKilled")]
        [HarmonyPrefix]
        private static void OnMonsterKilled(GameLocation __instance, Farmer who, Monster monster, Rectangle monsterBox, bool killedByBomb)
        {
            MissionManager.Increase(MissionID.KillMonsters);
            if (monster.isHardModeMonster.Value)
            {
                MissionManager.Increase(MissionID.KillHardModeMonsters);
            }
            //额外掉落，不调用原版方法
        }

        [HarmonyPatch(nameof(GameLocation.removeObject))]
        [HarmonyPostfix]
        private static void RemoveObject(GameLocation __instance, Vector2 location, bool showDestroyedObject)
        {
            StardewO o;
            __instance.objects.TryGetValue(location, out o);
            if (o != null && (o.CanBeGrabbed || showDestroyedObject))
            {
                if (o.IsWeeds())
                {
                    MissionManager.Increase(MissionID.ClearWeeds);
                }
            }
        }

        [HarmonyPatch("breakStone")]
        [HarmonyPostfix]
        private static void BreakStone(GameLocation __instance, string stoneId, int x, int y, Farmer who, Random r)
        {
            MissionManager.Increase(MissionID.MineStones);
            int level = MissionManager.GetLevel(MissionID.MineStones);
            if (level <= 0)
                return;
            string id = string.Empty;
            string ex = string.Empty;
            double exChance = 0;
            switch (stoneId)
            {
                //宝石矿
                case VeinID.Diamond_2:
                    id = ItemID.Diamond;
                    break;
                case VeinID.Ruby_4:
                    id = ItemID.Ruby;
                    break;
                case VeinID.Jade_6:
                    id = ItemID.Ruby;
                    break;
                case VeinID.Amethyst_8:
                    id = ItemID.Amethyst;
                    break;
                case VeinID.Topaz_10:
                    id = ItemID.Topaz;
                    break;
                case VeinID.Emerald_12:
                    id = ItemID.Emerald;
                    break;
                case VeinID.Aquamarine_14:
                    id = ItemID.Aquamarine;
                    break;
                //宝石和神秘石
                case VeinID.Gem_44:
                    id = $"(O){r.Next(1, 8) * 2}";
                    break;
                case VeinID.Mystic_46:
                    ex = ItemID.PrismaticShard;
                    exChance = 0.25;
                    break;
                //蚌
                case VeinID.Mussel_25:
                    break;
                //很多层都有的那两种
                case VeinID.Stone_668:
                case VeinID.Stone_670:
                    id = ItemID.Stone;
                    ex = ItemID.Coal;
                    exChance = 0.02;
                    break;
                //露天的
                case VeinID.Stone_343:
                case VeinID.Stone_450:
                    id = ItemID.Stone;
                    ex = ItemID.Coal;
                    exChance = 0.02;
                    break;
                //普通层
                case VeinID.Stone_32:
                case VeinID.Stone_34:
                case VeinID.Stone_36:
                case VeinID.Stone_38:
                case VeinID.Stone_40:
                case VeinID.Stone_42:
                    id = ItemID.Stone;
                    ex = ItemID.CopperOre;
                    exChance = 0.1;
                    break;
                //冰雪层
                case VeinID.Snowy_48:
                case VeinID.Snowy_50:
                case VeinID.Snowy_52:
                case VeinID.Snowy_54:
                    ex = ItemID.IronOre;
                    exChance = 0.1;
                    break;
                //熔岩层
                case VeinID.Snowy_56:
                case VeinID.Snowy_58:
                case VeinID.Stone_760:
                case VeinID.Stone_762:
                    ex = ItemID.GoldOre;
                    exChance = 0.1;
                    break;
                //晶球矿
                case VeinID.Geode_75:
                    id = ItemID.Geode;
                    break;
                case VeinID.FrozenGeode_76:
                    id = ItemID.FrozenGeode;
                    break;
                case VeinID.MagmaGeode_77:
                    id = ItemID.MagmaGeode;
                    break;
                case VeinID.OmniGeode_819:
                    id = ItemID.OmniGeode;
                    break;
                //矿石
                case VeinID.Copper_751:
                    id = ItemID.CopperOre;
                    break;
                case VeinID.Iron_290:
                    id = ItemID.IronOre;
                    break;
                case VeinID.Gold_764:
                    id = ItemID.GoldOre;
                    break;
                case VeinID.Iridium_765:
                    id = ItemID.IridiumOre;
                    ex = ItemID.PrismaticShard;
                    exChance = 0.035;
                    break;
                case VeinID.Radioactive_95:
                    id = ItemID.RadioactiveOre;
                    ex = ItemID.PrismaticShard;
                    exChance = 0.05;
                    break;
                //两骷髅矿洞的煤炭
                case VeinID.BasicCoalNode0:
                case VeinID.BasicCoalNode1:
                    id = ItemID.Coal;
                    break;
                //姜岛的矿石
                case VeinID.Copper_849:
                    id = ItemID.CopperOre;
                    ex = ItemID.CinderShard;
                    exChance = 0.1;
                    break;
                case VeinID.Iron_850:
                    id = ItemID.IronOre;
                    ex = ItemID.CinderShard;
                    exChance = 0.1;
                    break;
                case VeinID.VolcanoGoldNode:
                    id = ItemID.GoldOre;
                    ex = ItemID.CinderShard;
                    exChance = 0.1;
                    break;
                case VeinID.VolcanoCoalNode0:
                case VeinID.VolcanoCoalNode1:
                    id = ItemID.Coal;
                    ex = ItemID.CinderShard;
                    exChance = 0.1;
                    break;
                case VeinID.CinderShard_843:
                    id = ItemID.CinderShard;
                    ex = ItemID.DragonTooth;
                    exChance = 0.05;
                    break;
                case VeinID.CinderShard_844:
                    id = ItemID.CinderShard;
                    ex = ItemID.DragonTooth;
                    exChance = 0.05;
                    break;
                //姜岛火山的
                case VeinID.Stone_845:
                case VeinID.Stone_846:
                case VeinID.Stone_847:
                    id = ItemID.Stone;
                    int rng = r.Next(11);
                    if (rng < 4)
                        ex = ItemID.CopperOre;
                    else if (rng < 7)
                        ex = ItemID.IronOre;
                    else if (rng < 9)
                        ex = ItemID.GoldOre;
                    else
                        ex = rng == 9 ? ItemID.IridiumOre : ItemID.CinderShard;
                    exChance = 0.05;
                    break;
                //姜岛采西部石场的
                case VeinID.Fossil_816:
                case VeinID.Fossil_817:
                    id = ItemID.BoneFragment;
                    break;
                case VeinID.Clay_818:
                    id = ItemID.Clay;
                    break;
                //沙漠节
                case VeinID.Calico0:
                case VeinID.Calico1:
                case VeinID.Calico2:
                    id = ItemID.CalicoEgg;
                    break;
            }
            if (id != string.Empty)
                ItemHelper.CreateDroppedItem(id, x, y, __instance, MissionManager.GetBonusTimes(level, 1));
            if (ex != null)
                ItemHelper.CreateDroppedItem(ex, x, y, __instance, MissionManager.GetBonusTimes(level, exChance));
        }

        private static void ExtraClay(GameLocation location, int x, int y, Farmer who)
        {
            int level = MissionManager.GetLevel(MissionID.HoeDirts);
            if (level <= 0)
                return;
            ItemHelper.CreateDroppedItem(ItemID.Clay, x, y, location, MissionManager.GetBonusTimes(level, 0.5));
        }
    }
}
