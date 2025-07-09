using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Constants;
using StardewValley.Extensions;
using StardewValley.Locations;
using StardewValley.Monsters;
using StardewValley.Objects.Trinkets;
using StardewValley.TerrainFeatures;
using StardropScroll.Helper;
using StardropScroll.IDs;
using Object = StardewValley.Object;

namespace StardropScroll.Content.Mission
{
    public static class MissionBonus
    {
        public static int GetLevel(string name) => MissionManager.GetLevel(name);
        public static int GetBonusTimes(int level, double init, double fix = 0.95, Random r = null, bool failureBreak = false) => MissionManager.GetBonusTimes(level, init, fix, r, failureBreak);
        public static void ExtraClay(GameLocation location, int x, int y, Farmer who)
        {
            int level = GetLevel(MissionID.HoeDirts);
            if (level <= 0)
                return;
            ItemHelper.CreateDroppedItem(ItemID.Clay, x, y, location, GetBonusTimes(level, 0.5));
        }

        public static void ExtraForage(GameLocation location, Object obj, xTile.Dimensions.Location tile)
        {
            int level = GetLevel(MissionID.ForageItems);
            if (level <= 0)
                return;
            ItemHelper.CreateDroppedItem(obj.QualifiedItemId, tile.X, tile.Y, location, quality: obj.Quality,
               stack: GetBonusTimes(level, 1, 0.9, Utility.CreateRandom(tile.X * 2000, tile.Y * 700)));
        }

        public static void ExtraWoodDrop(Tree tree)
        {
            int level = GetLevel(MissionID.CutTrees);
            if (level <= 0)
                return;
            var tile = tree.Tile.ToPoint();
            int amount = 12 + ExtraWoodCalculator(tile, tree.treeType.Value);
            amount *= GetBonusTimes(level, 0.9);
            ItemHelper.CreateDroppedItem(ItemID.Wood, new(tile.X + (tree.shakeLeft.Value ? -4 : 4), tile.Y), tree.Location, amount);
        }

        private static int ExtraWoodCalculator(Point tileLocation, string treeType)
        {
            Random random = Utility.CreateRandom(Game1.uniqueIDForThisGame, Game1.stats.DaysPlayed, tileLocation.X * 7.0, tileLocation.Y * 11.0, 0.0);
            int extraWood = 0;
            if (random.NextDouble() < Game1.player.DailyLuck)
            {
                extraWood++;
            }
            if (random.NextDouble() < Game1.player.ForagingLevel / 12.5)
            {
                extraWood++;
            }
            if (random.NextDouble() < Game1.player.ForagingLevel / 12.5)
            {
                extraWood++;
            }
            if (random.NextDouble() < Game1.player.LuckLevel / 25.0)
            {
                extraWood++;
            }
            if (treeType == "3")
            {
                extraWood++;
            }
            return extraWood;
        }
        public static void ExtraGrow(HoeDirt dirt)
        {
            int level = MissionManager.GetLevel(MissionID.PlantCrops);
            if (level <= 0)
                return;
            if (dirt.crop == null)
                return;
            bool ignore = dirt.hasPaddyCrop() && dirt.paddyWaterCheck(true);
            if (ignore || dirt.state.Value == 1)
            {
                int amount = MissionManager.GetBonusTimes(level, 0.75, 0.9, RandomHelper.ByDayPlays());
                for (int i = 0; i < amount; i++)
                    dirt.crop.newDay(1);
            }
        }
        public static void ExtraVine(GameLocation location, string stoneId, int x, int y, Random r)
        {
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
                    ex = r.NextBool() ? ItemID.Coal : ItemID.CopperOre;
                    exChance = 0.1;
                    break;
                //冰雪层
                case VeinID.Snowy_48:
                case VeinID.Snowy_50:
                case VeinID.Snowy_52:
                case VeinID.Snowy_54:
                    ex = r.NextBool() ? ItemID.Coal : ItemID.IronOre;
                    exChance = 0.1;
                    break;
                //熔岩层
                case VeinID.Snowy_56:
                case VeinID.Snowy_58:
                case VeinID.Stone_760:
                case VeinID.Stone_762:
                    ex = r.NextBool() ? ItemID.Coal : ItemID.GoldOre;
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
                    if (r.NextBool())
                    {
                        ex = ItemID.Coal;
                        exChance = 0.1;
                    }
                    else
                    {
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
                    }
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
                ItemHelper.CreateDroppedItem(id, x, y, location, GetBonusTimes(level, 1));
            if (ex != null)
                ItemHelper.CreateDroppedItem(ex, x, y, location, GetBonusTimes(level, exChance));
        }
        public static void ExtraItemValue(Object obj, ref float multiplier)
        {
            if (obj.Type?.Contains("Arch") == true)
                multiplier += GetLevel(MissionID.DigArtifactSpots);
            if (obj.Category == Object.FishCategory)
                multiplier += GetLevel(MissionID.CatchFishes);
        }
        public static void ExtraFishTreasure(ref bool treasure, ref bool golden)
        {
            int level = GetLevel(MissionID.CatchFishTreasures);
            Random r = Random.Shared;
            if (!treasure)
                treasure = r.Next(10) < level;
            if (treasure && Game1.player.MasteryFishing())
                golden = r.Next(10) < GetBonusTimes(GetLevel(MissionID.CatchGoldenFishTreasures), 1);
        }

