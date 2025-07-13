using StardewModdingAPI.Events;
using StardewValley;
using StardropScroll.Content.Mission;
using StardropScroll.Helper;
using StardropScroll.IDs;

namespace StardropScroll.Common
{
    public static class NetPackage
    {
        public static void HandleNetPackages(object? sender, ModMessageReceivedEventArgs e)
        {
            if (!int.TryParse(e.Type, out int messageID))
            {
                Main.LogWarn("Net receive error");
                return;
            }
            switch (messageID)
            {
                case NetMessageID.Mission:
                    MissionManager.NetReceive(e);
                    break;
                case NetMessageID.SendStarDropTea:
                    MissionBonus.GiveStardropTeaEvent(e.ReadAs<string>(), Game1.GetPlayer(e.FromPlayerID, true));
                    break;
            }
        }
    }
}
