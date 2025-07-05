using Microsoft.Xna.Framework;
using StardewValley;
using StardropScroll.IDs;

namespace StardropScroll.Helper
{
    public static class ItemHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniqueID"></param>
        /// <param name="stack"></param>
        /// <param name="quality"><see cref="QualityID"/></param>
        /// <returns></returns>
        public static Item CreateItem(string uniqueID, int stack = 1, int quality = 0)
            => ItemRegistry.Create(uniqueID, stack, quality);

        public static void CreateDroppedItem(string uniqueID, Vector2 postion, GameLocation location = null, int stack = 1, int quality = QualityID.Common)
        {
            Item item = CreateItem(uniqueID, stack, quality);
            Game1.createItemDebris(item, postion, 4, location);
        }
    }
}
