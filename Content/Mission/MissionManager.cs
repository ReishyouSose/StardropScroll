using Newtonsoft.Json;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardropScroll.Helper;
using StardropScroll.IDs;
using System.Collections.ObjectModel;

namespace StardropScroll.Content.Mission
{
    public class PlayerMission
    {
        public string ID;

        public int Current;

        [JsonProperty("target")]
        public int Target { get; set; }

        public int Level;

        [JsonProperty("step")]
        public int Step { get; set; }

        [JsonProperty("max")]
        public int MaxLevel { get; set; }

        public string Data => $"{Current}.{Level}";

        public bool Completed => Level >= MaxLevel;
        public NetMission Net => new() { id = ID, data = Data };
    }
    public struct NetMission
    {
        public string id;
        public string data;
    }
    public static class MissionManager
    {
        private static string Key(string name) => $"{Main.UniqueID}/mission/{name}";
        public static ReadOnlyDictionary<string, PlayerMission> MissionProgress { get; private set; }
        public static void LoadMissionData()
        {
            var data = Main.ReadJsonFile<Dictionary<string, PlayerMission>>(Path.Combine("Assets", "MissionDatas.json"));
            if (data == null)
            {
                Main.LogError("Failed to load Mission Data!");
                return;
            }
            foreach (var (name, mission) in data)
            {
                mission.ID = name;
            }
            MissionProgress = new(data);
        }
        public static void LoadData(Farmer player)
        {
            var data = player.modData;
            foreach (var (name, m) in MissionProgress)
            {
                if (data.TryGetValue(Key(name), out var info))
                {
                    string[] infos = info.Split('.');
                    m.Current = int.Parse(infos[0]);
                    m.Level = int.Parse(infos[1]);
                }
                else
                {
                    m.Current = 0;
                    m.Level = 0;
                }
                m.Target += m.Level * m.Step;
            }
            Main.WriteJsonFile(Path.Combine("Assets", "Stats.json"), Game1.player.stats.Values);
        }
        public static void SubmitMission(PlayerMission m)
        {
            if (m.Current < m.Target || m.Completed)
                return;
            m.Current -= m.Target;
            m.Level++;
            if (Context.IsMainPlayer || !Context.IsMultiplayer)
                return;

            var player = Game1.player;
            player.modData[Key(m.ID)] = m.Data;

            if (!Context.IsMultiplayer || Context.IsMainPlayer)
                return;
            Main.NetSend(m.Net, NetMessageID.Mission);
        }
        public static void NetReceive(ModMessageReceivedEventArgs e)
        {
            Farmer? player = Game1.GetPlayer(e.FromPlayerID);
            if (player == null)
            {
                Main.LogWarn($"Receive mission data but player id {e.FromPlayerID} error");
                return;
            }
            var m = e.ReadAs<NetMission>();
            player!.modData[Key(m.id)] = m.data;
        }
    }
}
