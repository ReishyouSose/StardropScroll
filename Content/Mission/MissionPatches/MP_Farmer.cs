using HarmonyLib;
using StardewValley;
using StardropScroll.IDs;

namespace StardropScroll.Content.Mission.MissionPatches
{
    [HarmonyPatch(typeof(Farmer))]
    public static class MP_Farmer
    {
        [HarmonyPatch(nameof(Farmer.OnItemReceived))]
        [HarmonyPrefix]
        private static void OnItemReceived(Farmer __instance, Item item, int countAdded, Item mergedIntoStack, bool hideHudNotification = false)
        {
            Farmer player = __instance;
            if (!player.IsLocalPlayer || item.HasBeenInInventory)
            {
                return;
            }
            Item actualItem = mergedIntoStack ?? item;
            if (actualItem.QualifiedItemId == ItemID.PrismaticShard)
            {
                MissionManager.Increase(MissionID.ObtainPrismaticShards, actualItem.stack.Value);
            }
        }

        [HarmonyPatch(nameof(Farmer.changeFriendship))]
        [HarmonyPrefix]
        private static void ChangeFriendship(ref int amount)
        {
            MissionBonus.ExtraFriendShip(ref amount);
        }
    }
}
