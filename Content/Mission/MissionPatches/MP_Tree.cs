using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardropScroll.Helper;
using System.Reflection.Emit;

namespace StardropScroll.Content.Mission.MissionPatches
{
    internal class MP_Tree
    {
        public static IEnumerable<CodeInstruction> ModifyTreeDrop(IEnumerable<CodeInstruction> instructions, ILGenerator il)
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
                    ILHelper.Call<MP_Tree>(nameof(ExtraWoodDrop)),
                };
                codes.InsertRange(i, list);
                break;
            }
            return codes;
        }
        private static void ExtraWoodDrop(Tree tree)
        {
            var tile = tree.Tile.ToPoint();
            Farmer lastHitBy = Game1.GetPlayer(tree.lastPlayerToHit.Value, false) ?? Game1.MasterPlayer;
            int amount = 12 + ExtraWoodCalculator(tile, tree.treeType.Value);
            amount += (int)lastHitBy.stats.Get("TreesChopped") * 10;
            Game1.createRadialDebris(tree.Location, 12, tile.X + (tree.shakeLeft.Value ? -4 : 4), tile.Y, amount, true, -1, false, null);
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
    }
}
