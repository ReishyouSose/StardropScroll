using Microsoft.Xna.Framework;
using StardewValley;
using StardropScroll.IDs;

namespace StardropScroll.Helper
{
    public class ItemHelper
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

        public static void CreateDroppedItem(string uniqueID, Vector2 postion, GameLocation location = null, int stack = 1, int quality = QualityID.Common, bool flyToPlayer = false)
        {
            Item item = CreateItem(uniqueID, stack, quality);
            Game1.createItemDebris(item, postion, 4, location).chunksMoveTowardPlayer = flyToPlayer;
        }
        public static void CreateDroppedItem(string uniqueID, int tileX, int tileY, GameLocation location = null, int stack = 1, int quality = QualityID.Common, bool moveToPlayer = false)
        {
            CreateDroppedItem(uniqueID, new(tileX * 64 + 32, tileY * 64 + 32), location, stack, quality, moveToPlayer);
        }
    }
}
