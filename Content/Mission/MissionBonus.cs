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
            int count = GetBonusTimes(GetLevel(MissionID.HoeDirts), 0.25);
            if (count <= 0)
                return;
            ItemHelper.CreateDroppedItem(ItemID.Clay, x, y, location, count);
        }

        public static void ExtraForage(GameLocation location, Object obj, xTile.Dimensions.Location tile)
        {
            int level = GetLevel(MissionID.ForageItems);
            if (level <= 0)
                return;
            ItemHelper.CreateDroppedItem(obj.QualifiedItemId, tile.X, tile.Y, location, quality: obj.Quality,
               stack: GetBonusTimes(level, 1, 0.9, Utility.CreateRandom(tile.X * 2000, tile.Y * 700)), moveToPlayer: true);
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
        public static void ExtraCropGrow(HoeDirt dirt)
        {
            int level = MissionManager.GetLevel(MissionID.PlantCrops);
            if (level <= 0)
                return;
            if (dirt.crop == null)
                return;
            bool ignore = dirt.hasPaddyCrop() && dirt.paddyWaterCheck(true);
            if (ignore || dirt.state.Value == 1)
            {
                int amount = GetBonusTimes(level, 0.75, 0.9, RandomHelper.ByDayPlays(MissionID.PlantCrops.GetHashCode()));
                for (int i = 0; i < amount; i++)
                    dirt.crop.newDay(1);
            }
        }

        public static void ExtraVein(GameLocation location, string stoneId, int x, int y, Farmer who, Random r)
        {
            int luckLevel = who?.LuckLevel ?? 0;
            int miningLevel = who?.MiningLevel ?? 0;
            bool hasMiningMastery = who?.stats.Get(StatKeys.Mastery(3)) != 0;
            bool Island = location is IslandLocation;
            bool outDoor = location.IsOutdoors;
            bool treatAsOutDoor = location.treatAsOutdoors.Value;
            double dailyLuck = who?.DailyLuck ?? 0.0;
            double luckFactor = dailyLuck / 2.0 + miningLevel * 0.005 + luckLevel * 0.001;
            var pos = new Vector2(x, y) * 64 + new Vector2(32);
            double coalChance = 0.0;
            Random random = Utility.CreateDaySaveRandom(x * 1000, y);
            if (who != null)
            {
                if (who.professions.Contains(21))
                {
                    coalChance += 0.05 * (1.0 + luckFactor);
                }

                if (who.hasBuff("dwarfStatue_2"))
                {
                    coalChance += 0.025;
                }
            }

            Dictionary<string, int> drops = new();
            if (stoneId == VeinID.Gem_44)
            {
                stoneId = (r.Next(1, 8) * 2).ToString();
            }
            int times = GetBonusTimes(GetLevel(MissionID.MineStones), 0.75);
            for (int i = 0; i < times; i++)
            {
                switch (stoneId)
                {
                    case "95":
                        drops.AddDrop("(O)909", 1);
                        break;

                    case "843":
                    case "844":
                        drops.AddDrop("(O)848", 1);
                        break;

                    case "25":
                        drops.AddDrop("(O)719", r.Next(2, 5));
                        if (Island && r.NextDouble() < 0.1)
                        {
                            Game1.player.team.RequestLimitedNutDrops("MusselStone", location, x * 64, y * 64, 5);
                        }
                        break;

                    case "75":
                        drops.AddDrop("(O)535", 1);
                        break;

                    case "76":
                        drops.AddDrop("(O)536", 1);
                        break;

                    case "77":
                        drops.AddDrop("(O)537", 1);
                        break;

                    case "816":
                    case "817":
                        if (r.NextDouble() < 0.1)
                        {
                            drops.AddDrop("(O)823", 1);
                        }
                        else if (r.NextDouble() < 0.015)
                        {
                            drops.AddDrop("(O)824", 1);
                        }
                        else if (r.NextDouble() < 0.1)
                        {
                            drops.AddDrop($"(O){579 + r.Next(11)}", 1);
                        }
                        drops.AddDrop("(O)881", 1);
                        break;

                    case "818":
                        drops.AddDrop("(O)330", 1);
                        break;

                    case "819":
                        drops.AddDrop("(O)749", 1);
                        break;

                    case "8":
                        int qty66 = (who == null || !hasMiningMastery) ? 1 : 2;
                        drops.AddDrop("(O)66", qty66);
                        break;

                    case "10":
                        int qty68 = (who == null || !hasMiningMastery) ? 1 : 2;
                        drops.AddDrop("(O)68", qty68);
                        break;

                    case "12":
                        int qty60 = (who == null || !hasMiningMastery) ? 1 : 2;
                        drops.AddDrop("(O)60", qty60);
                        break;

                    case "14":
                        int qty62 = (who == null || !hasMiningMastery) ? 1 : 2;
                        drops.AddDrop("(O)62", qty62);
                        break;

                    case "6":
                        int qty70 = (who == null || !hasMiningMastery) ? 1 : 2;
                        drops.AddDrop("(O)70", qty70);
                        break;

                    case "4":
                        int qty64 = (who == null || !hasMiningMastery) ? 1 : 2;
                        drops.AddDrop("(O)64", qty64);
                        break;

                    case "2":
                        int qty72 = (who == null || !hasMiningMastery) ? 1 : 2;
                        drops.AddDrop("(O)72", qty72);
                        break;

                    case "846":
                    case "847":
                    case "668":
                    case "845":
                    case "670":
                        drops.AddDrop("(O)390", 1);
                        if (r.NextDouble() < 0.08)
                        {
                            drops.AddDrop("(O)382", 1);
                        }
                        break;

                    case "849":
                    case "751":
                        drops.AddDrop("(O)378", 1);
                        break;

                    case "850":
                    case "290":
                        drops.AddDrop("(O)380", 1);
                        break;

                    case "BasicCoalNode0":
                    case "BasicCoalNode1":
                    case "VolcanoCoalNode0":
                    case "VolcanoCoalNode1":
                        drops.AddDrop("(O)382", 1);
                        break;

                    case "764":
                    case "VolcanoGoldNode":
                        drops.AddDrop("(O)384", 1);
                        break;

                    case "765":
                        drops.AddDrop("(O)386", 1);
                        if (r.NextDouble() < 0.035)
                        {
                            drops.AddDrop("(O)74", 1);
                        }
                        break;

                    case "CalicoEggStone_0":
                    case "CalicoEggStone_1":
                    case "CalicoEggStone_2":
                        drops.AddDrop("CalicoEgg", 1);
                        break;
                    case "46":
                        drops.AddDrop(ItemID.CopperOre, r.Next(1, 4));
                        drops.AddDrop(ItemID.IronOre, r.Next(1, 5));
                        if (r.NextDouble() < 0.25)
                        {
                            drops.AddDrop("(O)74", 1);
                        }
                        break;
                }

                if ((!outDoor && !treatAsOutDoor) ||
                    !string.IsNullOrEmpty(stoneId) && stoneId != "0" && stoneId != "1")
                    return;

                drops.AddDrop(ItemID.Stone, 1);
                if (random.NextDouble() < coalChance)
                {
                    drops.AddDrop("(O)382", 1);
                }

                if (random.NextDouble() < 0.05 * (1.0 + luckFactor))
                {
                    drops.AddDrop("(O)382", 1);
                }
            }

            foreach (var (id, stack) in drops)
            {
                ItemHelper.CreateDroppedItem(id, pos, location, stack);
            }
            ReplaceUnstackable(location);
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

        public static void GiveStardropTeaEvent(string npcName, Farmer player)
        {
            if (!Main.IsMaster)
            {
                Main.NetSend(npcName, NetMessageID.SendStarDropTea);
                return;
            }
            switch (npcName)
            {
                case NPCID.Robin:
                    InstantBuilding();
                    break;
                case NPCID.Demetrius:
                    SpecialRain();
                    break;
                case NPCID.Clint:
                    if (player == null)
                        break;
                    InstantTool(player);
                    break;
            }
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
            ReplaceUnstackable(location);
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
            t.daysUntilMature.Value -= GetBonusTimes(level, 0.167, 0.833, RandomHelper.ByDayPlays(MissionID.PlantFruitTrees.GetHashCode()));
        }

        public static void ExtraWeedsDrop(Object o, Farmer who)
        {
            GameLocation location = o.Location;
            Vector2 centerPosition = o.tileLocation.Value * 64f + new Vector2(32f, 32f);
            double wildSeedBonus = who.stats.Get("Book_WildSeeds") > 0u ? 0.04 : 0.0;
            bool isSummer = Game1.currentSeason == "summer";
            bool canDropQiBeans = Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS");
            bool isGreenRainWeed = o.name.Contains("GreenRainWeeds");
            int times = GetBonusTimes(GetLevel(MissionID.ClearWeeds), 0.75);

            Dictionary<string, int> drops = new();

            for (int i = 0; i < times; i++)
            {
                string dropItemId = string.Empty;
                bool isBreakingGlass = false;

                if (Game1.random.NextBool())
                {
                    dropItemId = "771";
                }
                else if (Game1.random.NextDouble() < 0.05 + wildSeedBonus)
                {
                    dropItemId = "770";
                }
                else if (isSummer && Game1.random.NextDouble() < 0.05 + wildSeedBonus)
                {
                    dropItemId = "MixedFlowerSeeds";
                }

                if (isGreenRainWeed && Game1.random.NextDouble() < 0.1)
                {
                    dropItemId = "Moss";
                }

                switch (o.QualifiedItemId)
                {
                    case "(O)319":
                    case "(O)320":
                    case "(O)321":
                        isBreakingGlass = true;
                        if (Game1.random.NextDouble() < 0.0025)
                        {
                            dropItemId = "338";
                        }
                        break;

                    case "(O)793":
                    case "(O)794":
                    case "(O)792":
                        dropItemId = "770";
                        break;

                    case "(O)883":
                    case "(O)884":
                    case "(O)882":
                        if (Game1.random.NextDouble() < 0.01)
                        {
                            dropItemId = "828";
                        }
                        else if (Game1.random.NextDouble() < 0.08)
                        {
                            dropItemId = "831";
                        }
                        break;
                }

                if (!string.IsNullOrEmpty(dropItemId))
                {
                    drops.AddDrop(dropItemId, 1);
                }

                if (!isBreakingGlass)
                {
                    if (Game1.random.NextDouble() < 1E-05)
                    {
                        drops.AddDrop("(H)40", 1);
                    }

                    if (canDropQiBeans && Game1.random.NextDouble() <= 0.01)
                    {
                        drops.AddDrop("(O)890", 1);
                    }
                }
            }

            foreach (var drop in drops)
            {
                Item item = ItemRegistry.Create(drop.Key, drop.Value);
                location.debris.Add(new Debris(item, centerPosition));
            }

            ReplaceUnstackable(location);
        }

        public static void ExtraAnimalFriendShip(FarmAnimal animal)
        {
            int times = GetBonusTimes(GetLevel(MissionID.PetAnimals), 0.9);
            animal.friendshipTowardFarmer.Value = Math.Min(1000, animal.friendshipTowardFarmer.Value + 15 * times);
        }

        public static void RefreshBuffs()
        {
            Game1.player.buffs.Apply(new(I18n.Mission_Buff(), duration: Buff.ENDLESS, effects: new()
            {
                FarmingLevel = { GetLevel(MissionID.PlantCrops) },
                MiningLevel = { GetLevel(MissionID.MineStones) },
                ForagingLevel = { GetLevel(MissionID.ForageItems) },
                FishingLevel = { GetLevel(MissionID.CatchFishes) },
                CombatLevel = { GetLevel(MissionID.KillMonsters) },
                LuckLevel = { GetLevel(MissionID.CheckGarbages) },
                Speed = { GetLevel(MissionID.RunTrain) / 5f },
            })
            { visible = false });

        }
        private static void ReplaceUnstackable(GameLocation location)
        {
            if (!Main.Config.ReplaceUnStackable)
                return;
            List<Debris> target = new();
            foreach (var d in location.debris)
            {
                if (d.item == null)
                    continue;
                if (d.item.TypeDefinitionId != ItemDefinitionID.Object)
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

        private static void InstantBuilding()
        {
            foreach (var location in Game1.locations)
            {
                foreach (var build in location.buildings)
                {
                    do
                        build.dayUpdate(0);
                    while (build.daysOfConstructionLeft.Value > 0 || build.daysUntilUpgrade.Value > 0);
                }
            }
        }

        private static void InstantTool(Farmer who)
        {
            var tool = who.toolBeingUpgraded.Value;
            if (tool == null)
                return;
            who.daysLeftForToolUpgrade.Value = 0;
        }

        private static void SpecialRain()
        {
            ref string weather = ref Game1.weatherForTomorrow;
            if (weather == Game1.weather_lightning)
                Game1.netWorldState.Value.WeatherForTomorrow = (weather = Game1.weather_green_rain);
            if (weather == Game1.weather_rain)
                Game1.netWorldState.Value.WeatherForTomorrow = (weather = Game1.weather_lightning);
        }

        // 添加掉落物到字典
        private static void AddDrop(this Dictionary<string, int> drops, string itemId, int quantity)
        {
            if (drops.ContainsKey(itemId))
            {
                drops[itemId] += quantity;
            }
            else
            {
                drops[itemId] = quantity;
            }
        }
    }
}
