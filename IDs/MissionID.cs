using StardropScroll.Helper;

namespace StardropScroll.IDs
{
    public static class MissionID
    {
        public static string MissionKey(string name) => $"{Main.UniqueID}/mission/{name}";

        /// <summary>砍树</summary>
        public const string CutTrees = nameof(CutTrees);

        /// <summary>挖石头</summary>
        public const string MineStones = nameof(MineStones);

        /// <summary>种植种子</summary>
        public const string PlantCrops = nameof(PlantCrops);

        /// <summary>锄地</summary>
        public const string HoeDirts = nameof(HoeDirts);

        /// <summary>觅食物品</summary>
        public const string ForageItems = nameof(ForageItems);

        /// <summary>翻垃圾桶</summary>
        public const string CheckGarbages = nameof(CheckGarbages);

        /// <summary>挖掘远古斑点</summary>
        public const string DigArtifactSpots = nameof(DigArtifactSpots);

        /// <summary>钓鱼</summary>
        public const string CatchFishes = nameof(CatchFishes);

        /// <summary>钓鱼宝箱</summary>
        public const string CatchFishTreasures = nameof(CatchFishTreasures);

        /// <summary>金色钓鱼宝箱</summary>
        public const string CatchGoldenFishTreasures = nameof(CatchGoldenFishTreasures);

        /// <summary>赠送礼物（喜欢及以上）</summary>
        public const string GiveGifts= nameof(GiveGifts);

        /// <summary>击杀怪物</summary>
        public const string KillMonsters = nameof(KillMonsters);

        /// <summary>击杀困难模式怪物</summary>
        public const string KillHardModeMonsters = nameof(KillHardModeMonsters);
        
        /// <summary>获取五彩碎片</summary>
        public const string ObtainPrismaticShards = nameof(ObtainPrismaticShards);  

        /// <summary>抚摸动物</summary>
        public const string PetAnimals = nameof(PetAnimals);

        /// <summary>收获苔藓</summary>
        public const string HarvestMoss = nameof(HarvestMoss);

        /// <summary>清理杂草</summary>
        public const string ClearWeeds = nameof(ClearWeeds);

        /// <summary>种树</summary>
        public const string PlantTrees= nameof(PlantTrees);
    }
}