        public static void ExtraFriendShip(ref int amount)
        {
            if (amount <= 0)
                return;
            float value = amount;
            value *= 1 + GetBonusTimes(GetLevel(MissionID.GiveGifts), 1) * 0.3f;
            amount = (int)Math.Round(value);
        }

        public static void ExtraMonsterDrop(GameLocation location, Monster monster, int x, int y, Farmer who)
        {
            int times = GetBonusTimes(GetLevel(MissionID.KillMonsters), 1);
            int hard = GetLevel(MissionID.KillHardModeMonsters);
            for (int t = 0; t < times; t++)
            {
                IList<string> objects = new List<string>();
                Vector2 playerPosition = Utility.PointToVector2(who.StandingPixel);
                List<Item> extraDrops = monster.getExtraDropItems();
                if (DataLoader.Monsters(Game1.content).TryGetValue(monster.Name, out string result))
                {
                    string[] objectsSplit = ArgUtility.SplitBySpace(result.Split('/', StringSplitOptions.None)[6]);
                    for (int i = 0; i < objectsSplit.Length; i += 2)
                    {
                        if (Game1.random.NextDouble() < Convert.ToDouble(objectsSplit[i + 1]))
                        {
                            objects.Add(objectsSplit[i]);
                        }
                    }
                    if (who.timesReachedMineBottom >= 1)
                    {
                        if (Game1.random.NextDouble() < 0.01)
                        {
                            objects.Add("72");
                        }
                        if (Game1.random.NextDouble() < 0.0035)
                        {
                            objects.Add("74");
                        }
                    }
                    if (Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS") && Game1.random.NextDouble() < 0.03)
                    {
                        objects.Add("890");
                    }
                }
                List<Debris> debrisToAdd = new();
                for (int k = 0; k < objects.Count; k++)
                {
                    string objectToAdd = objects[k];
                    if (objectToAdd != null && objectToAdd.StartsWith('-') && int.TryParse(objectToAdd, out int parsedIndex))
                    {
                        debrisToAdd.Add(monster.ModifyMonsterLoot(new Debris(Math.Abs(parsedIndex), Game1.random.Next(1, 4), new Vector2((float)x, (float)y), playerPosition, 1f)));
                    }
                    else
                    {
                        debrisToAdd.Add(monster.ModifyMonsterLoot(new Debris(objectToAdd, new Vector2((float)x, (float)y), playerPosition)));
                    }
                }
                for (int l = 0; l < extraDrops.Count; l++)
                {
                    debrisToAdd.Add(monster.ModifyMonsterLoot(new Debris(extraDrops[l], new Vector2((float)x, (float)y), playerPosition)));
                }
                Trinket.TrySpawnTrinket(location, monster, monster.getStandingPosition(), 1.0);
                foreach (Debris d in debrisToAdd)
                {
                    location.debris.Add(d);
                }
                if (location.HasUnlockedAreaSecretNotes(who) && Game1.random.NextDouble() < 0.033)
                {
                    Object o = location.tryToCreateUnseenSecretNote(who);
                    if (o != null)
                    {
                        monster.ModifyMonsterLoot(Game1.createItemDebris(o, new Vector2((float)x, (float)y), -1, location, -1, false));
                    }
                }
                if (who != null && who.stats.Get(StatKeys.Mastery(0)) != 0U && Game1.random.NextDouble() < 0.001)
                {
                    Game1.createItemDebris(ItemRegistry.Create("(O)GoldenAnimalCracker", 1, 0, false), new Vector2((float)x, (float)y), -1, location, -1, false);
                }
                if (Game1.stats.DaysPlayed > 2U && Game1.random.NextDouble() < 0.003)
                {
                    Game1.createItemDebris(Utility.getRandomCosmeticItem(Game1.random), new Vector2((float)x, (float)y), -1, location, -1, false);
                }
                if (Game1.stats.DaysPlayed > 2U && Game1.random.NextDouble() < 0.0009)
                {
                    Game1.createItemDebris(ItemRegistry.Create("(O)SkillBook_" + Game1.random.Next(5).ToString(), 1, 0, false), new Vector2((float)x, (float)y), -1, location, -1, false);
                }
                if (Utility.tryRollMysteryBox(0.01))
                {
                    monster.ModifyMonsterLoot(Game1.createItemDebris(ItemRegistry.Create((who.stats.Get(StatKeys.Mastery(2)) != 0U) ? "(O)GoldenMysteryBox" : "(O)MysteryBox", 1, 0, false), new Vector2((float)x, (float)y), -1, location, -1, false));
                }
                if (who.mailReceived.Contains("voidBookDropped") && Game1.random.NextDouble() < 0.0005)
                {
                    monster.ModifyMonsterLoot(Game1.createItemDebris(ItemRegistry.Create("(O)Book_Void", 1, 0, false), new Vector2((float)x, (float)y), -1, location, -1, false));
                }
                if (location is Woods && Game1.random.NextDouble() < 0.1)
                {
                    monster.ModifyMonsterLoot(Game1.createItemDebris(ItemRegistry.Create("(O)292", 1, 0, false), new Vector2((float)x, (float)y), -1, location, -1, false));
                }
                if (Game1.netWorldState.Value.GoldenWalnutsFound >= 100 && monster.isHardModeMonster.Value)
                {
                    if (Game1.stats.Get("hardModeMonstersKilled") > 50U && Game1.random.NextDouble() < 0.001 * hard)
                    {
                        monster.ModifyMonsterLoot(Game1.createItemDebris(ItemRegistry.Create("(O)896", 1, 0, false), new Vector2((float)x, (float)y), -1, location, -1, false));
                    }
                    if (Game1.random.NextDouble() < 0.008 * hard)
                    {
                        monster.ModifyMonsterLoot(Game1.createItemDebris(ItemRegistry.Create("(O)858", 1, 0, false), new Vector2((float)x, (float)y), -1, location, -1, false));
                    }
                }
            }

            if (Main.Config.ReplaceUnStackable)
            {
                List<Debris> target = new();
                foreach (var d in location.debris)
                {
                    if (d.item == null)
                        continue;
                    if (d.item.TypeDefinitionId != TypeDefinitionID.Object)
                    {
                        target.Add(d);
                    }
                }
                int amount = 0;
                foreach (var d in target)
                {
                    location.debris.Remove(d);
                    int value = d.item.sellToStorePrice();
                    if (value <= 0)
                        value = 100;
                    amount += value;
                }
                if (amount <= 0)
                    return;
                Game1.playSound("moneyDial", null);
                Game1.dayTimeMoneyBox.gotGoldCoin(amount);
            }
        }

        public static void ExtraTreeGrowChance(Tree t)
        {
            int level = GetLevel(MissionID.PlantWildTrees);
            if (level <= 0)
                return;
            if (t.IsGrowthBlockedByNearbyTree())
                return;
            if (t.growthStage.Value >= 5)
                return;
            if (GetBonusTimes(level, 0.1, 1) > 0)
            {
                t.growthStage.Value++;
            }
            else if (GetBonusTimes(level, 0.025, 1) > 0)
            {
                t.growthStage.Value = 5;
            }
        }

        public static void ExtraTreeMossChance(Tree t)
        {
            int level = GetLevel(MissionID.HarvestMoss);
            if (level <= 0)
                return;
            //t.stopGrowingMoss 这是用了醋让树不长苔藓
            if (t.Location.IsGreenhouse && !Game1.getOnlineFarmers().Any(x => x.MasteryForaging()))
                return;
            if (t.hasMoss.Value)
                return;
            if (t.growthStage.Value < 5)
                return;
            if (Game1.random.NextBool(level / 10.0))
                t.hasMoss.Value = true;
        }

        public static void ExtraTreeImmediatelyTapper(Tree t)
        {
            int level = GetLevel(MissionID.PlantWildTrees);
            if (level <= 0)
                return;
            if (!t.tapped.Value)
                return;
            var tile = t.Tile.ToPoint();
            Object tapper = t.Location.getObjectAtTile(tile.X, tile.Y, false);
            if (tapper.MinutesUntilReady > 0)
            {
                if (Game1.random.NextBool(level / 20))
                    tapper.MinutesUntilReady = 0;
            }
        }

        public static void ExtraFruitTreeGrow(FruitTree t)
        {
            //t.growthRate 这玩意基于树苗星级，也就是种的更久的树重新长得更快那个设定
            int level = GetLevel(MissionID.PlantFruitTrees);
            t.daysUntilMature.Value -= GetBonusTimes(level, 0.167, 0.833, RandomHelper.ByDayPlays());
        }

        public static void ExtraWeedsDrop(Object o)
        {
            var location = o.Location;
            var pos = o.TileLocation;
            int level = GetLevel(MissionID.ClearWeeds);
            var debris = location.debris;
            int amount = GetBonusTimes(level, 0.75);
            if (amount > 0)
                debris.Add(new(ItemRegistry.Create(ItemID.Fiber, amount), pos));
            amount = GetBonusTimes(level, 0.5);
            if (amount > 0)
                debris.Add(new(ItemRegistry.Create(ItemID.Moss, amount), pos));
            amount = GetBonusTimes(level, 0.33);
            if (amount > 0)
                debris.Add(new(ItemRegistry.Create(ItemID.MixedSeeds, amount), pos));
            amount = GetBonusTimes(level, 0.25);
            if (amount > 0)
                debris.Add(new(ItemRegistry.Create(ItemID.MixedFlowerSeeds, amount), pos));
        }

        public static void ExtraAnimalFriendShip(FarmAnimal animal)
        {
            int times = GetBonusTimes(GetLevel(MissionID.PetAnimals), 0.9);
            animal.friendshipTowardFarmer.Value = Math.Min(1000, animal.friendshipTowardFarmer.Value + 15 * times);
        }
        private static void AddDropCount(this Dictionary<string, int> id_stack, string id, int add = 1)
        {
            id_stack.TryGetValue(id, out int stack);
            id_stack[id] = stack + add;
        }
    }
}
