using StardropScroll.Helper;

namespace StardropScroll.IDs
{
    public static class MissionID
    {
        public static string MissionKey(string name) => $"{Main.UniqueID}/mission/{name}";
        public const string CutTrees = "CutTrees";
        public const string MiningStones = "MiningStones";
    }
}
