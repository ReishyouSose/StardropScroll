using HarmonyLib;
using StardewValley;
using StardropScroll.IDs;

namespace StardropScroll.Content.Mission.MissionPatches
{
    [HarmonyPatch(typeof(NPC))]
    public static class MP_NPC
    {
        [HarmonyPatch(nameof(NPC.receiveGift))]
        [HarmonyPrefix]
        private static void ReceiveGift(NPC __instance, StardewValley.Object o, Farmer giver, bool updateGiftLimitInfo, float friendshipChangeMultiplier, bool showResponse)
        {
            NPC npc = __instance;
            if (npc.CanReceiveGifts())
            {
                int taste = npc.getGiftTasteForThisItem(o);
                switch (taste)
                {
                    case 0:
                        MissionManager.Increase(MissionID.GiveGifts);
                        break;
                    case 2:
                        MissionManager.Increase(MissionID.GiveGifts);
                        break;
                    case 4:
                        break;
                    case 6:
                        break;
                    case 7:
                        MissionManager.Increase(MissionID.GiveGifts);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
